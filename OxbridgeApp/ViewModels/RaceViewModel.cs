﻿using OxbridgeApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Linq;
using Xamarin.Forms.GoogleMaps;
using Newtonsoft.Json;
using OxbridgeApp.Models;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Runtime.ExceptionServices;
using System.Windows.Input;

namespace OxbridgeApp.ViewModels
{
    public class RaceViewModel : BaseViewModel
    {
        public Map Map { get; private set; }
        private double Latitude { get; set; }
        private double Longitude { get; set; }
        public Position MyPosition { get; set; }
        private Pin MyPosPin { get; set; }
        private List<Circle> CheckPoints { get; set; }
        private Dictionary<string, Position> Participants { get; set; }
        private int NextCheckPoint { get; set; }
        private string UserName { get; set; }
        public Command NorthCommand { get; set; }
        public Command SouthCommand { get; set; }
        public Command EastCommand { get; set; }
        public Command WestCommand { get; set; }
        private ICommand appearingCommand { get; set; }
        public ICommand AppearingCommand {
            get {
                return appearingCommand ?? (appearingCommand = new Command(this.Appearing));
            }
        }
        private ICommand disappearingCommand { get; set; }
        public ICommand DisappearingCommand {
            get {
                return disappearingCommand ?? (disappearingCommand = new Command(this.Disappearing));
            }
        }



        // Boat image
        private BitmapDescriptor boatPin = BitmapDescriptorFactory.FromBundle("boatSmall.png");


        public RaceViewModel() {
            UserName = "Robert";
            Participants = new Dictionary<string, Position>();
            App.WebConnection.NewCoordReceived += ReceivedCoord;
            App.WebConnection.ConnectSocket();

            //AppearingCommand = new Command(Appearing);
            //DisappearingCommand = new Command(Disappearing);


            this.NorthCommand = new Command(
                (object message) =>
                {
                    Latitude += 0.0002;
                    UpdateCheckPoints();
                    Console.WriteLine("*North*");
                },
                (object message) => { Console.WriteLine("*CanNorth*"); return true; });
            this.SouthCommand = new Command(
                (object message) =>
                {
                    Latitude -= 0.0002;
                    UpdateCheckPoints();
                    Console.WriteLine("*South*");
                },
                (object message) => { Console.WriteLine("*CanSouth*"); return true; });
            this.EastCommand = new Command(
                (object message) =>
                {
                   Longitude += 0.0002;
                    UpdateCheckPoints();
                    Console.WriteLine("*East*");
                },
                (object message) => { Console.WriteLine("*CanEast*"); return true; });
            this.WestCommand = new Command(
                (object message) =>
                {
                    Longitude -= 0.0002;
                    UpdateCheckPoints();
                    Console.WriteLine("*West*");
                },
                (object message) => { Console.WriteLine("*CanWest*"); return true; });


            //initial position
            Latitude = 54.912163;
            Longitude = 9.782445;
            MyPosition = new Position(Latitude, Longitude);
            Map = new Map();
            Map.MapType = MapType.Hybrid;
            CheckPoints = new List<Circle>();
            NextCheckPoint = 1;



            //move to position (should probably move to the first checkpoint when entering map)
            Map.MoveToRegion(
                MapSpan.FromCenterAndRadius(
                MyPosition, Distance.FromKilometers(1)));

            MyPosPin = new Pin
            {
                Label = "Robert",
                Type = PinType.Place,
                Position = new Position(Latitude, Longitude),
                Icon = boatPin
            };
            Map.Pins.Add(MyPosPin);

            StartCoordinateTimer();
            LoadCheckPoints();
            UpdateCheckPoints();



        }

        private void Appearing() {
            Console.WriteLine();
        }

        private void Disappearing() {
            Console.WriteLine();
        }

        public void StartCoordinateTimer() {
            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                //UpdatePositionFromGPS(); //comment out when testing
                UpdateAllPins();
                return true;
            });
        }

        bool first = true;
        private async void UpdatePositionFromGPS() {
            var request = new Xamarin.Essentials.GeolocationRequest(Xamarin.Essentials.GeolocationAccuracy.Medium);
            var location = await Xamarin.Essentials.Geolocation.GetLocationAsync(request);

            Latitude = location.Latitude;
            Longitude = location.Longitude;

            if (first) {
                Position currentPosition = new Position(location.Latitude, location.Longitude);
                Map.MoveToRegion(
                    MapSpan.FromCenterAndRadius(
                    currentPosition, Distance.FromKilometers(1)));
                first = false;
            }
        }

        public void UpdateAllPins() {
            App.WebConnection.SendCoordinate(new Coordinate(UserName, new Position(Latitude,Longitude)));

            try {
                Map.Pins.Clear();
                foreach (var item in Participants) {
                    if (!item.Key.Equals(UserName)) { //if opponent
                        Pin opponentPin = new Pin
                        {
                            Label = item.Key,
                            Type = PinType.Place,
                            Position = item.Value,
                            Transparency = 0.5f,
                            Icon = boatPin
                        };
                        Map.Pins.Add(opponentPin);
                    } else {
                        MyPosPin = new Pin
                        {
                            Label = item.Key,
                            Type = PinType.Place,
                            Position = item.Value,
                            Icon = boatPin
                        };
                        Map.Pins.Add(MyPosPin);
                    }
                }
            }
            catch (Exception ex) { //sometimes catching a "Collection was modified; enumeration operation may not execute." exception
                Console.WriteLine("*** " + ex.Message);
            }
        }

        private void LoadCheckPoints() {
            var viewModel = ServiceContainer.Resolve<MainMenuViewModel>();
            var selectedRace = viewModel.SelectedRace;
            
            for (int i = 0; i < selectedRace.CheckPoints.Count; i++) {
                Circle checkPoint = new Circle
                {
                    Tag = i+1,
                    Center = new Position(selectedRace.CheckPoints[i].Latitude, selectedRace.CheckPoints[i].Longitude),
                    Radius = new Distance(50),
                    StrokeColor = Color.FromRgba(255, 51, 51, 88), //red
                    StrokeWidth = 3,
                    FillColor = Color.FromRgba(255, 51, 51, 50)
                };
                Map.Circles.Add(checkPoint);
                CheckPoints.Add(checkPoint);
            }
            
            //checkpoints HARDCODED TEMPORARY! (should get from server request)
            //Circle firstCheckpoint = new Circle
            //{
            //    Tag = 1,
            //    Center = new Position(54.914359, 9.780739),
            //    Radius = new Distance(50),
            //    StrokeColor = Color.FromRgba(255, 51, 51, 88), //red
            //    StrokeWidth = 3,
            //    FillColor = Color.FromRgba(255, 51, 51, 50)
            //};
            //Map.Circles.Add(firstCheckpoint);
            //CheckPoints.Add(firstCheckpoint);
            //Circle secondCheckpoint = new Circle
            //{
            //    Tag = 2,
            //    Center = new Position(54.916548, 9.776104),
            //    Radius = new Distance(50),
            //    StrokeColor = Color.FromRgba(255, 51, 51, 88),
            //    StrokeWidth = 3,
            //    FillColor = Color.FromRgba(255, 51, 51, 50)
            //};
            //Map.Circles.Add(secondCheckpoint);
            //CheckPoints.Add(secondCheckpoint);
            //Circle thirdCheckpoint = new Circle
            //{
            //    Tag = 3,
            //    Center = new Position(54.916326, 9.769967),
            //    Radius = new Distance(50),
            //    StrokeColor = Color.FromRgba(255, 51, 51, 88),
            //    StrokeWidth = 3,
            //    FillColor = Color.FromRgba(255, 51, 51, 50)
            //};
            //Map.Circles.Add(thirdCheckpoint);
            //CheckPoints.Add(thirdCheckpoint);
            //Circle fourthCheckpoint = new Circle
            //{
            //    Tag = 4,
            //    Center = new Position(54.918386, 9.765483),
            //    Radius = new Distance(50),
            //    StrokeColor = Color.FromRgba(255, 51, 51, 88),
            //    StrokeWidth = 3,
            //    FillColor = Color.FromRgba(255, 51, 51, 50)
            //};
            //Map.Circles.Add(fourthCheckpoint);
            //CheckPoints.Add(fourthCheckpoint);
            //Circle fifthCheckpoint = new Circle
            //{
            //    Tag = 5,
            //    Center = new Position(54.921617, 9.766491),
            //    Radius = new Distance(50),
            //    StrokeColor = Color.FromRgba(255, 51, 51, 88), 
            //    StrokeWidth = 3,
            //    FillColor = Color.FromRgba(255, 51, 51, 50)
            //};
            //Map.Circles.Add(fifthCheckpoint);
            //CheckPoints.Add(fifthCheckpoint);
        }

        private void UpdateCheckPoints() {
            foreach (var item in CheckPoints) {
                if (item.Tag.ToString().Equals(NextCheckPoint.ToString())) {
                    item.StrokeColor = Color.FromRgba(51, 61, 255, 88); //blue
                    item.FillColor = Color.FromRgba(51, 61, 255, 50);
                    if (!item.Tag.Equals("done")) {
                        if (item.Center.Latitude - MyPosPin.Position.Latitude >= -0.0006 &&
                        item.Center.Latitude - MyPosPin.Position.Latitude <= 0.0006 &&
                        item.Center.Longitude - MyPosPin.Position.Longitude >= -0.0006 &&
                        item.Center.Longitude - MyPosPin.Position.Longitude <= 0.0006) {
                            item.StrokeColor = Color.FromRgba(71, 255, 51, 88); //green
                            item.FillColor = Color.FromRgba(71, 255, 51, 50);
                            item.Tag = "done";
                            NextCheckPoint += 1;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// fired by event every time any client emits its coordinate
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="message"></param>
        private void ReceivedCoord(object obj, string message) {
            Coordinate coordinate = JsonConvert.DeserializeObject<Coordinate>(message);
            Participants[coordinate.UserName] = coordinate.Position; //adding or updating this opponent
        }
    }
}
