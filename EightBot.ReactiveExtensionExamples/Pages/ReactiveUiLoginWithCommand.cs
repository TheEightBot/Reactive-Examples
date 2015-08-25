using System;
using System.Linq;
using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using ReactiveUI;
using EightBot.ReactiveExtensionExamples.Utilities;

namespace EightBot.ReactiveExtensionExamples.Pages
{
	public class ReactiveUiLoginWithCommand: ReactiveContentPage<ViewModels.LoginWithCommand>
	{
		Entry emailEntry, passwordEntry;

		Button login;

		ActivityIndicator loading;

		public ReactiveUiLoginWithCommand ()
		{
			ViewModel = new EightBot.ReactiveExtensionExamples.ViewModels.LoginWithCommand ();

			Content = new StackLayout { 
				Padding = new Thickness(40d),
				Children = {
					(emailEntry = new Entry{ Placeholder = "Email" }),
					(passwordEntry = new Entry { Placeholder = "Password", IsPassword = true }),
					(loading = new ActivityIndicator{ Color = Color.Blue, HorizontalOptions = LayoutOptions.Center }),
					(login = new Button{ Text = "Login" })
				}
			};

			this.Bind (ViewModel, vm => vm.EmailAddress, c => c.emailEntry.Text);
			this.Bind (ViewModel, vm => vm.Password, c => c.passwordEntry.Text);

			this.OneWayBind (ViewModel, vm => vm.IsLoading, c => c.loading.IsRunning);
			this.OneWayBind (ViewModel, vm => vm.IsLoading, c => c.loading.IsVisible);

			this.OneWayBind (ViewModel, vm => vm.IsValid, c => c.login.IsEnabled);

			//this.BindCommand (ViewModel, vm => vm.PerformLogin, c => c.login);
			Observable.FromEventPattern (x => login.Clicked += x, x => login.Clicked -= x)
				.Subscribe (args => ViewModel.PerformLogin.Execute (null));

			this.WhenAnyObservable (x => x.ViewModel.PerformLogin)
				.ObserveOn (RxApp.MainThreadScheduler)
				.Subscribe (_ => DisplayAlert ("Log In", "It's Log, It's Log", "It's Big, It's Heavy, It's Wood"));
		}
	}
}


