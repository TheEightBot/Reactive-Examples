using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Xamarin.Forms;
using System.Threading.Tasks;
using ReactiveUI.XamForms;

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

            this.WhenActivated((CompositeDisposable disposables) =>
            {
                this.Bind (ViewModel, vm => vm.Red, c => c.red.Value)
                    .DisposeWith(disposables);

                this.Bind (ViewModel, vm => vm.Green, c => c.green.Value)
                    .DisposeWith(disposables);
    
                this.Bind (ViewModel, vm => vm.Blue, c => c.blue.Value)
                    .DisposeWith(disposables);
    
                this.WhenAnyValue (x => x.ViewModel.Color)
                    .ObserveOn (RxApp.MainThreadScheduler)
                    .Select (color => Color.FromRgba (color.R, color.G, color.B, color.A))
                    .BindTo (this, x => x.colorDisplay.BackgroundColor)
                    .DisposeWith(disposables);
            });
		}
	}
}


