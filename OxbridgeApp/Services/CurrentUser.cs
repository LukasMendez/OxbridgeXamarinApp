using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace OxbridgeApp.Services
{
    public static class CurrentUser
    {

        // Login status
        public static bool IsLoggedIn = false;

        // Keys used to store consistent data in Preferences

        public static string TokenKey { get; } = "tokenKey";

        public static string FullName { get; } = "fullNameKey";

        public static string Username { get; } = "usernameKey";

        public static string IsAdmin { get; } = "isAdminKey";

        public static string IsTeamLeader { get; } = "isTeamLeaderKey";

        public static string Team { get; } = "teamKey";


        /// <summary>
        /// Easy method for configure the current user, that is logged into the system. 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="fullname"></param>
        /// <param name="username"></param>
        /// <param name="isAdmin"></param>
        /// <param name="isTeamLeader"></param>
        /// <param name="team"></param>
        public static void SetCurrentUser(string token, string fullname, string username, bool isAdmin, bool isTeamLeader, string team)
        {
            Preferences.Set(CurrentUser.TokenKey, token);
            Preferences.Set(CurrentUser.FullName, fullname);
            Preferences.Set(CurrentUser.Username, username);
            Preferences.Set(CurrentUser.IsAdmin, isAdmin);
            Preferences.Set(CurrentUser.IsTeamLeader, isTeamLeader);
            Preferences.Set(CurrentUser.Team, team);
        }

        public static void RemoveCurrentUser()
        {
            Preferences.Set(CurrentUser.TokenKey, "null");
            Preferences.Set(CurrentUser.FullName, "null");
            Preferences.Set(CurrentUser.Username, "null");
            Preferences.Set(CurrentUser.IsAdmin, false);
            Preferences.Set(CurrentUser.IsTeamLeader, false);
            Preferences.Set(CurrentUser.Team, "null");
        }

    }
}
