using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OxbridgeApp.ViewModels;

namespace OxbridgeApp.Services
{
    public interface INavigationService
    {
        BaseViewModel PreviousPageViewModel { get; }

        Task InitializeAsync();

        Task NavigateToAsync<TViewModel>() where TViewModel : BaseViewModel;

        Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : BaseViewModel;

        Task NavigateToAsyncWithBack<TViewModel>() where TViewModel : BaseViewModel;

        Task NavigateToAsync(Type viewModelType);

        Task RemoveLastFromBackStackAsync();

        Task RemoveBackStackAsync();
    }
}
