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
using System.IO.IsolatedStorage;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;

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
        public List<Circle> CheckPoints { get; set; }
        private Dictionary<string, Position> Participants { get; set; } //used to keep track of coordinates of all participants
        private ObservableCollection<string> leaderboardList;
        public ObservableCollection<string> LeaderboardList {
            get { return leaderboardList; }
            set { leaderboardList = value;
                this.OnPropertyChanged();
            }
        }

        private int NextCheckPoint { get; set; }
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
            mainMenuViewModel = ServiceContainer.Resolve<MainMenuViewModel>();
            Participants = new Dictionary<string, Position>();
            LeaderboardList = new ObservableCollection<string>();
            this.MyMap = new Map();
            this.MyMap.MapType = MapType.Hybrid;
            UserName = Preferences.Get(CurrentUser.Username, null);
            App.WebConnection.NewCoordReceived += ReceivedCoord;
            App.WebConnection.StartRaceReceived += StartRace;
            App.WebConnection.LeaderboardReceived += UpdateLeaderboard;
            App.WebConnection.ConnectSocket();

            //initial position
            Latitude = 54.910646;
            Longitude = 9.783899;
            MyPosition = new Position(Latitude, Longitude);

            CheckPoints = new List<Circle>();
            NextCheckPoint = 1;

            //move to testing position 
            MyMap.MoveToRegion(
                MapSpan.FromCenterAndRadius(
                MyPosition, Distance.FromKilometers(1)));

            this.NorthCommand = new Command(
                (object message) =>
                {
                    if (!mainMenuViewModel.IsSpectator) {
                        Latitude += 0.0002;
                        UpdateCheckPoints();
                        Console.WriteLine("*North*");
                    }
                },
                (object message) => { Console.WriteLine("*CanNorth*"); return true; });
            this.SouthCommand = new Command(
                (object message) =>
                {
                    if (!mainMenuViewModel.IsSpectator) {
                        Latitude -= 0.0002;
                        UpdateCheckPoints();
                        Console.WriteLine("*South*");
                    }
                },
                (object message) => { Console.WriteLine("*CanSouth*"); return true; });
            this.EastCommand = new Command(
                (object message) =>
                {
                    if (!mainMenuViewModel.IsSpectator) {
                        Longitude += 0.0002;
                        UpdateCheckPoints();
                        Console.WriteLine("*East*");
                    }
                },
                (object message) => { Console.WriteLine("*CanEast*"); return true; });
            this.WestCommand = new Command(
                (object message) =>
                {
                    if (!mainMenuViewModel.IsSpectator) {
                        Longitude -= 0.0002;
                        UpdateCheckPoints();
                        Console.WriteLine("*West*");
                    }
                },
                (object message) => { Console.WriteLine("*CanWest*"); return true; });
        }
        MainMenuViewModel mainMenuViewModel;

        /// <summary>
        /// Fired by Command attached to behavior attached to event "Appearing" which is called everytime the page is made visible/appears.
        /// </summary>
        private void Appearing() {
            // Will only show your boat/pin on the map if you are NOT a spectator
            if (!mainMenuViewModel.IsSpectator)
            {
                MyMap.Pins.Clear();
                MyPosPin = new Pin
                {
                    Label = "initial",
                    Type = PinType.Place,
                    Position = new Position(Latitude, Longitude),
                    Icon = boatPin
                };
                MyMap.Pins.Add(MyPosPin);
            } else
            {
                // If you have been logged in as Team Leader and then want to only spectate, we need to remove the previous pin
                MyMap.Pins.Clear();
            }

            LoadCheckPoints();
            if (!mainMenuViewModel.IsSpectator) {
                StartCoordinateTimer();
                UpdateCheckPoints();
                Console.WriteLine(Preferences.Get(CurrentUser.Team, null));
                App.WebConnection.SendMessage(new { Header = "addtoleaderboard", TeamName = Preferences.Get(CurrentUser.Team, null).ToString() });
            }
        }

        /// <summary>
        /// Fired by Command attached to behavior attached to event "Disappearing" which is called everytime the page is no longer visible/disappears.
        /// </summary>
        private void Disappearing() {
            //App.WebConnection.DisconnectSocket();
            runTimer = false;
            Console.WriteLine();
        }

        bool runTimer = false;
        /// <summary>
        /// Starting a timer that updates this users map with pins locally and sends its own 
        /// coordinates to the server every tick until stopped by "Disappearing" event/runTimer bool.
        /// </summary>
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
        /// <summary>
        /// Getting GPS information from device and using it to set current position
        /// </summary>
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

        /// <summary>
        /// Updating pins on the map by iterating Participants and drawing their locations.
        /// Also sending own coordinates to server.
        /// </summary>
        public void UpdateAllPins() {
            if (!mainMenuViewModel.IsSpectator) {
                //Send current coordinates
                App.WebConnection.SendMessage(new Coordinate("coordinate", Preferences.Get(CurrentUser.Team, null), new Position(Latitude, Longitude)));
            }

            try {
                MyMap.Pins.Clear();
                foreach (var item in Participants) {
                    if (!item.Key.Equals(Preferences.Get(CurrentUser.Team, null))) { //if opponent
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

        /// <summary>
        /// Getting checkpoints from SelectedRace property and drawing them as Circle objects on the map
        /// </summary>
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

        /// <summary>
        /// Updating the checkpoints so the next in line is blue and completed ones are green and tagged as "done".
        /// Also sending "checkpoint" message to server to indicate that the leaderboard should be updated.
        /// </summary>
        private void UpdateCheckPoints() {
            foreach (var item in CheckPoints) {
                if (item.Tag.ToString().Equals(NextCheckPoint.ToString())) { //if this checkpoint is the next
                    item.StrokeColor = Color.FromRgba(51, 61, 255, 88); //blue
                    item.FillColor = Color.FromRgba(51, 61, 255, 50);
                    if (!item.Tag.Equals("done")) { //if this checkpoint is not already completed
                        if (item.Center.Latitude - MyPosPin.Position.Latitude >= -0.0006 && //if boat is within this checkpoints range
                        item.Center.Latitude - MyPosPin.Position.Latitude <= 0.0006 &&
                        item.Center.Longitude - MyPosPin.Position.Longitude >= -0.0006 &&
                        item.Center.Longitude - MyPosPin.Position.Longitude <= 0.0006) {
                            item.StrokeColor = Color.FromRgba(71, 255, 51, 88); //green
                            item.FillColor = Color.FromRgba(71, 255, 51, 50);
                            NextCheckPoint += 1;
                            //emit the checkpoint since it was completed
                            App.WebConnection.SendMessage(new Checkpoint("checkpoint", Preferences.Get(CurrentUser.Team, null), item.Tag));
                            item.Tag = "done";
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Receiving a coordinate message from server and updating/adding it to Participants collection
        /// fired by event every time any client emits its coordinate
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="message"></param>
        private void ReceivedCoord(object obj, string message) {
            Coordinate coordinate = JsonConvert.DeserializeObject<Coordinate>(message);
            Participants[coordinate.TeamName] = coordinate.Position; //adding or updating this opponent
        }

        /// <summary>
        /// Starting the timer for sending coordinates when startrace message is received from server
        /// fired by event when server emits "startrace"
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="message"></param>
        private void StartRace(object obj, string message) {
            runTimer = true;
            StartCoordinateTimer();
            Console.WriteLine("*** race started"); 
        }

        /// <summary>
        /// Updating the current leaderboard collection with the one received from server
        /// fired by event when server emits "checkpoint"
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="message"></param>
        private void UpdateLeaderboard(object obj, string message) {
            SortedLeaderboard sortedLeaderboard = JsonConvert.DeserializeObject<SortedLeaderboard>(message);
            LeaderboardList.Clear();
            for (int i = 0; i < sortedLeaderboard.Leaderboard.Count; i++) {
                LeaderboardList.Add(i+1 + " " + sortedLeaderboard.Leaderboard[i].TeamName);
            }
        }
    }
}
