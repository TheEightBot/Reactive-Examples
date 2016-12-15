using System;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;
using System.Text.RegularExpressions;
using System.Reactive;
using System.Threading.Tasks;

namespace ReactiveExtensionExamples.ViewModels
{
	public class Login : ReactiveObject
	{
		string _emailAddress;
		public string EmailAddress {
			get { return _emailAddress; }
			set { this.RaiseAndSetIfChanged (ref _emailAddress, value); }
		}

		string _password;
		public string Password {
			get { return _password; }
			set { this.RaiseAndSetIfChanged (ref _password, value); }
		}
			
		ObservableAsPropertyHelper<bool> _isLoading;

		public bool IsLoading {
			get { return _isLoading?.Value ?? false; }
		}

		ObservableAsPropertyHelper<bool> _isValid;
		public bool IsValid {
			get { return _isValid?.Value ?? false; }
		}

		public ReactiveCommand<object, Unit> PerformLogin;

		public Login ()
		{
			this.WhenAnyValue (e => e.EmailAddress, p => p.Password,
				(emailAddress, password) =>
					/* Validate our email address */
					(
						!string.IsNullOrEmpty(emailAddress)
							&&
						Regex.Matches(emailAddress, "^\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$").Count == 1
					)
					&&
					/* Validate our password */
					(
						!string.IsNullOrEmpty(password) 
							&&
						password.Length > 5
					))
				.ToProperty(this, v => v.IsValid, out _isValid);

			var canExecuteLogin = 
				this.WhenAnyValue (x => x.IsLoading, x => x.IsValid, 
					(isLoading, IsValid) => !isLoading && IsValid)
				    .Do(x => System.Diagnostics.Debug.WriteLine($"Can Login: {x}"));
			
			PerformLogin = ReactiveCommand.CreateFromTask<object, Unit> (
				async _ => {
					var random = new Random(Guid.NewGuid().GetHashCode());
					await Task.Delay (random.Next(250, 10000)) /* Fake Web Service Call */;

					return Unit.Default;
				},
                canExecuteLogin);

            this.WhenAnyObservable(x => x.PerformLogin.IsExecuting)
                .StartWith(false)
				.ToProperty (this, x => x.IsLoading, out _isLoading);
		}
	}
}

