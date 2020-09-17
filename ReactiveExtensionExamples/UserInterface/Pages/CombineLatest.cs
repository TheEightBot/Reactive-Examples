using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xamarin.Forms;

namespace ReactiveExtensionExamples.UserInterface.Pages
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

			Observable
				.CombineLatest (
                    red
                        .Events()
                        .ValueChanged
                        .Select(result => (int)result.NewValue),
                    green
                        .Events()
                        .ValueChanged
                        .Select(result => (int)result.NewValue)
                        .StartWith(0),
                    blue
                        .Events()
                        .ValueChanged
                        .Select(result => (int)result.NewValue)
                        .StartWith(0),
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


