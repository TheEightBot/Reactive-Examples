using System;
using Refit;
using Splat;
using Xamarin.Forms;

namespace ReactiveExtensionExamples
{
	public class App : Application
	{
		public App ()
		{
			Values.Styles.Initialize ();
			MainPage = new Pages.NavigationContainerPage ();
		}

		protected override void OnStart ()
		{
			Locator
                .CurrentMutable
                .RegisterLazySingleton(
    				() => RestService.For<Services.Api.IDuckDuckGoApi>("https://api.duckduckgo.com"), 
    				typeof(Services.Api.IDuckDuckGoApi));
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

