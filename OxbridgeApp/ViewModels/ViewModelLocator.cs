using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace OxbridgeApp.ViewModels
{
    class ViewModelLocator
    {
        public static readonly BindableProperty AutoWireViewModelProperty =
            BindableProperty.CreateAttached("AutoWireViewModel", typeof(bool), typeof(ViewModelLocator), default(bool), propertyChanged: OnAutoWireViewModelChanged);

        public static bool GetAutoWireViewModel(BindableObject bindable) {
            return (bool)bindable.GetValue(ViewModelLocator.AutoWireViewModelProperty);
        }

        public static void SetAutoWireViewModel(BindableObject bindable, bool value) {
            bindable.SetValue(ViewModelLocator.AutoWireViewModelProperty, value);
        }

        public static T Resolve<T>() where T : class {
            return ServiceContainer.Resolve<T>();
        }
        private static void OnAutoWireViewModelChanged(BindableObject bindable, object oldValue, object newValue) {
            Console.WriteLine("*** autowire called");
            var view = bindable as Element;
            if (view == null) {
                Console.WriteLine("ViewModel type was null");
                return;
            }

            var viewType = view.GetType();
            var viewName = viewType.FullName.Replace(".Views.", ".ViewModels.");
            var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            var viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}Model, {1}", viewName, viewAssemblyName);

            var viewModelType = Type.GetType(viewModelName);
            if (viewModelType == null) {
                return;
            }
            var viewModel = ServiceContainer.Resolve(viewModelType);
            Console.WriteLine("Setting binding context of {0} to {1}", view.GetType(), viewModel.GetType());
            view.BindingContext = viewModel;
        }
    }
}
