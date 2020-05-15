using System;
using System.Collections.Generic;
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
                    await NavigationService.NavigateToAsync<RaceViewModel>();
                    Console.WriteLine("*Spectate*");
                },
                (object message) => { Console.WriteLine("*CanSpectate*"); return true; });
        }
    }
}
