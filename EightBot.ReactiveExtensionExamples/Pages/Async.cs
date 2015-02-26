using System;

using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using ReactiveUI;
using EightBot.ReactiveExtensionExamples.Utilities;
using System.Threading.Tasks;

namespace EightBot.ReactiveExtensionExamples.Pages
{
	public class Async : ContentPage
	{
		Label outputLabel;
		Button button1;

		public Async ()
		{

			button1 = new Button{ Text = "Calculate" };

			var button1Clicked = Observable.FromEventPattern (x => button1.Clicked += x, x => button1.Clicked -= x);

			Content = new StackLayout { 
				Padding = new Thickness(40d),
				Children = {
					button1, 
					(outputLabel = new Label { XAlign = TextAlignment.Center, Text = "Let's calcuNOW, not calcuLATEr" }),

				}
			};
					
			button1Clicked
				.ObserveOn (RxApp.MainThreadScheduler)
				.Dump ("Value")
				.Subscribe (async args => {
					var random = new Random();

					var calculationObservable = Observable
						.Interval(TimeSpan.FromMilliseconds(250))
						.Zip(Observable.Range(random.Next(1, 5), random.Next(1, 25)), (t, r) => r)
						.Scan((previous, current) => previous * current);

					var calculationDisposable = calculationObservable
						.ObserveOn(RxApp.MainThreadScheduler)
						.Subscribe(async currentCalculation => {
							outputLabel.Text = string.Format("Final Calculation: {0}", currentCalculation);
						});

					var result = await calculationObservable;

					outputLabel.Text = string.Format("Calculation Complete", result);
				});

		}
	}
}


