using Microsoft.AspNetCore.Mvc;
using StockApplication.Code.Handlers;
using System;
using StockApplication.Code.DAL;
using Microsoft.EntityFrameworkCore;
using StockApplication.Code;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace StockApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly IStockRepository _db;
        private readonly IHostedService listenerService;
        private readonly ILogger _log;

        private readonly string RESPONSE_userNotLoggedIn = "You have to be logged in to perform this action!";
        private readonly string RESPONSE_companyNotActive = "No company is currently active!";
        private readonly string RESPONSE_userNotFound = "User could not be found!";
        private readonly string RESPONSE_companyNotFound = "Company could not be found!";
        private readonly string RESPONSE_stockNotFound = "Stock could not be found!";
        private readonly string RESPONSE_usernameInvalid = "Username was invalid or already taken!";
        public StockController(IStockRepository db, ILogger<StockController> log)
        {
            _db = db;
            _log = log;
        }
        //returns a user by its id || should never be used as it takes in an id
        public async Task<ActionResult> GetUserByID(string id)
        {
            User user = await _db.GetUserByID(Guid.Parse(id));
            if (user == null)
            {
                _log.LogInformation(RESPONSE_userNotFound);
                return NotFound();
            }
            return Ok(new ClientUser(user.id, user.username, user.balance));
        }

        //returns a user by a given username
        public async Task<ActionResult> GetUserByUsername(string username)
        {
            User user = await _db.GetUserByUsername(username);
            if (user == null)
            {
                _log.LogInformation(RESPONSE_userNotFound);
                return NotFound();
            }
            string id = HttpContext.Session.GetString(SessionKeyUser); //get user-id from session key
            if (string.IsNullOrEmpty(id) && id != user.id.ToString()) //checking if id from sesion key is valid and matches the user.id
            {
                _log.LogInformation(RESPONSE_userNotLoggedIn);
                return Unauthorized();
            }

            //never pass id to client
            return Ok(new ClientUser(user.id, user.username, user.balance));
        }

        //returns all registered users with their usernames and passwords
        public async Task<ActionResult> GetAllUsers()
        {
            List<ClientUser> allUsers = await _db.GetAllClientUsers();
            return Ok(allUsers);
        }

        //update a user, does not work for admin
        [HttpPut("updateUser")]
        public async Task<ActionResult> UpdateUser(string username)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeyUser)))
            {
                _log.LogInformation(RESPONSE_userNotLoggedIn);
                return Unauthorized();
            }
            string id = HttpContext.Session.GetString(SessionKeyUser);
            _log.LogInformation(id);
            ServerResponse response = await _db.UpdateUser(id, username);
            bool updated = response.Status;

            if (!updated)
            {
                _log.LogInformation(response.Response);
                return BadRequest();
            }
            return Ok();
        }

        //creates a new user with a given name
        [HttpPost("createUser")]
        public async Task<ActionResult> CreateUser(Login login)
        {
            ServerResponse response = await _db.CreateUser(login.username, login.password);
            bool created = response.Status;
            if (!created)
            {
                _log.LogInformation(response.Response);
                return BadRequest();
            }
            User user = await _db.GetUserByUsername(login.username);
            _log.LogInformation(user.id.ToString());
            if (user == null)
            {
                _log.LogInformation(response.Response);
                return BadRequest();
            }
            HttpContext.Session.SetString(SessionKeyUser, user.id.ToString());

            return Ok();
        }

        //checks if username is taken | WIP when creating user and changing name | client side only
        public async Task<ActionResult> CheckUsername(string username)
        {
            bool checkName = await _db.CheckUsername(username);

            if (!checkName)
            {
                _log.LogInformation(RESPONSE_usernameInvalid);
                return BadRequest();
            }
            return Ok();
        }

        //creates a new company
        [HttpPost("{name}")]
        public async Task<ActionResult> CreateCompany(string name)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeyUser)))
            {
                _log.LogInformation(RESPONSE_userNotLoggedIn);
                return Unauthorized();
            }

            ServerResponse response = await _db.CreateCompany(name);
            bool created = response.Status;

            if (!created)
            {
                _log.LogInformation(response.Response);
                return BadRequest();
            }
            return Ok();
        }

        //gets a single company by its id
        public async Task<ActionResult> GetCompanyByName(string name)
        {
            Company company = await _db.GetCompanyByName(name);
            if(company == null)
            {
                _log.LogInformation(RESPONSE_companyNotFound);
                return NotFound();
            }
            return Ok(company);
        }

        //returns a list of all companies registeres
        [HttpGet]
        public async Task<ActionResult> GetAllCompanies()
        {
            List<Company> companyList = await _db.GetAllCompanies();
            
            return Ok(companyList);
        }

        //deletets the user in the session. cannot delete admin
        [HttpDelete("deleteUser")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            
            if (string.IsNullOrEmpty(id) && (HttpContext.Session.GetString(SessionKeyUser) == id))
            {
                _log.LogInformation(RESPONSE_userNotLoggedIn);
                return Unauthorized();
            }

            ServerResponse response = await _db.DeleteUser(id);
            if (!response.Status)
            {
                _log.LogInformation(response.Response);
                return BadRequest(); //Check
            }
            HttpContext.Session.SetString(SessionKeyUser, "");
            return Ok();
        }

        public async Task<bool> DeleteCompany(string id) //not yet implemented
        {
            return false;
            //return await _db.DeleteCompany(id);
        }

        private const string SessionKeyCompany = "_currentCompany";
        //sets a new company in session
        
        [HttpGet("SetCurrentCompany")]
        public void SetCurrentCompany(string id)
        {
            HttpContext.Session.SetString(SessionKeyCompany, id);
        }

        //gets the company id saved in session    
        public string GetCurrentCompanyID()
        {
            return HttpContext.Session.GetString(SessionKeyCompany);
        }

        //gets company linked to id in session
        [HttpGet("company/current")]
        public async Task<ActionResult> GetCurrentCompany()
        {
            string id = HttpContext.Session.GetString(SessionKeyCompany); //get current company id
            if (string.IsNullOrEmpty(id))
            {
                _log.LogInformation(RESPONSE_companyNotActive);
                return Unauthorized();
            }

            Company company = await _db.GetCompanyByID(Guid.Parse(id));
            if(company == null)
            {
                _log.LogInformation(RESPONSE_companyNotFound);
                return NotFound();
            }
            return Ok(new ClientCompany(company.name, company.value, company.values));
        }

        //clears 
        public void RemoveCurrentCompany()
        {
            HttpContext.Session.SetString(SessionKeyCompany, "");
        }
        private const string SessionKeyUser = "_currentUser";
        
        //sets a new user to the session
        private bool SetCurrentUser(string id)
        {
            HttpContext.Session.SetString(SessionKeyUser, id);
            return true;
        }


        //returns current user
        [HttpGet("getCurrentUser")]
        public async Task<ActionResult> GetCurrentUser()
        {
            string id = HttpContext.Session.GetString(SessionKeyUser);
            if (string.IsNullOrEmpty(id))
            {
                _log.LogInformation(RESPONSE_userNotLoggedIn);
                return Unauthorized();
            }

            User user = await _db.GetUserByID(Guid.Parse(id));
            if(user == null)
            {
                _log.LogInformation(RESPONSE_userNotFound);
                return NotFound();
            }
            return Ok(new ClientUser(user.id, user.username, user.balance));
        }

        
        private const string _loggedIn = "loggedIn";
        private const string _notLoggedIn = "";

        

       

        public async Task<ActionResult> LogIn(Login login)
        {
            ServerResponse response = await _db.LogIn(login.username, login.password);
            if (!response.Status)
            {
                _log.LogInformation(response.Response);
                return BadRequest();
            }
            User user = await _db.GetUserByUsername(login.username);
            if(user == null)
            {
                _log.LogInformation(RESPONSE_userNotFound);
                return NotFound();
            }

            HttpContext.Session.SetString(SessionKeyUser, user.id.ToString());
            return Ok();
        }
        [HttpGet("logOut")]
        public void LogOut()
        {
            HttpContext.Session.SetString(SessionKeyUser, "");
        }

        //returns list of stocks certain user own
        [HttpGet("getStocksForUser/{id}")]
        public async Task<ActionResult> GetStocksForUser(string id)
        {
            string userid = HttpContext.Session.GetString(SessionKeyUser);
            if (string.IsNullOrEmpty(userid) && userid != id)
            {
                _log.LogInformation(RESPONSE_userNotLoggedIn);
                return Unauthorized();
            }
         
            return Ok(await _db.GetStocksWithUserID(userid));
        }

        //returns list with total value for every user
        [HttpGet("getUsersValue")]
        public async Task<ActionResult> GetUsersValue() 
        {
            List<ClientStock> stocks = await _db.GetAllUsersTotalValue();
            return Ok(stocks);
        }

        //get ClientStock object with specific user's value
        [HttpGet("getUsersValueByID")]
        public async Task<ActionResult> GetUsersValueByID(string id)
        {
            string userid = HttpContext.Session.GetString(SessionKeyUser);
            if (string.IsNullOrEmpty(userid) && userid != id)
            {
                _log.LogInformation(RESPONSE_userNotLoggedIn);
                return Unauthorized();
            }
            ClientStock stockName = await _db.GetUsersValueByID(id);
            if (stockName == null)
            {
                _log.LogInformation(RESPONSE_userNotFound);
                return NotFound();
            }
            return Ok(stockName);
        }

       
        

        //tries to buy stock for user and company in session
        [HttpGet("buyStock")]
        public async Task<ActionResult> BuyStock(int amount)
        {
            string userid = HttpContext.Session.GetString(SessionKeyUser);
            string companyid = HttpContext.Session.GetString(SessionKeyCompany);
            if (string.IsNullOrEmpty(userid) && string.IsNullOrEmpty(companyid))
            {
                _log.LogInformation("You have to be logged in to perform this action or No company is currently active");
                return Unauthorized();
            }
            ServerResponse response = await _db.TryToBuyStockForUser(userid, companyid, amount);
            if(!response.Status == true)
            {
                _log.LogInformation(response.Response);
                return BadRequest();
            }
            return Ok();
        }

        //tries to sell stock for user and company in session
        [HttpGet("sellStock")]
        public async Task<ActionResult> SellStock(int amount)
        {
            string userid = HttpContext.Session.GetString(SessionKeyUser);
            string companyid = HttpContext.Session.GetString(SessionKeyCompany);
            if (string.IsNullOrEmpty(userid) && string.IsNullOrEmpty(companyid))
            {
                _log.LogInformation("You have to be logged in to perform this action or No company is currently active");
                return Unauthorized();
            }
            ServerResponse response = await _db.TryToSellStockForUser(userid, companyid, amount);
            if (!response.Status == true)
            {
                _log.LogInformation(response.Response);
                return BadRequest();
            }
            return Ok();
            
        }

        //gets the current stock from user and company in session
        [HttpGet("GetCurrentStock")]
        public async Task<ActionResult> GetCurrentStock()
        {
            string userid = HttpContext.Session.GetString(SessionKeyUser);
            string companyid = HttpContext.Session.GetString(SessionKeyCompany);
            if (string.IsNullOrEmpty(userid) && string.IsNullOrEmpty(companyid))
            {
                _log.LogInformation("You have to be logged in to perform this action or No company is currently active");
                return Unauthorized();
            }
            Stock stock = await _db.GetStockWithUserAndCompany(userid, companyid);
            if(stock == null)
            {
                _log.LogInformation(RESPONSE_stockNotFound);
                return Ok(new ClientStock(null, 0));
            }
            return Ok(new ClientStock(stock.companyName, stock.amount));
        }





    }
}
