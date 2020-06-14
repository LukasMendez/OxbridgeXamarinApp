using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using OxbridgeApp.ViewModels;
using OxbridgeApp.Views;

namespace OxbridgeApp.Services
{
    class NavigationService : INavigationService {
        private readonly ISettingsService _settingsService;

        public BaseViewModel PreviousPageViewModel {
            get {
                var mainPage = Application.Current.MainPage as CustomNavigationPage;
                var viewModel = mainPage.Navigation.NavigationStack[mainPage.Navigation.NavigationStack.Count - 2].BindingContext;
                return viewModel as BaseViewModel;
            }
        }

        public NavigationService(ISettingsService settingsService) {
            _settingsService = settingsService;
        }

        public Task InitializeAsync() {
            return NavigateToAsync<MainMenuViewModel>();//The startpage
        }

        public Task NavigateToAsync<TViewModel>() where TViewModel : BaseViewModel {
            return InternalNavigateToAsync(typeof(TViewModel), null);
        }

        public Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : BaseViewModel {
            return InternalNavigateToAsync(typeof(TViewModel), parameter);
        }

        /// <summary>
        /// Used by the Master Detail Menu
        /// </summary>
        /// <param name="viewModelType"></param>
        /// <returns></returns>
        public Task NavigateToAsync(Type viewModelType) {
            return InternalNavigateToAsync(viewModelType, null);
        }

        public Task RemoveLastFromBackStackAsync() {
            var mainPage = Application.Current.MainPage as CustomNavigationPage;

            if (mainPage != null) {
                mainPage.Navigation.RemovePage(
                    mainPage.Navigation.NavigationStack[mainPage.Navigation.NavigationStack.Count - 2]);
            }

            return Task.FromResult(true);
        }

        public Task RemoveBackStackAsync() {
            var mainPage = Application.Current.MainPage as CustomNavigationPage;

            if (mainPage != null) {
                for (int i = 0; i < mainPage.Navigation.NavigationStack.Count - 1; i++) {
                    var page = mainPage.Navigation.NavigationStack[i];
                    mainPage.Navigation.RemovePage(page);
                }
            }

            return Task.FromResult(true);
        }

        private async Task InternalNavigateToAsync(Type viewModelType, object parameter) {
            Page page = CreatePage(viewModelType, parameter);

            //Use Master detail navigation instead
            var mpage = Application.Current.MainPage as MasterDetailPage;
            mpage.Detail = new NavigationPage(page);

            /*
            if (page is TestView) //avoid back buttons
            {
                NavigationPage.SetHasBackButton(page, false); //remove back button
                NavigationPage.SetHasNavigationBar(page, false); //remove top nav bar

                Application.Current.MainPage = new CustomNavigationPage(page);
            }
            else
            {
                var navigationPage = Application.Current.MainPage as CustomNavigationPage;
                if (navigationPage != null)
                {
                    NavigationPage.SetHasBackButton(page, false); //remove back button
                    NavigationPage.SetHasNavigationBar(page, false); //remove top nav bar
                    await navigationPage.PushAsync(page);
                }
                else
                {
                    NavigationPage.SetHasBackButton(page, false); //remove back button
                    NavigationPage.SetHasNavigationBar(page, false); //remove top nav bar

                    Application.Current.MainPage = new CustomNavigationPage(page);
                }
            }*/

            await (page.BindingContext as BaseViewModel).InitializeAsync(parameter);//error why
        }

        public Task NavigateToAsyncWithBack<TViewModel>() where TViewModel : BaseViewModel {
            return InternalNavigateToAsyncWithBack(typeof(TViewModel), null);
        }

        private async Task InternalNavigateToAsyncWithBack(Type viewModelType, object parameter) {
            Page page = CreatePage(viewModelType, parameter);
            var mainPage = Application.Current.MainPage as MasterDetailPage;
            if (mainPage != null) {
                await mainPage.Detail.Navigation.PushAsync(page);
            }
        }

        private Type GetPageTypeForViewModel(Type viewModelType) {
            var viewName = viewModelType.FullName.Replace("Model", string.Empty);
            var viewModelAssemblyName = viewModelType.GetTypeInfo().Assembly.FullName;
            var viewAssemblyName = string.Format(CultureInfo.InvariantCulture, "{0}, {1}", viewName, viewModelAssemblyName);
            var viewType = Type.GetType(viewAssemblyName);
            return viewType;
        }

        private Page CreatePage(Type viewModelType, object parameter) {
            Type pageType = GetPageTypeForViewModel(viewModelType);
            if (pageType == null) {
                throw new Exception($"Cannot locate page type for {viewModelType}");
            }

            Page page = Activator.CreateInstance(pageType) as Page;
            return page;
        }
    }
}
