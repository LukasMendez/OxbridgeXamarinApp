using Newtonsoft.Json;
using OxbridgeApp.Models;
using OxbridgeApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;

namespace OxbridgeApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainMenuView : ContentPage
    {
        public MainMenuView() {
            InitializeComponent();
        }

        private void TempButton_Clicked(object sender, EventArgs e) {
            Console.WriteLine("*** " + Preferences.Get(CurrentUser.Username, null));
        }
    }
}