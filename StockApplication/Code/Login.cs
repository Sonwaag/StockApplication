using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockApplication.Code
{
    public class Login
    {
        [RegularExpression(@"[a-zA-ZæøåÆØÅ. \-]{3,12}")]
        public string username { get; set; }
        [RegularExpression(@"[a-zA-ZæøåÆØÅ. \-]{8,32}")]
        public string password { get; set; }
        public Login(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }
}
