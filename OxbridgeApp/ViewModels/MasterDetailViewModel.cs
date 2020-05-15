using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using OxbridgeApp.Models;

namespace OxbridgeApp.ViewModels
{
    class MasterDetailViewModel : BaseViewModel
    {

        /*
         * //relatively more clean MVVM
        private MasterMenuItems _selectedItem = null;

        public MasterMenuItems SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;

                if (_selectedItem == null)
                    return;

                ChangeVMCMD.Execute(_selectedItem);

                SelectedItem = null;
            }
        }
        **/

        public ICommand ChangeVMCMD => new Command<MasterMenuItems>(async (MasterMenuItems mmi) => {

            await NavigationService.NavigateToAsync(mmi.TargetViewModel);
        });
    }
}
