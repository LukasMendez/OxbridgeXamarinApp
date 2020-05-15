using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace OxbridgeApp.Droid
{
    [Application]
    [MetaData("com.google.android.maps.v2.API_KEY",
              Value = "AIzaSyDHI4E2O7wPl2EnQ-200nIoV08wdhrdckQ")]
    public class MyApp : Application
    {
        public MyApp(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer) {
        }
    }
}