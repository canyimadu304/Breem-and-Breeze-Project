using Breeze_Beam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breeze_Beam.Models
{
    public class User : Person
    {
        public string Username { get; set; }
        public string Email { get; set; }

        public User(string u, string e)
        {
            this.Username = u;
            this.Email = e;
        }
    }
}
