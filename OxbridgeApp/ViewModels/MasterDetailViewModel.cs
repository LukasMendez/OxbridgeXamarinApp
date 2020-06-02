using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using OxbridgeApp.Models;
using System.Collections.ObjectModel;

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

        }

        async void SelectPage(MasterMenuItem item)
        {
            IsPresented = false;
            Console.WriteLine("*Switching view*");
            await NavigationService.NavigateToAsync(item.TargetViewModel);

        }

        public void SwitchLoginState()
        {
            if (MasterMenuItems.Contains(LoginItem))
            {
                MasterMenuItems.Remove(LoginItem);
                MasterMenuItems.Add(SignOutItem);
            } else if (MasterMenuItems.Contains(SignOutItem))
            {
                MasterMenuItems.Remove(SignOutItem);
                MasterMenuItems.Add(LoginItem);
            }
        }

    }
}
