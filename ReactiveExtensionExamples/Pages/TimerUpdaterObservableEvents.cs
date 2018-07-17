using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xamarin.Forms;

namespace ReactiveExtensionExamples.Pages
{
    public class TimerUpdaterObservableEvents : PageBase
	{
		Label timerLabel;
		Button start, stop;

		IDisposable timerSubscription;

		protected override void SetupUserInterface ()
		{
			start = new Button{ Text = "Start" };
			stop = new Button{ Text = "Stop" };

			Content = new StackLayout { 
				Padding = new Thickness(8d),
				Spacing = 16d,
				Children = {
					start, 
					stop,
					(timerLabel = new Label { HorizontalTextAlignment = TextAlignment.Center, Text = "Go Time" })
				}
			};
		}

		protected override void SetupReactiveExtensions ()
		{
            var intervalObservable =
                Observable
                    .Interval (TimeSpan.FromSeconds (1), TaskPoolScheduler.Default)
                    .Select(timeInterval => string.Format ("Last Interval: {0}", timeInterval));
                    
			Observable
				.FromEventPattern (
					x => start.Clicked += x, 
					x => start.Clicked -= x)
                .Subscribe (args => {
                    timerSubscription?.Dispose ();

                    timerSubscription =
                        intervalObservable
                            .Subscribe (timeInterval => 
                                Device.BeginInvokeOnMainThread(() => timerLabel.Text = timeInterval)
                            );
                })
                .DisposeWith (SubscriptionDisposables);

			Observable
				.FromEventPattern (
					x => stop.Clicked += x, 
					x => stop.Clicked -= x)
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


