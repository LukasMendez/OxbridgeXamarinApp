using OxbridgeApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;


using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OxbridgeApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginView : ContentPage
    {
        public LoginView() {
            InitializeComponent();

            //OS specific programming
            if (Device.RuntimePlatform == Device.Android) {
                Padding = new Thickness(0, 20, 0, 0);
            }

            //OS specific programming using compiler statements
            #if __MOBILE__
            // Xamarin iOS or Android-specific code
            Padding = new Thickness(0, 20, 0, 0);
            #endif

            #if __IOS__
            // iOS-specific code
            Padding = new Thickness(0, 20, 0, 0);
            #endif

            #if __ANDROID__
            // Android-specific code
            Padding = new Thickness(0, 20, 0, 0);
            #endif

        }


    }
}