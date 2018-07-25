using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Xamarin.Forms;
using System.Threading.Tasks;
using ReactiveUI.XamForms;

namespace ReactiveExtensionExamples.Pages
{
    public class ReactiveUiEssentials : ReactiveContentPage<ViewModels.XamarinEssentials>
	{
		BoxView colorDisplay;

		Slider x, y, z;

		public ReactiveUiEssentials ()
		{
			ViewModel = new ReactiveExtensionExamples.ViewModels.XamarinEssentials ();

			Title = "RxUI - Xamarin Essentials";

			Content = new StackLayout { 
				Padding = new Thickness(40d),
				Children = {
					(colorDisplay = new BoxView{ HeightRequest = 250 }),

					new Label{ Text = "X"},
					(x = new Slider(-1, 1, 0){ InputTransparent = true }),

					new Label{ Text = "Y"},
					(y = new Slider(-1, 1, 0){ InputTransparent = true }),

					new Label{ Text = "Z"},
					(z = new Slider(-1, 1, 0){ InputTransparent = true })
				}
			};

            this.WhenActivated((CompositeDisposable disposables) =>
            {
                this.OneWayBind (ViewModel, vm => vm.X, c => c.x.Value)
                    .DisposeWith(disposables);

                this.OneWayBind (ViewModel, vm => vm.Y, c => c.y.Value)
                    .DisposeWith(disposables);
    
                this.OneWayBind (ViewModel, vm => vm.Z, c => c.z.Value)
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


