using System;

using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Reactive.Concurrency;
using ReactiveUI;
using ReactiveExtensionExamples.Utilities;
using System.Threading.Tasks;
using System.Reactive.Disposables;
using System.Reactive;

namespace ReactiveExtensionExamples.Pages
{
	public class AsyncToObservable : PageBase
	{
		Label outputLabel;
		Button button1;
		ActivityIndicator loading;

		IObservable<EventPattern<Object>> button1ClickedObservable;

		protected override void SetupUserInterface ()
		{
			Title = "Rx - Async to Observable";

			button1 = new Button{ Text = "Call Service" };

			loading = new ActivityIndicator { };

			Content = new StackLayout { 
				Padding = new Thickness(8d),
				Spacing = 16d,
				Children = {
					button1, 
					(outputLabel = new Label { HorizontalTextAlignment = TextAlignment.Center, Text = "" }),
					loading
				}
			};
		}

		protected override void SetupReactiveObservables ()
		{
			button1ClickedObservable =
				Observable
					.FromEventPattern (x => button1.Clicked += x, x => button1.Clicked -= x);
		}

		protected override void SetupReactiveSubscriptions ()
		{
			button1ClickedObservable
				.Subscribe (async args => {

					Device.BeginInvokeOnMainThread(() => {
						outputLabel.TextColor = Color.Black;
						outputLabel.Text = "Starting Calculation";
						loading.IsRunning = true;
					});
						
					try {
						var result = 
							await Observable
								.FromAsync(() => PerformCalculation())
								.Timeout(TimeSpan.FromMilliseconds(300))
								.Retry(5)
								.Catch<int, TimeoutException>(tex => Observable.Return(-1))
								.Catch<int, Exception>(ex => Observable.Return(-100));

						Device.BeginInvokeOnMainThread(() => {
							outputLabel.Text = 
								result >= 0
									? string.Format("Calculation Complete: {0}", result)
									: result == -100
										? "Listen, things went really bad." + Environment.NewLine + "Reconsider your life choices"
										: "Bummer, it looks like your calculation failed";

							if(result < 0)
								outputLabel.TextColor = Color.Red;
						});
					} finally {
						Device.BeginInvokeOnMainThread(() => loading.IsRunning = false);
					}
				})
				.DisposeWith(SubscriptionDisposables);
		}


		//Imagine this is a faux web service or similar
		async Task<int> PerformCalculation (){
			var random = new Random (DateTime.Now.Millisecond);

			var delayTime = random.Next (150, 500);

			System.Diagnostics.Debug.WriteLine ("Delaying {0}", delayTime);

			await Task.Delay (delayTime);

			var calcedValue = random.Next (1, 10);

			System.Diagnostics.Debug.WriteLine ("Calced Value {0}", calcedValue);

			if (calcedValue % 2 == 0)
				throw new Exception ("Even number are not allowed");

			return calcedValue;
		}
	}
}


