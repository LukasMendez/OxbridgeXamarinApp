using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using OxbridgeApp;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using OxbridgeApp.Models;
using OxbridgeApp.Services;
using Xamarin.Essentials;

namespace XunitTesting
{
    public class Tests
    {
        /// <summary>
        /// Test if a team is actually added to the LeaderboardList collection
        /// </summary>
        [Fact]
        public void CrossCheckpointToAddTeamToLeaderboard() {
            // arrange
            string team = "John's Team"; //the team we expect to be added to the leaderboard 
            Circle circle = new Circle { Tag = 1 };
            Preferences.Set(CurrentUser.Team, team);
            Checkpoint checkpoint = new Checkpoint("checkpoint", team, circle.Tag);
            var raceVM = new OxbridgeApp.ViewModels.RaceViewModel();

            // act
            raceVM.LeaderboardList.Clear();
            App.WebConnection.SendMessage(checkpoint); //telling the server that we completed this checkpoint
            string actual = raceVM.LeaderboardList[0];

            // assert
            Assert.Equal(team, actual);
        }

        /// <summary>
        /// Test if a propertychanged event is fired when a team is added
        /// </summary>
        [Fact]
        public void TestLeaderboardListCollectionPropertyChanged() {
            // arrange
            bool invoked = false;
            string team = "John's Team";
            var raceVM = new OxbridgeApp.ViewModels.RaceViewModel();

            raceVM.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName.Equals("LeaderboardList")) invoked = true;
            };

            // act
            raceVM.LeaderboardList.Add(team);

            // assert
            Assert.True(invoked);
        }

        /// <summary>
        /// Test the login method
        /// It should only be possible to login with a valid username and password
        /// </summary>
        [Theory]
        [InlineData("Xunit", "123", true)]
        [InlineData("Xunit", "1234", false)]
        [InlineData("Xunit2", "123", false)]
        public async void CanLogin(string username, string password, bool result) {
            // arrange
            var loginVM = new OxbridgeApp.ViewModels.LoginViewModel();
            loginVM.Username = username;
            loginVM.Password = password;
            // act
            bool loggedIn = await App.WebConnection.Login(loginVM.Username, loginVM.Password);
            // assert
            Assert.Equal(result, loggedIn);
        }
    }
}