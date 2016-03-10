using System;

using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using ReactiveUI;
using EightBot.ReactiveExtensionExamples.Utilities;
using System.Threading.Tasks;
using System.Reactive.Threading.Tasks;

namespace EightBot.ReactiveExtensionExamples.Pages
{
	public class AsyncEvent : PageBase
	{
		Label outputLabel;
		Button button1, button2;

		protected override void SetupUserInterface ()
		{
			Title = "Rx - Async Events";

			button1 = new Button{ Text = "Calculate" };
			button2 = new Button{ Text = "STOP" };

			Content = new StackLayout { 
				Padding = new Thickness(40d),
				Children = {
					button1, 
					button2,
					(outputLabel = new Label { XAlign = TextAlignment.Center, Text = "Let's calcuNOW, not calcuLATEr" }),

				}
			};
		}

		protected override void SetupReactiveExtensions ()
		{
			var button1Clicked = 
				Observable
					.FromEventPattern (x => button1.Clicked += x, x => button1.Clicked -= x);

			var button2Clicked = 
				Observable
					.FromEventPattern (x => button2.Clicked += x, x => button2.Clicked -= x)
					.Do(args => {  
						System.Diagnostics.Debug.WriteLine("Button 2 Clicked");
					})
					.FirstAsync ();

			var calculationObservable = 
				Observable
					.Interval (TimeSpan.FromMilliseconds (250))
					.Do (val => System.Diagnostics.Debug.WriteLine ("Next Value: {0}", val))
					.Scan ((previous, current) => previous + current);

			button1Clicked
				.ObserveOn (RxApp.MainThreadScheduler)
				.Subscribe (async args => {

					//Start Calculating
					var calculationSubscription = 
						calculationObservable
							.ObserveOn(RxApp.MainThreadScheduler)
							.Subscribe(val => {
								outputLabel.Text = string.Format("Calculation Value: {0}", val);
							});

					//This will only get the first click of the button after we start listening
					await button2Clicked;

					//Unsubscribe from calculations, this will stop the calculation process
					calculationSubscription.Dispose();

					outputLabel.Text = string.Format("Clicked Stop at " + DateTime.Now);

				});
		}
	}
}


