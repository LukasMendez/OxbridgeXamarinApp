using OxbridgeApp.Models;
using OxbridgeApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace OxbridgeApp.ViewModels
{
    public class MainMenuViewModel : BaseViewModel
    {
        public Command SpectateCommand { get; set; }

        public MainMenuViewModel() {
            this.SpectateCommand = new Command(
                async (object message) =>
                {
                    await NavigationService.NavigateToAsyncWithBack<RaceViewModel>();
                    Console.WriteLine("*Spectate*");
                },
                (object message) => { Console.WriteLine("*CanSpectate*"); return true; });

            GetRaces();
        }

        private async void GetRaces() {
            ObservableCollection<Race> races = await WebConnection.GetRaces();
            Console.WriteLine();
        }
    }
}
