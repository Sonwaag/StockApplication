using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockApplication.Code
{
    public class ClientUser
    {
        [RegularExpression(@"[0-9a-zA-ZæøåÆØÅ. \-]{3,12}")]
        public string username { get; set; }
        public float balance { get; set; }
        public ClientUser(string username, float balance)
        {
            this.username = username;
            this.balance = balance;
        }
    }
}
