using System;
using System.Linq;
using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using ReactiveUI;
using EightBot.ReactiveExtensionExamples.Utilities;

namespace EightBot.ReactiveExtensionExamples.Pages
{
	public class ReactiveUiLoginWithValidation: ReactiveContentPage<ViewModels.LoginWithValidation>
	{
		Entry emailEntry, passwordEntry;

		Button login;

		public ReactiveUiLoginWithValidation ()
		{
			ViewModel = new EightBot.ReactiveExtensionExamples.ViewModels.LoginWithValidation ();

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
			this.Bind (ViewModel, vm => vm.IsValid, c => c.login.IsEnabled);
		}
	}
}


