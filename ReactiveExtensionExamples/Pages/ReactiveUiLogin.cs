using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.XamForms;
using Xamarin.Forms;

namespace ReactiveExtensionExamples.Pages
{
    public class ReactiveUiLogin : ReactiveContentPage<ViewModels.Login>
	{
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

            this.WhenActivated((CompositeDisposable disposables) =>
            {
                this.Bind (ViewModel, vm => vm.EmailAddress, c => c.emailEntry.Text)
                    .DisposeWith(disposables);
                
                this.Bind (ViewModel, vm => vm.Password, c => c.passwordEntry.Text)
                    .DisposeWith(disposables);
    
                this.OneWayBind (ViewModel, vm => vm.IsLoading, c => c.loading.IsRunning)
                    .DisposeWith(disposables);
                
                this.OneWayBind (ViewModel, vm => vm.IsLoading, c => c.loading.IsVisible)
                    .DisposeWith(disposables);
    
                this.BindCommand (ViewModel, vm => vm.PerformLogin, c => c.login)
                    .DisposeWith(disposables);
    
                this.WhenAnyObservable (x => x.ViewModel.PerformLogin)
                    .ObserveOn (RxApp.MainThreadScheduler)
                    .SelectMany (async _ => 
                    { 
                        await DisplayAlert("Log In", "It's Log, It's Log", "It's Big, It's Heavy, It's Wood"); 
                        return Unit.Default; 
                    })
                    .Subscribe()
                    .DisposeWith(disposables);            
            });
		}
	}
}


