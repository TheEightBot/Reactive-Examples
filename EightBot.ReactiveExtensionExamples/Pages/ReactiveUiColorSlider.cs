using System;
using System.Linq;
using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using ReactiveUI;
using EightBot.ReactiveExtensionExamples.Utilities;

namespace EightBot.ReactiveExtensionExamples.Pages
{
	public class ReactiveUiColorSlider: ReactiveContentPage<ViewModels.ColorSlider>
	{
		BoxView colorDisplay;

		Slider red, green, blue;


		public ReactiveUiColorSlider ()
		{
			ViewModel = new EightBot.ReactiveExtensionExamples.ViewModels.ColorSlider ();

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

			this.Bind (ViewModel, vm => vm.Red, c => c.red.Value);
			this.Bind (ViewModel, vm => vm.Green, c => c.green.Value);
			this.Bind (ViewModel, vm => vm.Blue, c => c.blue.Value);

			this.WhenAnyValue (x => x.ViewModel.Color)
				.ObserveOn (RxApp.MainThreadScheduler)
				.Subscribe (color => {
					colorDisplay.BackgroundColor = Color.FromRgba(color.R, color.G, color.B, color.A);
				});
		}
	}
}


