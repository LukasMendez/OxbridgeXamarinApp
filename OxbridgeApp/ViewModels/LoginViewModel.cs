using OxbridgeApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OxbridgeApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        private string errorMessage;

        public string ErrorMessage {
            get { return errorMessage; }
            set { errorMessage = value;
                  this.OnPropertyChanged();
            }
        }
        public Command LoginCommand { get; set; }


        public LoginViewModel() {
            this.LoginCommand = new Command(
                async (object message) =>
                {
                    bool success = await App.WebConnection.Login(Username, Password);
                    if (success) {
                        Console.WriteLine(Preferences.Get(GlobalKeys.TokenKey,"none"));
                        await NavigationService.NavigateToAsync<MainMenuViewModel>();
                    } else {
                        ErrorMessage = "Invalid username or password";
                    }
                },
                (object message) => { Console.WriteLine("*CanLogin*"); return true; });
        }
    }
}
