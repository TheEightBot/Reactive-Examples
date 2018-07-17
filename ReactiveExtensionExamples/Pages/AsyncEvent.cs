using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Xamarin.Forms;

namespace ReactiveExtensionExamples.Pages
{
    public class AsyncEvent : PageBase
	{
		Label outputLabel;
		Button calculate, stop;

		IDisposable calculationSubscription;

		protected override void SetupUserInterface ()
		{
			Title = "Rx - Async Events";

			calculate = new Button{ Text = "Calculate" };

			stop = new Button{ Text = "STOP" };

			Content = new StackLayout { 
				Padding = new Thickness(8d),
				Spacing = 16d,
				Children = {
					calculate, 
					stop,
					(outputLabel = new Label { 
						HorizontalTextAlignment = TextAlignment.Center, 
						Text = "Let's calcuNOW, not calcuLATEr" 
					})
				}
			};
		}

		protected override void SetupReactiveExtensions ()
		{
            var stopClickedObservable = 
                Observable
                    .FromEventPattern (x => stop.Clicked += x, x => stop.Clicked -= x)
                    .Do(args => System.Diagnostics.Debug.WriteLine("Button 2 Clicked"))
                    .FirstAsync ();

            var calculationObservable = 
                Observable
                    .Interval (TimeSpan.FromMilliseconds (250))
                    .Do (val => System.Diagnostics.Debug.WriteLine ("Next Value: {0}", val))
                    .Scan ((previous, current) => previous + current);
                    
			Observable
				.FromEventPattern (x => calculate.Clicked += x, x => calculate.Clicked -= x)
                .Subscribe (async args => {
                    try {
                        Device.BeginInvokeOnMainThread(() => calculate.IsEnabled = false);

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
                        await stopClickedObservable;

                        calculationSubscription?.Dispose();

                        Device.BeginInvokeOnMainThread(() => 
                            outputLabel.Text = string.Format("Clicked Stop at " + DateTime.Now)
                        );
                    } finally {
                        Device.BeginInvokeOnMainThread(() => calculate.IsEnabled = true);
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


