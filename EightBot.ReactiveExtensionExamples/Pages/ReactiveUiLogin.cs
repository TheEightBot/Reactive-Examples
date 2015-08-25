using System;
using System.Linq;
using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using ReactiveUI;
using EightBot.ReactiveExtensionExamples.Utilities;
using System.Threading.Tasks;

namespace EightBot.ReactiveExtensionExamples.Pages
{
	public class ReactiveUiLogin: ContentPage, IViewFor<ViewModels.Login>
	{
		Entry emailEntry, passwordEntry;

		Button login;

		public ReactiveUiLogin ()
		{
			ViewModel = new EightBot.ReactiveExtensionExamples.ViewModels.Login ();

			Content = new StackLayout { 
				Padding = new Thickness(40d),
				Children = {
					(emailEntry = new Entry{ Placeholder = "Email" }),
					(passwordEntry = new Entry { Placeholder = "Password", IsPassword = true }),
					(login = new Button{ Text = "Login" })
				}
			};

			this.Bind (ViewModel, vm => vm.EmailAddress, c => c.emailEntry.Text);

			this.Bind (ViewModel, vm => vm.Password, c => c.passwordEntry.Text);


			Task.Run (async () => {
				
				await Task.Delay(2000);
				Xamarin.Forms.Device.BeginInvokeOnMainThread(() => 
					ViewModel.EmailAddress = "test@test.com");
			});
		}

		public static readonly BindableProperty ViewModelProperty = 
			BindableProperty.Create<ReactiveUiLogin, ViewModels.Login>(x => x.ViewModel, null, BindingMode.OneWay);

		#region IViewFor implementation

		public EightBot.ReactiveExtensionExamples.ViewModels.Login ViewModel {
			get {
				return (ViewModels.Login)GetValue (ViewModelProperty);
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
				ViewModel = (ViewModels.Login)value;
			}
		}

		#endregion
	}
}


