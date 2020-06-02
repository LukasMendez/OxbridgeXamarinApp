﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;
using OxbridgeApp.ViewModels;
using OxbridgeApp.Services;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace OxbridgeApp
{
    public partial class App : Application
    {
        private static WebConnection webConnection;
        public static WebConnection WebConnection {
            get {
                if (webConnection == null) {
                    webConnection = new WebConnection();
                }
                return webConnection;
            }
        }

        ISettingsService _settingsService;
        public App() {
            InitializeComponent();


            
            ServiceContainer.Register<ISettingsService>(() => new SettingService());
            _settingsService = ServiceContainer.Resolve<ISettingsService>();
            ServiceContainer.Register<INavigationService>(() => new NavigationService(_settingsService));

            ServiceContainer.Register<MainMenuViewModel>(() => new MainMenuViewModel());
            ServiceContainer.Register<LoginViewModel>(() => new LoginViewModel());
            ServiceContainer.Register<RaceViewModel>(() => new RaceViewModel());

            ServiceContainer.Register<MasterDetailViewModel>(() => new MasterDetailViewModel());

            //MainPage = new MainPage();
            var master = new Views.MasterDetailView();
            MainPage = master;
          //  master.BindingContext = masterDetailViewModel;
        }

        private Task InitNavigation() {
            var navigationService = ServiceContainer.Resolve<INavigationService>();
            return navigationService.InitializeAsync();
        }

        protected async override void OnStart() {
            // Handle when your app starts
            base.OnStart();
            await InitNavigation();
            base.OnResume();
        }

        protected override void OnSleep() {
            // Handle when your app sleeps
        }

        protected override void OnResume() {
            // Handle when your app resumes
        }
    }
}
