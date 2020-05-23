using OxbridgeApp.Models;
using OxbridgeApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace OxbridgeApp.ViewModels
{
    public class MainMenuViewModel : BaseViewModel {
        public Command SpectateCommand { get; set; }
        public ICommand ItemSelectedCommand { get; set; }
        public ObservableCollection<Race> RaceList { get; set; }
        public Race SelectedRace { get; set; }


        #region remove this
        // TEST AREA 

        //public Command SendCommand { get; set; }
        //public Command ConnectCommand { get; set; }


        //private string infoMessage = "Not connected";
        //public string InfoMessage
        //{
        //    get { return infoMessage; }
        //    set { infoMessage = value; this.OnPropertyChanged(); }
        //}


        //private string myMessageText;
        //public string MyMessageText
        //{
        //    get { return myMessageText; }
        //    set { myMessageText = value; this.OnPropertyChanged(); }
        //}

        //private string receivedText;
        //public string ReceivedText
        //{
        //    get { return receivedText; }
        //    set { receivedText = value; this.OnPropertyChanged(); }
        //}

        //private void ReceivedMessageTestMethod(object obj, string message)
        //{
        //    this.ReceivedText = message;
        //}

        //private void Connected(object obj)
        //{
        //    this.InfoMessage = "Connected successfully";
        //}


        #endregion

        public MainMenuViewModel() {
            RaceList = new ObservableCollection<Race>();

            #region remove this
            // TEST AREA
            //App.WebConnection.NewMessageReceived += ReceivedMessageTestMethod;
            //App.WebConnection.ConnectedEvent += Connected;


            //SendCommand = new Command(() =>
            //{
            //    App.WebConnection.SendMessage(MyMessageText);
            //});

            //ConnectCommand = new Command(() =>
            //{
            //    App.WebConnection.ConnectAndTest();
            //});


            #endregion

            this.SpectateCommand = new Command(
                async (object message) =>
                {
                    await NavigationService.NavigateToAsyncWithBack<RaceViewModel>();
                    Console.WriteLine("*Spectate*");
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
            foreach (var item in temp) {
                RaceList.Add(item);
            }
        }

        private async Task<ObservableCollection<Race>> GetRaces() {
            return await WebConnection.GetRaces();
        }
    }
}
