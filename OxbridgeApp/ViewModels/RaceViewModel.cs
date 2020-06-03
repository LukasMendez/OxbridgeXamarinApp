using OxbridgeApp.Services;
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
using Xamarin.Essentials;
using Map = Xamarin.Forms.GoogleMaps.Map;

namespace OxbridgeApp.ViewModels
{
    public class RaceViewModel : BaseViewModel
    {
        private Map map;
        public Map MyMap {
            get { return map; }
            set { map = value;
                this.OnPropertyChanged();
            }
        }
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
            Participants = new Dictionary<string, Position>();
            this.MyMap = new Map();
            this.MyMap.MapType = MapType.Hybrid;
            UserName = Preferences.Get(CurrentUser.Username, null);
            App.WebConnection.NewCoordReceived += ReceivedCoord;
            App.WebConnection.StartRaceReceived += StartRace;
            App.WebConnection.ConnectSocket();

            //initial position
            Latitude = 54.912163;
            Longitude = 9.782445;
            MyPosition = new Position(Latitude, Longitude);

            CheckPoints = new List<Circle>();
            NextCheckPoint = 1;

            //move to testing position 
            MyMap.MoveToRegion(
                MapSpan.FromCenterAndRadius(
                MyPosition, Distance.FromKilometers(1)));

            MyPosPin = new Pin
            {
                Label = "initial",
                Type = PinType.Place,
                Position = new Position(Latitude, Longitude),
                Icon = boatPin
            };
            MyMap.Pins.Add(MyPosPin);

            this.NorthCommand = new Command(
                (object message) =>
                {
                    Latitude += 0.0002;
                    UpdateCheckPoints();
                    Console.WriteLine("*North*");
                    App.WebConnection.SendHiTest();
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
        }

        private void Appearing() {

            //this.MyMap = new Map();
            //this.MyMap.MapType = MapType.Hybrid;
            //runTimer = true; //should be set from callback from emit.startRace from server
            LoadCheckPoints();
            StartCoordinateTimer();
            UpdateCheckPoints();
        }

        private void Disappearing() {
            //App.WebConnection.DisconnectSocket();
            runTimer = false;
            Console.WriteLine();
        }

        bool runTimer = false;
        public void StartCoordinateTimer() {
            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                if (runTimer) {
                    //UpdatePositionFromGPS(); //comment out when testing
                    UpdateAllPins();
                    return true;
                } else {
                    return false;
                }
                
            });
        }

        bool first = true; //only move the map one time
        private async void UpdatePositionFromGPS() {
            var request = new Xamarin.Essentials.GeolocationRequest(Xamarin.Essentials.GeolocationAccuracy.Medium);
            var location = await Xamarin.Essentials.Geolocation.GetLocationAsync(request);

            Latitude = location.Latitude;
            Longitude = location.Longitude;

            if (first) {
                Position currentPosition = new Position(location.Latitude, location.Longitude);
                MyMap.MoveToRegion(
                    MapSpan.FromCenterAndRadius(
                    currentPosition, Distance.FromKilometers(1)));
                first = false;
            }
        }

        public void UpdateAllPins() {
            App.WebConnection.SendCoordinate(new Coordinate("coordinate", UserName, new Position(Latitude,Longitude)));

            try {
                MyMap.Pins.Clear();
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
                        MyMap.Pins.Add(opponentPin);
                    } else {
                        MyPosPin = new Pin
                        {
                            Label = item.Key,
                            Type = PinType.Place,
                            Position = item.Value,
                            Icon = boatPin
                        };
                        MyMap.Pins.Add(MyPosPin);
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
            MyMap.Circles.Clear();
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
                MyMap.Circles.Add(checkPoint);
                CheckPoints.Add(checkPoint);
            }
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

        /// <summary>
        /// fired by event when server emits "startrace"
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="message"></param>
        private void StartRace(object obj, string message) {
            runTimer = true;
            StartCoordinateTimer();
            Console.WriteLine("*** race started");
        }
    }
}
