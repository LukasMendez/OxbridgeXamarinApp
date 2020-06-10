using OxbridgeApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OxbridgeApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginView : ContentPage
    {
        public LoginView() {
            InitializeComponent();
            //errorLabel = new Label();
        }

        LoginViewModel loginViewModel = ServiceContainer.Resolve<LoginViewModel>();

        //private async void loginButton_Clicked(object sender, EventArgs e) {
        //    loginViewModel.VerifyLoginCommand.Execute(this);
        //    if (errorLabel.Text != null && errorLabel.Text.Equals("OK")) {
        //        await MoveTransformation();
        //    } else {
        //        Console.WriteLine("**** NO LOGIN");
        //    }
        //}

        
        //private async Task MoveTransformation() {
        //    Task loginButtonTranslate = loginButton.TranslateTo(50, 0, 400, Easing.CubicIn).ContinueWith(x => loginButton.TranslateTo(400, 0, 500, Easing.CubicOut));
        //    Task loginButtonFade = loginButton.FadeTo(1, 400).ContinueWith(x => loginButton.FadeTo(0, 200));
        //    await Task.WhenAll(new List<Task> { loginButtonTranslate, loginButtonFade });
        //    loginViewModel.LoginCommand.Execute(this);
        //}
    }
}