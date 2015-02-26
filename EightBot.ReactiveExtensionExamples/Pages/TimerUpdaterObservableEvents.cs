using System;

using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using ReactiveUI;
using EightBot.ReactiveExtensionExamples.Utilities;

namespace EightBot.ReactiveExtensionExamples.Pages
{
	public class TimerUpdaterObservableEvents : ContentPage
	{
		Label timerLabel;
		Button start, stop;

		IDisposable timerSubscription;

		public TimerUpdaterObservableEvents ()
		{

			start = new Button{ Text = "Start" };

			var startClicked = Observable.FromEventPattern (x => start.Clicked += x, x => start.Clicked -= x);

			startClicked
				.ObserveOn (RxApp.MainThreadScheduler)
				.Subscribe (args => {
					if(timerSubscription != null)
						timerSubscription.Dispose();

					timerSubscription = 
						Observable
							.Interval (TimeSpan.FromSeconds (1.5))
							.ObserveOn(RxApp.MainThreadScheduler)
							.Dump("Value")
							.Subscribe (timeInterval => timerLabel.Text = string.Format("Last Interval: {0}", timeInterval));
				});

			stop = new Button{ Text = "Stop" };
			var stopClicked = Observable.FromEventPattern (x => stop.Clicked += x, x => stop.Clicked -= x);

			stopClicked
				.ObserveOn (RxApp.MainThreadScheduler)
				.Subscribe (args => {
					if(timerSubscription != null)
						timerSubscription.Dispose();
				});

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


