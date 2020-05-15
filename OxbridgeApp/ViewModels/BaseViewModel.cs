using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using System.Threading.Tasks;
using OxbridgeApp.Models;
using OxbridgeApp.Services;

namespace OxbridgeApp.ViewModels
{
    public abstract class BaseViewModel : ExtendedBindableObject
    {
        protected readonly INavigationService NavigationService;

        internal static string UserName = "";

        public BaseViewModel() {
            NavigationService = ViewModelLocator.Resolve<INavigationService>();
            var settingsService = ViewModelLocator.Resolve<ISettingsService>();

        }
        public virtual Task InitializeAsync(object navigationData) {
            return Task.FromResult(false);
        }
    }
}
