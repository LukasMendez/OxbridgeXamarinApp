using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using OxbridgeApp.Services;
using OxbridgeApp.Views;

namespace OxbridgeApp
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new RaceView();
            // THIS IS A TEST
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
