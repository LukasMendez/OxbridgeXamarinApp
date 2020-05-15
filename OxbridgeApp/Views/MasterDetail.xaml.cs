using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using OxbridgeApp.Models;
using OxbridgeApp.ViewModels;
using System.Reflection;

namespace OxbridgeApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterDetail : MasterDetailPage
    {
        public MasterDetail() {
            InitializeComponent();
            profileImage.Source = ImageSource.FromFile("spider.jpg");

            navigationList.ItemsSource = GetMenuList();

            IsPresented = false;
        }

        public List<MasterMenuItems> GetMenuList() {
            var list = new List<MasterMenuItems>();

            //list.Add(new MasterMenuItems()
            //{
            //    Text = "MainMenu",
            //    Detail = "Try it out",
            //    ImagePath = "skeleton.png",
            //    TargetViewModel = typeof(MainMenuViewModel)
            //});

            list.Add(new MasterMenuItems()
            {
                Text = "Login",
                Detail = "Test it out",
                ImagePath = "grill.png",
                TargetViewModel = typeof(LoginViewModel)
            });

            //list.Add(new MasterMenuItems()
            //{
            //    Text = "Race",
            //    Detail = "Test it out",
            //    ImagePath = "grill.png",
            //    TargetViewModel = typeof(RaceViewModel)
            //});

            return list;
        }

        private void OnMenuItemSelected(object sender, SelectedItemChangedEventArgs e) {
            var selectedMenuItem = (MasterMenuItems)e.SelectedItem;

            var viewModel = (ViewModels.MasterDetailViewModel)this.BindingContext;
            viewModel.ChangeVMCMD.Execute(selectedMenuItem);
            IsPresented = false;
            //if (((ListView)sender).SelectedItem != null) {
            //    ((ListView)sender).SelectedItem = null; // de-select the row
            //}
            navigationList.ItemsSource = GetMenuList(); //resets the list as a workaround for nulling SelectedItem
        }
    }
}