using System;

using Xamarin.Forms;

namespace EightBot.ReactiveExtensionExamples
{
	public class App : Application
	{
		public App ()
		{
			MainPage = new Pages.NavigationContainerPage ();
		}

		protected override void OnStart ()
		{
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

