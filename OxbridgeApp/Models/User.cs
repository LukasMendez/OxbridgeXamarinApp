using System;
using System.Collections.Generic;
using System.Text;

namespace OxbridgeApp.Models
{
    public class User
    {
        string FullName { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        bool IsTeamLeader { get; set; }
        bool IsAdmin { get; set; }
        string Team { get; set; }

        public User(string fullName, string username, string password, bool isTeamLeader, bool isAdmin, string team) {
            this.FullName = fullName;
            this.Username = username;
            this.Password = password;
            this.IsTeamLeader = isTeamLeader;
            this.IsAdmin = isAdmin;
            this.Team = team;
        }
    }
}
