using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace OxbridgeApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public Command LoginCommand { get; set; }

        public LoginViewModel() {
            this.LoginCommand = new Command(
                async (object message) =>
                {
                    await NavigationService.NavigateToAsync<MainMenuViewModel>();
                    Console.WriteLine("*Login*");
                },
                (object message) => { Console.WriteLine("*CanLogin*"); return true; });
        }
    }
}
