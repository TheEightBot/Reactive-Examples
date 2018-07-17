using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xamarin.Forms;

namespace ReactiveExtensionExamples.Pages
{
    public class Async : PageBase
	{
		Label outputLabel, calculationProgress;
		Button download;

		protected override void SetupUserInterface ()
		{
			Title = "Rx - Async";

			download = new Button{ Text = "Calculate" };

			Content = new StackLayout { 
				Padding = new Thickness(8d),
				Spacing = 16d,
				Children = {
					download, 
					(calculationProgress = 
						new Label { 
							HorizontalTextAlignment = TextAlignment.Center, 
							FontAttributes = FontAttributes.Italic,
							Text = "Next Value: " 
						}
					),
					(outputLabel = new Label { HorizontalTextAlignment = TextAlignment.Center, Text = "Calculation Result" }),

				}
			};
		}

		protected override void SetupReactiveExtensions ()
		{
			var random = new Random(DateTime.Now.Millisecond);

			var calculationObservable = 
				Observable
					.Interval(TimeSpan.FromMilliseconds(random.Next(100, 300)))
					.Zip(
						Observable.Range(random.Next(1, 5), random.Next(2, 7)), 
						(t, r) => (long)r
					)
					.Scan((previous, current) => previous * current * (long)(random.Next(1, 35)))
					.Do(val => Device.BeginInvokeOnMainThread(() => calculationProgress.Text = string.Format("Next Value: {0}", val)));

			Observable
				.FromEventPattern (x => download.Clicked += x, x => download.Clicked -= x)
				.Subscribe (async args => {
					try {
						Device.BeginInvokeOnMainThread(() => {
							download.IsEnabled = false;
							outputLabel.Text = "Starting Calculation";
						});

						var result = await calculationObservable;

						Device.BeginInvokeOnMainThread(() =>
							outputLabel.Text = string.Format("Calculation Complete: {0}", result)
						);

					} finally {
						Device.BeginInvokeOnMainThread(() => 
							download.IsEnabled = true
						);
					}
				})
				.DisposeWith(SubscriptionDisposables);
		}
	}
}


