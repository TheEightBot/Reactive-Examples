using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Xamarin.Forms;

namespace ReactiveExtensionExamples.Pages
{
    public class ReactiveUiLogin : ReactiveContentPage<ViewModels.Login>
	{
		readonly CompositeDisposable subscriptionDisposables = new CompositeDisposable ();

		Entry emailEntry, passwordEntry;

		Button login;

		ActivityIndicator loading;

		public ReactiveUiLogin ()
		{
			ViewModel = new ReactiveExtensionExamples.ViewModels.Login ();

			Title = "RxUI - Login";

			Content = new StackLayout { 
				Padding = new Thickness(40d),
				Children = {
					(emailEntry = new Entry{ Placeholder = "Email" }),
					(passwordEntry = new Entry { Placeholder = "Password", IsPassword = true }),
					(login = new Button{ Text = "Login" }),
					(loading = new ActivityIndicator{ HorizontalOptions = LayoutOptions.Center }),
				}
			};
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			this.Bind (ViewModel, vm => vm.EmailAddress, c => c.emailEntry.Text)
				.DisposeWith(subscriptionDisposables);
			
			this.Bind (ViewModel, vm => vm.Password, c => c.passwordEntry.Text)
				.DisposeWith(subscriptionDisposables);

			this.OneWayBind (ViewModel, vm => vm.IsLoading, c => c.loading.IsRunning)
				.DisposeWith(subscriptionDisposables);
			
			this.OneWayBind (ViewModel, vm => vm.IsLoading, c => c.loading.IsVisible)
				.DisposeWith(subscriptionDisposables);

			this.BindCommand (ViewModel, vm => vm.PerformLogin, c => c.login)
				.DisposeWith(subscriptionDisposables);

			this.WhenAnyObservable (x => x.ViewModel.PerformLogin)
				.ObserveOn (RxApp.MainThreadScheduler)
				.Subscribe (async _ => await DisplayAlert ("Log In", "It's Log, It's Log", "It's Big, It's Heavy, It's Wood"))
				.DisposeWith(subscriptionDisposables);
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();

			subscriptionDisposables.Clear ();
		}
	}
}


