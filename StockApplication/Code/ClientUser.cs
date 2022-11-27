using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockApplication.Code
{
    public class ClientUser
    {
        public Guid id { get; set; }
        [RegularExpression(@"[a-zA-ZæøåÆØÅ. \-]{2,20}")]
        public string username { get; set; }
        public float balance { get; set; }
        public ClientUser(Guid id, string username, float balance)
        {
            this.id = id;
            this.username = username;
            this.balance = balance;
        }
    }
}
