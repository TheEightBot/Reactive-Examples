using System;

using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using ReactiveUI;
using EightBot.ReactiveExtensionExamples.Utilities;

namespace EightBot.ReactiveExtensionExamples.Pages
{
	public class TimerUpdater : ContentPage
	{
		Label timerLabel;
		Button start, stop;

		IDisposable timerSubscription;

		public TimerUpdater ()
		{

			start = new Button{ Text = "Start" };
			start.Clicked += (object sender, EventArgs e) => {

				if(timerSubscription != null)
					timerSubscription.Dispose();


				var timerObs = 
					Observable
						.Interval (TimeSpan.FromSeconds (1.5))
						.ObserveOn(RxApp.MainThreadScheduler)
						.Dump("Value");

					timerSubscription = 
						timerObs
						.Subscribe (timeInterval => 
							timerLabel.Text = 
								string.Format("Last Interval: {0}", timeInterval));

			};

			stop = new Button{ Text = "Stop" };
			stop.Clicked += (object sender, EventArgs e) => {
				if(timerSubscription != null)
					timerSubscription.Dispose();
			};

			Content = new StackLayout { 
				Padding = new Thickness(40d),
				Children = {
					(timerLabel = new Label { XAlign = TextAlignment.Center, Text = "Go Time" }),
					start, 
					stop
				}
			};

		}
	}
}


