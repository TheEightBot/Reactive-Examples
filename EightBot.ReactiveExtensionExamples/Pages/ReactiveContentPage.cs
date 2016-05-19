using System;
using Xamarin.Forms;
using ReactiveUI;

namespace EightBot.ReactiveExtensionExamples.Pages
{
	public abstract class ReactiveContentPage<TViewModel> : ContentPage, IViewFor<TViewModel>
		where TViewModel : class
	{
		public ReactiveContentPage ()
		{
		}

		public static readonly BindableProperty ViewModelProperty = 
			BindableProperty.Create(nameof(ViewModels), typeof(TViewModel), typeof(ReactiveContentPage<TViewModel>), default(TViewModel), BindingMode.OneWay);

		#region IViewFor implementation

		public TViewModel ViewModel {
			get {
				return (TViewModel)GetValue (ViewModelProperty);
			}
			set {
				SetValue (ViewModelProperty, value);
			}
		}

		#endregion

		#region IViewFor implementation

		object IViewFor.ViewModel {
			get {
				return ViewModel;
			}
			set {
				ViewModel = (TViewModel)value;
			}
		}

		#endregion
	}
}

