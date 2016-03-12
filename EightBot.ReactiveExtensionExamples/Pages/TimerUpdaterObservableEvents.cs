using System;

using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using ReactiveUI;
using EightBot.ReactiveExtensionExamples.Utilities;
using System.Reactive.Disposables;
using System.Reactive;

namespace EightBot.ReactiveExtensionExamples.Pages
{
	public class TimerUpdaterObservableEvents : PageBase
	{
		Label timerLabel;
		Button start, stop;

		IObservable<EventPattern<object>> 
			startClickedObservable, stopClickedObservable;

		IObservable<string> intervalObservable;

		IDisposable timerSubscription;

		protected override void SetupUserInterface ()
		{
			start = new Button{ Text = "Start" };
			stop = new Button{ Text = "Stop" };

			Content = new StackLayout { 
				Padding = new Thickness(40d),
				Children = {
					start, 
					stop,
					(timerLabel = new Label { HorizontalTextAlignment = TextAlignment.Center, Text = "Go Time" })
				}
			};
		}

		protected override void SetupReactiveObservables ()
		{
			startClickedObservable = 
				Observable
					.FromEventPattern (
						x => start.Clicked += x, 
						x => start.Clicked -= x
					);

			stopClickedObservable = 
				Observable
					.FromEventPattern (
						x => stop.Clicked += x, 
						x => stop.Clicked -= x
					);

			intervalObservable =
				Observable
					.Interval (TimeSpan.FromSeconds (1))
					.Select(timeInterval => string.Format ("Last Interval: {0}", timeInterval));
		}

		protected override void SetupReactiveSubscriptions ()
		{
			startClickedObservable
				.Subscribe (args => {
					timerSubscription?.Dispose ();

					timerSubscription =
						intervalObservable
							.ObserveOn (RxApp.MainThreadScheduler)
							.Subscribe (timeInterval => timerLabel.Text = timeInterval);
				})
				.DisposeWith (SubscriptionDisposables);

			stopClickedObservable
				.ObserveOn (RxApp.MainThreadScheduler)
				.Subscribe (args => timerSubscription?.Dispose ())
				.DisposeWith (SubscriptionDisposables);
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();

			timerSubscription?.Dispose ();
		}
	}
}


