using System;

using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using ReactiveUI;
using EightBot.ReactiveExtensionExamples.Utilities;
using System.Threading.Tasks;
using System.Reactive.Threading.Tasks;
using System.Reactive;

namespace EightBot.ReactiveExtensionExamples.Pages
{
	public class AsyncEvent : PageBase
	{
		Label outputLabel;
		Button button1, button2;

		IObservable<EventPattern<Object>>
			button1ClickedObservable, button2ClickedObservable;

		IObservable<long> calculationObservable;

		IDisposable calculationSubscription;

		protected override void SetupUserInterface ()
		{
			Title = "Rx - Async Events";

			button1 = new Button{ Text = "Calculate" };

			button2 = new Button{ Text = "STOP" };

			Content = new StackLayout { 
				Padding = new Thickness(8d),
				Spacing = 16d,
				Children = {
					button1, 
					button2,
					(outputLabel = new Label { 
						HorizontalTextAlignment = TextAlignment.Center, 
						Text = "Let's calcuNOW, not calcuLATEr" 
					})
				}
			};
		}

		protected override void SetupReactiveObservables ()
		{
			button1ClickedObservable = 
				Observable
					.FromEventPattern (x => button1.Clicked += x, x => button1.Clicked -= x);

			button2ClickedObservable = 
				Observable
					.FromEventPattern (x => button2.Clicked += x, x => button2.Clicked -= x)
					.Do(args => System.Diagnostics.Debug.WriteLine("Button 2 Clicked"))
					.FirstAsync ();

			calculationObservable = 
				Observable
					.Interval (TimeSpan.FromMilliseconds (250))
					.Do (val => System.Diagnostics.Debug.WriteLine ("Next Value: {0}", val))
					.Scan ((previous, current) => previous + current);
		}

		protected override void SetupReactiveSubscriptions ()
		{
			button1ClickedObservable
				.Subscribe (async args => {
					try {
						Device.BeginInvokeOnMainThread(() => button1.IsEnabled = false);

						//Start Calculating
						calculationSubscription = 
							calculationObservable
								.ObserveOn(RxApp.MainThreadScheduler)
								.Subscribe(val => 
									Device.BeginInvokeOnMainThread(() => 
										outputLabel.Text = string.Format("Calculation Value: {0}", val)
									)
								);
						
						//This will only get the first click of the button after we start listening
						await button2ClickedObservable;

						calculationSubscription?.Dispose();

						Device.BeginInvokeOnMainThread(() => 
							outputLabel.Text = string.Format("Clicked Stop at " + DateTime.Now)
						);
					} finally {
						Device.BeginInvokeOnMainThread(() => button1.IsEnabled = true);
					}
				})
				.DisposeWith(SubscriptionDisposables);
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();

			calculationSubscription?.Dispose();
		}
	}
}


