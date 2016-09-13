using System;
using System.Linq;
using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using ReactiveUI;
using ReactiveExtensionExamples.Utilities;
using System.Reactive.Disposables;

namespace ReactiveExtensionExamples.Pages
{
	public class ReactiveUiColorSlider: ReactiveContentPage<ViewModels.ColorSlider>
	{
		readonly CompositeDisposable subscriptionDisposables = new CompositeDisposable ();

		BoxView colorDisplay;

		Slider red, green, blue;

		public ReactiveUiColorSlider ()
		{
			ViewModel = new ReactiveExtensionExamples.ViewModels.ColorSlider ();

			Title = "RxUI - Color Slider";

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

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			this.Bind (ViewModel, vm => vm.Red, c => c.red.Value)
				.DisposeWith(subscriptionDisposables);

			this.Bind (ViewModel, vm => vm.Green, c => c.green.Value)
				.DisposeWith(subscriptionDisposables);

			this.Bind (ViewModel, vm => vm.Blue, c => c.blue.Value)
				.DisposeWith(subscriptionDisposables);

			this.WhenAnyValue (x => x.ViewModel.Color)
				.ObserveOn (RxApp.MainThreadScheduler)
				.Subscribe (color => {
					colorDisplay.BackgroundColor = Color.FromRgba (color.R, color.G, color.B, color.A);
				})
				.DisposeWith(subscriptionDisposables);
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();

			subscriptionDisposables.Clear ();
		}
	}
}


