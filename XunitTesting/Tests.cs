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
            var race = new OxbridgeApp.ViewModels.RaceViewModel();

            // act
            race.LeaderboardList.Clear();
            App.WebConnection.SendMessage(checkpoint); //telling the server that we completed this checkpoint
            string actual = race.LeaderboardList[0];

            // assert
            Assert.Equal(team, actual);
        }

        //[Fact]
        //public void Test1() {
        //    // arrange
        //    string expected = "123";
        //    string actual = "123";
        //    // assert
        //    Assert.Equal(expected, actual);
        //}

        ///// <summary>
        ///// Test if a propertychanged event is fired when a car is added
        ///// </summary>
        //[Fact]
        //public void TestCarCollectionPropertyChanged() {
        //    // arrange
        //    bool invoked = false;
        //    var car = new IcomIvalEx.Model.Car() { Brand = "Ford", Price = 100, Model = "Probe" };
        //    var carDealerVM = new IcomIvalEx.ViewModel.CarDealer();

        //    carDealerVM.PropertyChanged += (sender, e) =>
        //    {
        //        if (e.PropertyName.Equals("InputCar")) invoked = true;
        //    };

        //    // act
        //    carDealerVM.InputCar = car;
        //    carDealerVM.AddCar();

        //    //    var order = await orderService.GetOrderAsync(1, GlobalSetting.Instance.AuthToken);
        //    //await orderViewModel.InitializeAsync(order);

        //    // assert
        //    Assert.True(invoked);
        //}

        ///// <summary>
        ///// Test the AddCarCanCMD Command
        ///// It should only be possible to add up to two cars
        ///// </summary>
        ///// <param name="noOfCarsToAdd"></param>
        ///// <param name="result"></param>
        //[Theory]
        //[InlineData(0, true)]
        //[InlineData(1, true)]
        //[InlineData(3, false)]
        //public void CanAddCarCMD(int noOfCarsToAdd, bool result) {
        //    // arrange
        //    var car = new IcomIvalEx.Model.Car() { Brand = "Ford", Price = 100, Model = "Probe" };
        //    var carDealerVM = new IcomIvalEx.ViewModel.CarDealer();
        //    carDealerVM.InputCar = car;

        //    // act
        //    for (int i = 0; i < noOfCarsToAdd; i++) {
        //        carDealerVM.AddCar();
        //    }

        //    bool canExecuteVal = (carDealerVM.AddCarCanCMD as Command).CanExecute(null);

        //    // assert
        //    Assert.Equal(result, canExecuteVal);
    }
    }
}