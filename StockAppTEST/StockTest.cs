using Microsoft.Extensions.Logging;
using Moq;
using StockApplication.Code;
using StockApplication.Code.DAL;
using StockApplication.Controllers;
using System.Threading.Tasks;
using System;
using Xunit;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace StockAppTEST
{
    public class StockTest
    {
        private const string _loggedIn = "loggedIn";
        private const string _notLoggedIn = "";
        private const string SessionKeyUser = "_currentUser";
        private const string SessionKeyCompany = "_currentCompany";


        private readonly Mock<IStockRepository> mockRep = new Mock<IStockRepository>();
        private readonly Mock<ILogger<StockController>> mockLog = new Mock<ILogger<StockController>>();

        private readonly Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();
        private readonly MockHttpSession mockSession = new MockHttpSession();


        [Fact]
        public async Task GetUserByID_Found()
        {
            //Arrange
            var user = new User(Guid.NewGuid(), "Luddebassen", "Luddebassen", "Luddebassen", 100);
            var clientUser = new ClientUser(user.username, user.balance); //controller returns a new object clientUser (not all parameters are being sent back to the client)

            mockRep.Setup(u => u.GetUserByID(user.id)).ReturnsAsync(user);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            //Act
            var result = (OkObjectResult)await stockController.GetUserByID(user.id.ToString());
            var resultStr = JsonConvert.SerializeObject(result.Value); //converting result value to string with json 
            var userStr = JsonConvert.SerializeObject(clientUser);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(resultStr, userStr);
        }

        [Fact]
        public async Task GetUserByID_NotFound()
        {
            //Arrange
            Guid id = Guid.NewGuid(); //generating a random id that does not exist for any users
            
            mockRep.Setup(u => u.GetUserByID(id)).ReturnsAsync((User)null); //returns null for User-object if User.id is not found

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            //Act
            var result = (NotFoundResult)await stockController.GetUserByID(id.ToString()); //NotFoundResult because no object is being sent with the Not Found statuscode

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
            
        }
        [Fact]
        public async Task GetUserByUsername_Found_LogInOK()
        {
            //Arrange
            var user = new User(Guid.NewGuid(), "Luddebassen", "Luddebassen", "Luddebassen", 100);
            var clientUser = new ClientUser(user.username, user.balance); //controller returns a new object clientUser (not all parameters are being sent back to the client)

            mockRep.Setup(u => u.GetUserByUsername(user.username)).ReturnsAsync(user);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = user.id.ToString(); //connect id to session
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (OkObjectResult)await stockController.GetUserByUsername(user.username);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode); //OK = OK
            Assert.Equal(JsonConvert.SerializeObject(result.Value), JsonConvert.SerializeObject(clientUser)); //casting to string so both parameters are the same object type
        }
        [Fact]
        public async Task GetUserByUsername_NotFound()
        {
            //Arrange
            string username = "not_found"; //generating a random id that does not exist for any users
            
            mockRep.Setup(u => u.GetUserByUsername(username)).ReturnsAsync((User)null); //returns null for User-object if User.id is not found

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            //do not need sessions because we check if object exists before checking log in

            //Act
            var result = (NotFoundResult)await stockController.GetUserByUsername(username); //NotFoundResult because no object is being sent with the Not Found statuscode

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }
        [Fact]
        public async Task GetUserByUsername_Found_LogInNotOK()
        {
            //Arrange
            var user = new User(Guid.NewGuid(), "Luddebassen", "Luddebassen", "Luddebassen", 100);
          
            mockRep.Setup(u => u.GetUserByUsername(user.username)).ReturnsAsync(user);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = _notLoggedIn; //not logged in
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (UnauthorizedResult)await stockController.GetUserByUsername(user.username); //UnauthorizedResult because no object is being sent with the Unauthorized statuscode

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
        }
        [Fact]
        public async Task GetAllClientUsers() //Not dependent on log-in
        {
            // Arrange
            var user1 = new ClientUser("Luddebassen", 100);
            var user2 = new ClientUser("Luddebassen", 100);
            var user3 = new ClientUser("Luddebassen", 100);


            var userList = new List<ClientUser>();
            userList.Add(user1);
            userList.Add(user2);
            userList.Add(user3);

            mockRep.Setup(k => k.GetAllClientUsers()).ReturnsAsync(userList);

            var stockController = new StockController(mockRep.Object, mockLog.Object);


            // Act
            var resultat = await stockController.GetAllUsers() as OkObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal<List<ClientUser>>((List<ClientUser>)resultat.Value, userList);
        }
        [Fact]
        public async Task CreateUser_dbError()
        {
            //Arrange
            Login newLogin = new Login("luddebassen", "luddebassen");
            
            ServerResponse serverRespons = new ServerResponse(false, "Server Exception!");

            mockRep.Setup(u => u.CreateUser(newLogin.username, newLogin.password)).ReturnsAsync(serverRespons);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            //Act
            var result = (BadRequestResult)await stockController.CreateUser(newLogin);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            
        }
        [Fact]
        public async Task CreateUser_ModelInvalid() //invalid username or password: empty
        {
            //Arrange
            Login newLogin = new Login("", "password");

            var stockController = new StockController(mockRep.Object, mockLog.Object);
            stockController.ModelState.AddModelError("username", "Username or password invalid");

            mockSession[SessionKeyUser] = SessionKeyUser; //logged in
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (BadRequestResult)await stockController.CreateUser(newLogin);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }
        [Fact]
        public async Task CreateUser_dbOK_UserNotFound() //should never happen but just in case
        {
            //Arrange
            Login newLogin = new Login("luddebassen", "luddebassen");
            ServerResponse serverRespons = new ServerResponse(true);

            mockRep.Setup(u => u.CreateUser(newLogin.username, newLogin.password)).ReturnsAsync(serverRespons);
            mockRep.Setup(u => u.GetUserByUsername(newLogin.username)).ReturnsAsync((User)null);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            //Act
            var result = (NotFoundResult)await stockController.CreateUser(newLogin);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);

        }
        [Fact]
        public async Task LogIn_dbError()
        {
            //Arrange
            Login newLogin = new Login("luddebassen", "luddebassen");
            ServerResponse serverRespons = new ServerResponse(false, "Server Exception!");

            mockRep.Setup(u => u.LogIn(newLogin.username, newLogin.password)).ReturnsAsync(serverRespons);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            //Act
            var result = (BadRequestResult)await stockController.LogIn(newLogin);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            
        }
        [Fact]
        public async Task LogIn_dbOK_UserNotFound()
        {
            //Arrange
            Login newLogin = new Login("luddebassen", "luddebassen");
            ServerResponse serverRespons = new ServerResponse(true);
            string serverRespons2 = "User could not be found!";


            mockRep.Setup(u => u.LogIn(newLogin.username, newLogin.password)).ReturnsAsync(serverRespons);
            mockRep.Setup(u => u.GetUserByUsername(newLogin.username)).ReturnsAsync((User)null);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            //Act
            var result = (NotFoundResult)await stockController.LogIn(newLogin);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
            
        }
        [Fact]
        public async Task LogIn_dbOK_UserFound()
        {
            //Arrange
            Login newLogin = new Login("luddebassen", "luddebassen");
            ServerResponse serverRespons = new ServerResponse(true);
            User user = new User(Guid.NewGuid(), newLogin.username, newLogin.password, newLogin.password, 100);


            mockRep.Setup(u => u.LogIn(newLogin.username, newLogin.password)).ReturnsAsync(serverRespons);
            mockRep.Setup(u => u.GetUserByUsername(newLogin.username)).ReturnsAsync(user);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = user.id.ToString();
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (OkResult)await stockController.LogIn(newLogin);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            
        }
        [Fact]
        public async Task Login_ModelInvalid() //invalid username or password: empty
        {
            //Arrange
            Login newLogin = new Login("", "password");

            var stockController = new StockController(mockRep.Object, mockLog.Object);
            stockController.ModelState.AddModelError("username", "Username or password invalid");

            mockSession[SessionKeyUser] = SessionKeyUser; //logged in
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (BadRequestResult)await stockController.LogIn(newLogin);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }
        [Fact]
        public async Task CheckUsername_Valid()
        {
            //Arrange
            string username = "Luddebassen";
            

            mockRep.Setup(u => u.CheckUsername(username)).ReturnsAsync(true);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            //not dependent on login (dont need session)

            //Act
            var result = (OkResult)await stockController.CheckUsername(username);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            
        }
        [Fact]
        public async Task CheckUsername_Taken()
        {
            //Arrange
            string username = "Luddebassen";
            

            mockRep.Setup(u => u.CheckUsername(username)).ReturnsAsync(false);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            //not dependent on login (dont need session)

            //Act
            var result = (BadRequestResult)await stockController.CheckUsername(username);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            
        }
        [Fact]
        public async Task CreateCompany_LogInNotOK()
        {
            //Arrange
            //no mockRep setup because if user is not logged in the updateUser-function in the repo is not called and will return null

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = _notLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (UnauthorizedResult)await stockController.CreateCompany(It.IsAny<string>());

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            
        }
        [Fact]
        public async Task CreateCompany_LogInOk_DBerror()
        {
            //Arrange
            var serverRespons = new ServerResponse(false, "Server Exception!");
            string name = "Company";

            mockRep.Setup(u => u.CreateCompany(name)).ReturnsAsync(serverRespons);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = SessionKeyUser;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (BadRequestResult)await stockController.CreateCompany(name);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            
        }
        [Fact]
        public async Task CreateCompany_LogInOk_dbOK()
        {
            //Arrange
            var serverRespons = new ServerResponse(true);
            string name = "Company";


            mockRep.Setup(u => u.CreateCompany(name)).ReturnsAsync(serverRespons);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = SessionKeyUser;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (OkResult)await stockController.CreateCompany(name);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            
        }
        [Fact]
        public async Task GetCompanyByName_Found()
        {
            //Arrange
            Company company = new Company(Guid.NewGuid(), "Company", 10, "[0,1,2,3,4,5,6,7,8,9]");


            mockRep.Setup(u => u.GetCompanyByName(company.name)).ReturnsAsync(company);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            //Act
            var result = (OkObjectResult)await stockController.GetCompanyByName(company.name);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(JsonConvert.SerializeObject(result.Value), JsonConvert.SerializeObject(company));
        }
        [Fact]
        public async Task GetCompanyByID_NotFound()
        {
            //Arrange 
            Guid id = Guid.NewGuid(); //random id
            
            mockRep.Setup(u => u.GetCompanyByID(id)).ReturnsAsync((Company)null); //returns null for User-object if User.id is not found

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            //Act 
            var result = (NotFoundResult)await stockController.GetCompanyByName(It.IsAny<string>());

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
            
        }
        [Fact]
        public async Task GetAllCompanies_OK() //Not dependent on log-in
        {
            //Arrange
            //list with companies that is being sent to the user
            var clientCompany1 = new ClientCompany("Company1", 100, "[22,33,100]");
            var clientCompany2 = new ClientCompany("Company2", 100, "[22,33,100]");
            var clientCompany3 = new ClientCompany("Company3", 100, "[22,33,100]");

            var clientCompanylist = new List<ClientCompany>();
            clientCompanylist.Add(clientCompany1);
            clientCompanylist.Add(clientCompany2);
            clientCompanylist.Add(clientCompany3);

            mockRep.Setup(u => u.GetAllClientCompanies()).ReturnsAsync(clientCompanylist);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            // Act
            var result = await stockController.GetAllCompanies() as OkObjectResult;

            // Assert 

            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal<List<ClientCompany>>((List<ClientCompany>)result.Value, clientCompanylist);
        }
        [Fact]
        public async Task DeleteUser_LogInNotOK()
        {
            //Arrange
            //no mockRep setup because if user is not logged in the updateUser-function in the repo is not called and will return null

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = _notLoggedIn; //not logged in
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (UnauthorizedResult)await stockController.DeleteUser();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            
        }
        [Fact]
        public async Task DeleteUser_LogInOK_DBError()
        {
            //Arrange
            ServerResponse serverRespons = new ServerResponse(false, "Server Exception!");

            mockRep.Setup(u => u.DeleteUser(It.IsAny<string>())).ReturnsAsync(serverRespons);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = SessionKeyUser;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (BadRequestResult)await stockController.DeleteUser();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            
        }
        [Fact]
        public async Task DeleteUser_LogInOK()
        {
            //Arrange
            ServerResponse serverRespons = new ServerResponse(true);

            mockRep.Setup(u => u.DeleteUser(It.IsAny<string>())).ReturnsAsync(serverRespons);
            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = SessionKeyUser;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (OkResult)await stockController.DeleteUser();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
 
        }
        [Fact]
        public async Task GetCurrentCompany_SessionNotOK() //checking if session for company is active
        {
            //Arrange
            //mockRep setup not needed because no function is called if company is not active

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyCompany] = _notLoggedIn; //not logged in
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (UnauthorizedResult)await stockController.GetCurrentCompany();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            
        }
        [Fact]
        public async Task GetCurrentCompany_SessionOK_NotFound()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            

            mockRep.Setup(u => u.GetCompanyByID(id)).ReturnsAsync((Company)null);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyCompany] = id.ToString();
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (NotFoundResult)await stockController.GetCurrentCompany();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
            
        }
        [Fact]
        public async Task GetCurrentCompany_ActiveOK_Found()
        {
            //Arrange
            var company1 = new Company(Guid.NewGuid(), "Company1", 100, "[22,33,100]");
            var clientCompany1 = new ClientCompany(company1.name, company1.value, company1.values);

            mockRep.Setup(u => u.GetCompanyByName(company1.name)).ReturnsAsync(company1);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyCompany] = company1.name;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (OkObjectResult)await stockController.GetCurrentCompany();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(JsonConvert.SerializeObject(result.Value), JsonConvert.SerializeObject(clientCompany1));

        }
        [Fact]
        public async Task GetCurrentUser_LogInNotOK()
        {
            //Arrange
            //no mockRep setup because if user is not logged in the updateUser-function in the repo is not called and will return null

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = _notLoggedIn; //not logged in
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (UnauthorizedResult)await stockController.GetCurrentUser();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            
        }
        [Fact]
        public async Task GetCurrentUser_LogInOK_DBError()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            
            mockRep.Setup(u => u.GetUserByID(id)).ReturnsAsync((User)null);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = id.ToString();
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (NotFoundResult)await stockController.GetCurrentUser();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
            
        }
        [Fact]
        public async Task GetCurrentUser_LogInOK_dbOK()
        {
            //Arrange
            User user = new User(Guid.NewGuid(), "Luddebassen", "Luddebassen", "Luddebassen", 100);
            ClientUser clientUser = new ClientUser(user.username, user.balance);

            mockRep.Setup(u => u.GetUserByID(user.id)).ReturnsAsync(user);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = user.id.ToString();
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (OkObjectResult)await stockController.GetCurrentUser();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(JsonConvert.SerializeObject(result.Value), JsonConvert.SerializeObject(clientUser));
        }
        [Fact]
        public async Task GetListOfStocksForUserWithID_LogInNotOK()
        {
            //Arrange
            //no mockRep setup because if user is not logged in the updateUser-function in the repo is not called and will return null

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = _notLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (UnauthorizedResult)await stockController.GetStocksForUser();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            
        }
        [Fact]
        public async Task GetListOfStocksForUserWithID_LogInOK()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            List<ClientStock> newList = new List<ClientStock>();
            mockRep.Setup(u => u.GetStocksWithUserID(id.ToString())).ReturnsAsync(newList);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = SessionKeyUser;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (OkObjectResult)await stockController.GetStocksForUser();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            
        }
        [Fact]
        public async Task GetAllUsersValue()
        {
            //Arrange
            List<ClientStock> userList = new List<ClientStock>();
            mockRep.Setup(k => k.GetAllUsersTotalValue()).ReturnsAsync(userList);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            //Act
            var result = (OkObjectResult)await stockController.GetUsersValue();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal<List<ClientStock>>(userList, (List<ClientStock>)result.Value);
        }
        [Fact]
        public async Task GetUserValueByID_LogInNotOK()
        {
            //Arrange
            //no mockRep setup because if user is not logged in the updateUser-function in the repo is not called and will return null

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = _notLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (UnauthorizedResult)await stockController.GetUserValue();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            
        }
        [Fact]
        public async Task GetUserValueByID_LogInOK_NotFound()
        {
            //Arrange
            Guid id = Guid.NewGuid(); //generating a random id that does not exist for any users
       
            mockRep.Setup(u => u.GetUsersValueByID(id.ToString())).ReturnsAsync((ClientStock)null); //returns null for stockname-object for id is not found

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = id.ToString();
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (NotFoundResult)await stockController.GetUserValue();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
            
        }
        [Fact]
        public async Task GetUserValueByID_LogInOK_Found()
        {
            //Arrange
            var user = new User(Guid.NewGuid(), "Luddebassen", "Luddebassen", "Luddebassen", 100);
            var userStock = new ClientStock("Luddebassen", 22, 100);

            mockRep.Setup(u => u.GetUsersValueByID(user.id.ToString())).ReturnsAsync(userStock); //returns null for stockname-object for id is not found

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = user.id.ToString();
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (OkObjectResult)await stockController.GetUserValue();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(JsonConvert.SerializeObject(result.Value), JsonConvert.SerializeObject(userStock));
        }

        [Fact]
        public async Task BuyStock_LogInNotOK()
        {
            //Arrange
            //no mockRep setup because if user is not logged in the updateUser-function in the repo is not called and will return null
            StringWrapper wrap = new StringWrapper("5");

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = _notLoggedIn;
            mockSession[SessionKeyCompany] = _notLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;


            //Act
            var result = (UnauthorizedResult)await stockController.BuyStock(wrap);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            
        }
        [Fact]
        public async Task BuyStock_LogInOK_dbError()
        {
            //Arrange
            Guid userid = Guid.NewGuid();
            Guid companyid = Guid.NewGuid();
            ServerResponse serverRespons = new ServerResponse(false, "Server Exception!");
            StringWrapper wrap = new StringWrapper("5");

            mockRep.Setup(u => u.TryToBuyStockForUser(userid.ToString(), companyid.ToString(), It.IsAny<int>())).ReturnsAsync(serverRespons);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = userid.ToString();
            mockSession[SessionKeyCompany] = companyid.ToString();
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (BadRequestResult)await stockController.BuyStock(wrap);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            
        }
        [Fact]
        public async Task BuyStock_LogInOK_dbOK()
        {
            //Arrange
            Guid userid = Guid.NewGuid();
            Guid companyid = Guid.NewGuid();
            ServerResponse serverRespons = new ServerResponse(true);
            StringWrapper wrap = new StringWrapper("5");

            mockRep.Setup(u => u.TryToBuyStockForUser(userid.ToString(), companyid.ToString(), It.IsAny<int>())).ReturnsAsync(serverRespons);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = userid.ToString();
            mockSession[SessionKeyCompany] = companyid.ToString();
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (OkResult)await stockController.BuyStock(wrap);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            

        }
        [Fact]
        public async Task BuyStock_InputError()
        {
            //Arrange
            StringWrapper wrap = new StringWrapper("Invalid syntax");
            //no mockRep setup because if user is not logged in the updateUser-function in the repo is not called and will return null

            var stockController = new StockController(mockRep.Object, mockLog.Object);
            //Act
            var result = (BadRequestResult)await stockController.BuyStock(wrap);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);

        }
        [Fact]
        public async Task SellStock_LogInNotOK()
        {
            //Arrange
            //no mockRep setup because if user is not logged in the updateUser-function in the repo is not called and will return null
            StringWrapper wrap = new StringWrapper("5");

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = _notLoggedIn;
            mockSession[SessionKeyCompany] = _notLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;


            //Act
            var result = (UnauthorizedResult)await stockController.SellStock(wrap);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            
        }
        [Fact]
        public async Task SellStock_LogInOK_dbError()
        {
            //Arrange
            Guid userid = Guid.NewGuid();
            Guid companyid = Guid.NewGuid();
            ServerResponse serverRespons = new ServerResponse(false, "Server Exception!");
            StringWrapper wrap = new StringWrapper("5");

            mockRep.Setup(u => u.TryToSellStockForUser(userid.ToString(), companyid.ToString(), It.IsAny<int>())).ReturnsAsync(serverRespons);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = userid.ToString();
            mockSession[SessionKeyCompany] = companyid.ToString();
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (BadRequestResult)await stockController.SellStock(wrap);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            
        }
        [Fact]
        public async Task SellStock_LogInOK_dbOK()
        {
            //Arrange
            Guid userid = Guid.NewGuid();
            Guid companyid = Guid.NewGuid();
            ServerResponse serverRespons = new ServerResponse(true);
            StringWrapper wrap = new StringWrapper("5");

            mockRep.Setup(u => u.TryToSellStockForUser(userid.ToString(), companyid.ToString(), It.IsAny<int>())).ReturnsAsync(serverRespons);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = userid.ToString();
            mockSession[SessionKeyCompany] = companyid.ToString();
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (OkResult)await stockController.SellStock(wrap);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            
        }
        [Fact]
        public async Task SellStock_InputError()
        {
            //Arrange
            StringWrapper wrap = new StringWrapper("Invalid syntax");
            //no mockRep setup because if user is not logged in the updateUser-function in the repo is not called and will return null

            var stockController = new StockController(mockRep.Object, mockLog.Object);
            //Act
            var result = (BadRequestResult)await stockController.SellStock(wrap);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);

        }
        [Fact]
        public async Task GetCurrentStock_LogInNotOK()
        {
            //Arrange
            //no mockRep setup because if user is not logged in the updateUser-function in the repo is not called and will return null
            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = _notLoggedIn;
            mockSession[SessionKeyCompany] = _notLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;


            //Act
            var result = (UnauthorizedResult)await stockController.GetCurrentStock();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            
        }
        [Fact]
        public async Task GetCurrentStock_LogInOK_dbError()
        {
            //Arrange
            Guid userid = Guid.NewGuid();
            Guid companyid = Guid.NewGuid();
            ClientStock clientStock = new ClientStock(null, 0);

            mockRep.Setup(u => u.GetStockWithUserAndCompany(userid.ToString(), companyid.ToString())).ReturnsAsync((Stock)null);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = userid.ToString();
            mockSession[SessionKeyCompany] = companyid.ToString();
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (OkObjectResult)await stockController.GetCurrentStock();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(JsonConvert.SerializeObject(result.Value), JsonConvert.SerializeObject(clientStock));
            
        }
        [Fact]
        public async Task GetCurrentStock_LogInOK_dbOK()
        {
            //Arrange
            Guid userid = Guid.NewGuid();
            Guid companyid = Guid.NewGuid();
            Stock stock = new Stock(Guid.NewGuid(), 1, userid, companyid, "Company1");
            ClientStock clientStock = new ClientStock(stock.companyName, stock.amount);


            mockRep.Setup(u => u.GetStockWithUserAndCompany(userid.ToString(), companyid.ToString())).ReturnsAsync(stock);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = userid.ToString();
            mockSession[SessionKeyCompany] = companyid.ToString();
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (OkObjectResult)await stockController.GetCurrentStock();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(JsonConvert.SerializeObject(result.Value), JsonConvert.SerializeObject(clientStock));
        }
        [Fact]
        public async Task UpdateUserLogInOk()
        {
            //Arrange
            StringWrapper wrap = new StringWrapper("username");
            string username = wrap.value;
            string id = Guid.NewGuid().ToString();
            ServerResponse serverRespons = new ServerResponse(true);

            mockRep.Setup(u => u.UpdateUser(id, username)).ReturnsAsync(serverRespons);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = id;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (OkResult)await stockController.UpdateUser(wrap);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            
        }
        [Fact]
        public async Task UpdateUserLogInNotOk()
        {
            //Arrange
            //no mockRep setup because if user is not logged in the updateUser-function in the repo is not called and will return null
            StringWrapper wrap = new StringWrapper("username");

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = _notLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (UnauthorizedResult)await stockController.UpdateUser(wrap);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            
        }
        [Fact]
        public async Task UpdateUserLogInOk_DBerror()
        {
            //Arrange
            StringWrapper wrap = new StringWrapper("username");
            ServerResponse serverRespons = new ServerResponse(false, "Username is the same!");
            mockRep.Setup(u => u.UpdateUser(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(serverRespons);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = SessionKeyUser;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (BadRequestResult)await stockController.UpdateUser(wrap);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            
        }


    }
}
