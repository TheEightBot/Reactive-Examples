
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
using System.Threading.Tasks;

namespace ReactiveExtensionExamples.Droid
{
	[Activity (		
		Label = "Rx Examples", 
		Icon = "@mipmap/ic_launcher",
		Theme = "@style/Theme.Splash",
		MainLauncher = true, NoHistory = true)]			
	public class SplashScreen : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			this.StartActivity (typeof(MainActivity));
			this.Finish ();
		}
	}
}

