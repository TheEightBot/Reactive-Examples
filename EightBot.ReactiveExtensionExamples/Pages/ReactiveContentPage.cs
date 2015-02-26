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
			BindableProperty.Create<ReactiveContentPage<TViewModel>, TViewModel>(x => x.ViewModel, null, BindingMode.OneWay);

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

