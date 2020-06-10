using OxbridgeApp.Models;
using OxbridgeApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OxbridgeApp.ViewModels
{
    public class MainMenuViewModel : BaseViewModel {
        public Command SpectateCommand { get; set; }
        public ICommand ItemSelectedCommand { get; set; }
        public ObservableCollection<Race> RaceList { get; set; }
        public Race SelectedRace { get; set; }
        private string errorMessage;
        public string ErrorMessage {
            get { return errorMessage; }
            set { errorMessage = value;
                this.OnPropertyChanged();}
        }
        private string userText;
        public string UserText {
            get { return userText; }
            set { userText = value;
                this.OnPropertyChanged();}
        }
        private string raceButtonText;
        public string RaceButtonText {
            get { return raceButtonText; }
            set {
                raceButtonText = value;
                this.OnPropertyChanged();
            }
        }
        public bool IsSpectator { get; set; }



        public MainMenuViewModel() {
            //Preferences.Get(CurrentUser.IsLoggedIn.ToString(), null) == null || Preferences.Get(CurrentUser.IsLoggedIn.ToString(), null).Equals("False")
            string test = Preferences.Get(CurrentUser.IsLoggedIn.ToString(), null);
            if (Preferences.Get(CurrentUser.Username.ToString(), null) != null && Preferences.Get(CurrentUser.Username.ToString(), null) != "usernameKey") {
                UserText = "Welcome " + Preferences.Get(CurrentUser.Username, null) + " (" + Preferences.Get(CurrentUser.Team, null) + ")";
                RaceButtonText = "Enter race";
                IsSpectator = false;
            } else {
                UserText = "Welcome Spectator";
                RaceButtonText = "Spectate";
                IsSpectator = true;
            }

            RaceList = new ObservableCollection<Race>();

            this.SpectateCommand = new Command(
                async (object message) =>
                {
                    if(SelectedRace != null) {
                        if (!IsSpectator) {
                            bool access = await App.WebConnection.JoinRace(SelectedRace.RaceID);
                            if (access) {
                                await NavigationService.NavigateToAsyncWithBack<RaceViewModel>();
                            } else {
                                await Application.Current.MainPage.DisplayAlert("Access denied", "Your team is not assigned to participate in this race", "Ok");
                            }
                            //Console.WriteLine("*Spectate*");
                        } else {
                            await NavigationService.NavigateToAsyncWithBack<RaceViewModel>();
                        }
                    }
                },
                (object message) => { Console.WriteLine("*CanSpectate*"); return true; });

            ItemSelectedCommand = new Command<Race>(SelectRace);

            UpdateRaceList();
            Console.WriteLine();

        }

        void SelectRace(Race race) {
            Console.WriteLine("selectedItem: " + race.StartTime + " " + race.LocationDescription);
            SelectedRace = race;
        }

        private async void UpdateRaceList() {
            ObservableCollection<Race> temp;
            temp = await GetRaces();
            if (temp != null) {
                foreach (var item in temp) {
                    RaceList.Add(item);
                }
            } else {
                ErrorMessage = "Server-Error getting races.";
            }
        }

        private async Task<ObservableCollection<Race>> GetRaces() {
            return await App.WebConnection.GetRaces();
        }

        
    }
}
