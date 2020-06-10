using System;
using System.Collections.Generic;
using System.Text;

namespace OxbridgeApp.Models
{
    public class User
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsTeamLeader { get; set; }
        public bool IsAdmin { get; set; }
        public string Team { get; set; }

        public User() {

        }
    }
}
