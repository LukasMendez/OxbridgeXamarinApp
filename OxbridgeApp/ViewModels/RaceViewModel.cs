using OxbridgeApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Linq;
using Xamarin.Forms.GoogleMaps;

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
        private int NextCheckPoint { get; set; }
        public Command NorthCommand { get; set; }
        public Command SouthCommand { get; set; }
        public Command EastCommand { get; set; }
        public Command WestCommand { get; set; }

        public RaceViewModel() {
            this.NorthCommand = new Command(
                (object message) =>
                {
                    Latitude += 0.0002;
                    MoveMyPosPin();
                    UpdateCheckPoints();
                    Console.WriteLine("*North*");
                },
                (object message) => { Console.WriteLine("*CanNorth*"); return true; });
            this.SouthCommand = new Command(
                (object message) =>
                {
                    Latitude -= 0.0002;
                    MoveMyPosPin();
                    UpdateCheckPoints();
                    Console.WriteLine("*South*");
                },
                (object message) => { Console.WriteLine("*CanSouth*"); return true; });
            this.EastCommand = new Command(
                (object message) =>
                {
                    Longitude += 0.0002;
                    MoveMyPosPin();
                    UpdateCheckPoints();
                    Console.WriteLine("*East*");
                },
                (object message) => { Console.WriteLine("*CanEast*"); return true; });
            this.WestCommand = new Command(
                (object message) =>
                {
                    Longitude -= 0.0002;
                    MoveMyPosPin();
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

            MoveMyPosPin();
            LoadCheckPoints();
            UpdateCheckPoints();



        }

        private void MoveMyPosPin() {
            //pin position HARDCODED TEMPORARY! (should get from device GPS)
            MyPosPin = new Pin
            {
                Label = "Me",
                Address = "My Boat",
                Type = PinType.Place,
                Position = new Position(Latitude,Longitude)
            };
            Map.Pins.Clear();
            Map.Pins.Add(MyPosPin);
        }

        private void LoadCheckPoints() {
            //checkpoints HARDCODED TEMPORARY! (should get from server request)
            Circle firstCheckpoint = new Circle
            {
                Tag = 1,
                Center = new Position(54.914359, 9.780739),
                Radius = new Distance(50),
                StrokeColor = Color.FromRgba(255, 51, 51, 88), //red
                StrokeWidth = 3,
                FillColor = Color.FromRgba(255, 51, 51, 50)
            };
            Map.Circles.Add(firstCheckpoint);
            CheckPoints.Add(firstCheckpoint);
            Circle secondCheckpoint = new Circle
            {
                Tag = 2,
                Center = new Position(54.916548, 9.776104),
                Radius = new Distance(50),
                StrokeColor = Color.FromRgba(255, 51, 51, 88),
                StrokeWidth = 3,
                FillColor = Color.FromRgba(255, 51, 51, 50)
            };
            Map.Circles.Add(secondCheckpoint);
            CheckPoints.Add(secondCheckpoint);
            Circle thirdCheckpoint = new Circle
            {
                Tag = 3,
                Center = new Position(54.916326, 9.769967),
                Radius = new Distance(50),
                StrokeColor = Color.FromRgba(255, 51, 51, 88),
                StrokeWidth = 3,
                FillColor = Color.FromRgba(255, 51, 51, 50)
            };
            Map.Circles.Add(thirdCheckpoint);
            CheckPoints.Add(thirdCheckpoint);
            Circle fourthCheckpoint = new Circle
            {
                Tag = 4,
                Center = new Position(54.918386, 9.765483),
                Radius = new Distance(50),
                StrokeColor = Color.FromRgba(255, 51, 51, 88),
                StrokeWidth = 3,
                FillColor = Color.FromRgba(255, 51, 51, 50)
            };
            Map.Circles.Add(fourthCheckpoint);
            CheckPoints.Add(fourthCheckpoint);
            Circle fifthCheckpoint = new Circle
            {
                Tag = 5,
                Center = new Position(54.921617, 9.766491),
                Radius = new Distance(50),
                StrokeColor = Color.FromRgba(255, 51, 51, 88), 
                StrokeWidth = 3,
                FillColor = Color.FromRgba(255, 51, 51, 50)
            };
            Map.Circles.Add(fifthCheckpoint);
            CheckPoints.Add(fifthCheckpoint);
        }

        private void UpdateCheckPoints() {
            foreach (var item in CheckPoints) {
                if (item.Tag.ToString().Equals(NextCheckPoint.ToString())) {
                    item.StrokeColor = Color.FromRgba(51, 61, 255, 88); //blue
                    item.FillColor = Color.FromRgba(51, 61, 255, 50);
                    if (!item.Tag.Equals("done")) {
                        if (item.Center.Latitude - MyPosPin.Position.Latitude >= -0.0005 &&
                        item.Center.Latitude - MyPosPin.Position.Latitude <= 0.0005 &&
                        item.Center.Longitude - MyPosPin.Position.Longitude >= -0.0005 &&
                        item.Center.Longitude - MyPosPin.Position.Longitude <= 0.0005) {
                            item.StrokeColor = Color.FromRgba(71, 255, 51, 88); //green
                            item.FillColor = Color.FromRgba(71, 255, 51, 50);
                            item.Tag = "done";
                            NextCheckPoint += 1;
                        }
                    }
                }
                
                
            }
        }

        //private async void currentButton_Clicked(object sender, EventArgs e) {
        //    var request = new GeolocationRequest(GeolocationAccuracy.Medium);
        //    var location = await Geolocation.GetLocationAsync(request);

        //    Position currentPosition = new Position(location.Latitude, location.Longitude);
        //    MyMap.MoveToRegion(
        //        MapSpan.FromCenterAndRadius(
        //        currentPosition, Distance.FromKilometers(1)));

        //    Pin pin = new Pin
        //    {
        //        Label = "Home",
        //        Address = "The city with a boardwalk",
        //        Type = PinType.Place,
        //        Position = currentPosition
        //    };

        //    MyMap.Pins.Add(pin);
        //}

    }
}
