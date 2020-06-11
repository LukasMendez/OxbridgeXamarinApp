using OxbridgeApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
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
                async (object button) =>
                {
                    bool success = await App.WebConnection.Login(Username, Password);
                    if (success) {
                        await MoveAnimation((Button)button);
                        Console.WriteLine(Preferences.Get(CurrentUser.TokenKey,"none"));
                        var mainMenuViewModel = ServiceContainer.Resolve<MainMenuViewModel>();
                        mainMenuViewModel.UserText = "Welcome " + Preferences.Get(CurrentUser.Username, null) + " (" + Preferences.Get(CurrentUser.Team, null) + ")";
                        mainMenuViewModel.RaceButtonText = "Enter Race";

                        var masterDetailViewModel = ServiceContainer.Resolve<MasterDetailViewModel>();
                        // If the MasterMenuItem 'Login' was visible before. It will be hidden as the user is now logged in
                        // This will add a 'Sign-out' button instead
                        masterDetailViewModel.SwitchLoginState();

                        await NavigationService.NavigateToAsync<MainMenuViewModel>();
                    } else {
                        MoveTransformation((Button)button);
                        ErrorMessage = "Invalid username or password";
                    }
                },
                (object message) => { Console.WriteLine("*CanLogin*"); return true; });
        }

        private async Task MoveAnimation(Button button) {
            Task loginButtonTranslate = button.TranslateTo(50, 0, 400, Easing.CubicIn).ContinueWith(x => button.TranslateTo(400, 0, 500, Easing.CubicOut));
            Task loginButtonFade = button.FadeTo(1, 400).ContinueWith(x => button.FadeTo(0, 200));
            Task loginButtonDelay = button.RotateTo(0, 700); //purely for delay since no other available option to delay
            await Task.WhenAll(new List<Task> { loginButtonTranslate, loginButtonFade, loginButtonDelay });
        }

        public void MoveTransformation(Button button) {
            bool runTimer = true;
            int count = 0;
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 10), () =>
            {
                if (runTimer) {
                    if (count < 5) {
                        button.TranslationX += 4;
                        count++;
                    }
                    if(count >= 5 && count < 10) {
                        button.TranslationX -= 8;
                        count++;
                    }
                    if(count >= 10 && count < 15) {
                        button.TranslationX += 4;
                        count++;
                    }
                    if (count >= 15) {
                        runTimer = false;
                    }
                    return true; //continue timer
                } else {
                    return false; //stop timer
                }
            });
        }
    }
}
