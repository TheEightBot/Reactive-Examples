using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xamarin.Forms;

namespace ReactiveExtensionExamples.Pages
{
    public class CombineLatest: PageBase
	{
		BoxView colorDisplay;

		Slider red, green, blue;

		public CombineLatest ()
		{
			Title = "Combine Latest";

			Content = new StackLayout { 
				Padding = new Thickness(40d),
				Children = {
					(colorDisplay = new BoxView{ HeightRequest = 250 }),

					new Label{ Text = "Red"},
					(red = new Slider(0, 255, 0)),

					new Label{ Text = "Green"},
					(green = new Slider(0, 255, 0)),

					new Label{ Text = "Blue"},
					(blue = new Slider(0, 255, 0))
				}
			};
		}
			

		protected override void SetupReactiveExtensions ()
		{
			base.SetupReactiveExtensions ();

			var redValueChangedObservable =
				Observable
					.FromEventPattern<EventHandler<ValueChangedEventArgs>, ValueChangedEventArgs> (
						x => red.ValueChanged += x, 
						x => red.ValueChanged -= x
					)				
					.Select(result => (int)result.EventArgs.NewValue)
                    .StartWith(0);		

			var greenValueChangedObservable =
				Observable
					.FromEventPattern<EventHandler<ValueChangedEventArgs>, ValueChangedEventArgs> (
						x => green.ValueChanged += x, 
						x => green.ValueChanged -= x
					)
					.Select(result => (int)result.EventArgs.NewValue)
                    .StartWith(0);
			
			var blueValueChangedObservable =
				Observable
					.FromEventPattern<EventHandler<ValueChangedEventArgs>, ValueChangedEventArgs> (
						x => blue.ValueChanged += x, 
						x => blue.ValueChanged -= x
					)
					.Select(result => (int)result.EventArgs.NewValue)
                    .StartWith(0);


			Observable
				.CombineLatest (
                    redValueChangedObservable,
					greenValueChangedObservable,
					blueValueChangedObservable,
					(r, g, b) =>  Color.FromRgb (r, g, b)
				)
                .Do(x => System.Diagnostics.Debug.WriteLine($"current color: {x}"))
                .Subscribe (color => {
                    Device.BeginInvokeOnMainThread (() => colorDisplay.BackgroundColor = color);
                })
                .DisposeWith (SubscriptionDisposables);
		}
	}
}


