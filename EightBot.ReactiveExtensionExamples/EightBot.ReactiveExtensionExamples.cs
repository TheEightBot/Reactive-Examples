using System;

using Xamarin.Forms;

namespace EightBot.ReactiveExtensionExamples
{
	public class App : Application
	{
		public App ()
		{
			MainPage = 
				//TODO: Demo 1: Timer
				//new Pages.TimerUpdater ();
				//TODO: Demo 2: Timer with better event handling
				//new Pages.TimerUpdaterObservableEvents ();
				//TODO: Demo 3: Delay Notifications
				//new Pages.Delay ();
				//TODO: Demo 4: Throttle Notifications
				//new Pages.Throttle ();
				//TODO: Demo 5: Buffer Notifications
				//new Pages.Buffer ();
				//TODO: Demo 6: Buffer Notifications with Where Clause
				//new Pages.BufferWithWhere ();
				//TODO: Demo 7: Sample
				//new Pages.Sample ();
				//TODO: Demo 8: Merge Observables
				//new Pages.Merge ();
				//TODO: Demo 9: Async Support
				//new Pages.Async ();

				//SWITCHING GEARS TO REACTIVE UI

				//TODO: Demo 10: Simple Login
				//new Pages.ReactiveUiLogin ();
				//TODO: Demo 11: Simple Login With Validation
				//new Pages.ReactiveUiLoginWithValidation ();
				//TODO: Demo 12: Simple Login With Validation and Property Helpers
				//new Pages.ReactiveUiLoginWithValidationWithProperty ();
				//TODO: Demo 13: Simple Login With Commands
				//new Pages.ReactiveUiLoginWithCommand ();

				//TODO: Demo 14: Color Slider Example
				new Pages.ReactiveUiColorSlider ();

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

