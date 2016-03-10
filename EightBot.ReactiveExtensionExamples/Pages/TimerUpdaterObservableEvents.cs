using System;

using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using ReactiveUI;
using EightBot.ReactiveExtensionExamples.Utilities;
using System.Reactive.Disposables;

namespace EightBot.ReactiveExtensionExamples.Pages
{
	public class TimerUpdaterObservableEvents : ContentPage
	{
		Label timerLabel;
		Button start, stop;

		IDisposable timerSubscription;

		readonly CompositeDisposable _subscriptions = new CompositeDisposable();

		public TimerUpdaterObservableEvents ()
		{
			start = new Button{ Text = "Start" };
			stop = new Button{ Text = "Stop" };

			Content = new StackLayout { 
				Padding = new Thickness(40d),
				Children = {
					(timerLabel = new Label { XAlign = TextAlignment.Center, Text = "Go Time" }),
					start, 
					stop
				}
			};

		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			var startClicked = 
				Observable
					.FromEventPattern (x => start.Clicked += x, x => start.Clicked -= x);

			_subscriptions.Add (
				startClicked
					.Subscribe (args => {
						if(timerSubscription != null)
							timerSubscription.Dispose();

						timerSubscription = 
							Observable
								.Interval (TimeSpan.FromSeconds (1.5))
								.ObserveOn(RxApp.MainThreadScheduler)
								.Subscribe (timeInterval => timerLabel.Text = string.Format("Last Interval: {0}", timeInterval));
					})
			);

			var stopClicked = 
				Observable
					.FromEventPattern (x => stop.Clicked += x, x => stop.Clicked -= x);

			_subscriptions.Add (
				stopClicked
					.ObserveOn (RxApp.MainThreadScheduler)
					.Subscribe (args => {
						if(timerSubscription != null)
							timerSubscription.Dispose();
					})
			);
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();

			_subscriptions.Clear ();
		}
	}
}


