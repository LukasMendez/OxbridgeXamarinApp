using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using OxbridgeApp.Models;
using System.Collections.ObjectModel;
using OxbridgeApp.Services;
using Xamarin.Essentials;

namespace OxbridgeApp.ViewModels
{
    class MasterDetailViewModel : BaseViewModel
    {


        private bool isPresented = false;
        public bool IsPresented {
            get
            {
                return isPresented;
            }
            set
            {
                isPresented = value; this.OnPropertyChanged();
            }

        }

        public ObservableCollection<MasterMenuItem> MasterMenuItems { get; set; }

        // Sign-in item
        private MasterMenuItem LoginItem = new MasterMenuItem()
        {
            Text = "Login",
            Detail = "Sign-in as Team Leader",
            ImagePath = "leader.png",
            TargetViewModel = typeof(LoginViewModel)
        };

        // Sign-out item
        private MasterMenuItem SignOutItem = new MasterMenuItem()
        {
            Text = "Sign-out",
            Detail = "Exit",
            ImagePath = "exit.png",
            TargetViewModel = typeof(MainMenuViewModel)
        };

        public ICommand ChangeVMCMD { get; set; }

        public MasterDetailViewModel()
        {
            MasterMenuItems = new ObservableCollection<MasterMenuItem>();

            MasterMenuItems.Add(new MasterMenuItem()
            {
                Text = "Home",
                Detail = "View incoming races",
                ImagePath = "start.png",
                TargetViewModel = typeof(MainMenuViewModel)
            });

            // Show login at first 
            MasterMenuItems.Add(LoginItem);

            ChangeVMCMD = new Command<MasterMenuItem>(SelectPage);
            SwitchLoginState(); //checking if user is already logged in at startup

        }

        async void SelectPage(MasterMenuItem item)
        {
            IsPresented = false;
            Console.WriteLine("*Switching view*");
            if (item.Text.Equals("Sign-out")) {
                SignOut();
            }
            await NavigationService.NavigateToAsync(item.TargetViewModel);

        }

        private void SignOut() {
            CurrentUser.RemoveCurrentUser();
            SwitchLoginState();
        }

        public void SwitchLoginState()
        {
            //making sure we actually know if we are logged in
            if(Preferences.Get(CurrentUser.TokenKey, null) != null) { //if has token
                CurrentUser.IsLoggedIn = true;
            } else {
                CurrentUser.IsLoggedIn = false;
            }
            //clear menuItems (avoiding any weird situations)
            MasterMenuItems.Remove(LoginItem);
            MasterMenuItems.Remove(SignOutItem);
            //set correct menuItem
            if (CurrentUser.IsLoggedIn) {
                MasterMenuItems.Add(SignOutItem);
            } else {
                MasterMenuItems.Add(LoginItem);
            }
        }

    }
}
