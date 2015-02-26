using System;
using System.Linq;
using ReactiveUI;
using System.Text.RegularExpressions;
using System.Reactive;
using System.Threading.Tasks;

namespace EightBot.ReactiveExtensionExamples.ViewModels
{
	public class LoginWithCommand : ReactiveObject
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
			
		bool _isLoading;

		public bool IsLoading {
			get { return _isLoading; }
			set { this.RaiseAndSetIfChanged (ref _isLoading, value); }
		}

		ObservableAsPropertyHelper<bool> _isValid;
		public bool IsValid {
			get { return _isValid.Value; }
		}

		public ReactiveCommand<Unit> PerformLogin;

		public LoginWithCommand ()
		{

			PerformLogin = ReactiveCommand.CreateAsyncTask<Unit> (
				async _ => {
					try {
						IsLoading = true;
						await Task.Delay (2500) /* Fake Web Service Call */;
					} finally {
						IsLoading = false;
					}

					return Unit.Default;
				});

			this.WhenAnyValue (e => e.EmailAddress, p => p.Password,
				(emailAddress, password) =>
					/* Item 1 is our email address */
					(
						!string.IsNullOrEmpty(emailAddress)
							&&
						Regex.Matches(emailAddress, "^\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$").Count == 1
					)
					&&
					/* Item 2 is our password */
					(
						!string.IsNullOrEmpty(password) 
							&&
						password.Length > 5
					))
				.ToProperty(this, v => v.IsValid, out _isValid);
		}
	}
}

