using System;

using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using ReactiveUI;
using EightBot.ReactiveExtensionExamples.Utilities;
using System.Threading.Tasks;
using System.Reactive.Disposables;

namespace EightBot.ReactiveExtensionExamples.Pages
{
	public class Async : PageBase
	{
		Label outputLabel;
		Button button1;
			

		protected override void SetupUserInterface ()
		{
			Title = "Rx - Async";

			button1 = new Button{ Text = "Calculate" };

			Content = new StackLayout { 
				Padding = new Thickness(40d),
				Children = {
					button1, 
					(outputLabel = new Label { HorizontalTextAlignment = TextAlignment.Center, Text = "Let's calcuNOW, not calcuLATEr" }),

				}
			};
		}

		protected override void SetupReactiveExtensions ()
		{
			var random = new Random();

			var calculationObservable = 
				Observable
					.Interval(TimeSpan.FromMilliseconds(250))
					.Zip(
						Observable.Range(random.Next(1, 5), random.Next(1, 25)), 
						(t, r) => r
					)
					.Do(val => System.Diagnostics.Debug.WriteLine("Next Value: {0}", val))
					.Scan((previous, current) => previous * current * random.Next());

			Observable
				.FromEventPattern (x => button1.Clicked += x, x => button1.Clicked -= x)
				.ObserveOn (RxApp.MainThreadScheduler)
				.Subscribe (async args => {
					outputLabel.Text = "Starting Calculation";

					var result = await calculationObservable;

					outputLabel.Text = string.Format("Calculation Complete: {0}", result);
				});
		}
	}
}


