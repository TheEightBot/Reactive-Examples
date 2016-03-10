using System;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;
using System.Text.RegularExpressions;
using System.Reactive;
using System.Threading.Tasks;

namespace EightBot.ReactiveExtensionExamples.ViewModels
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

		public Login ()
		{

			PerformLogin = ReactiveCommand.CreateAsyncTask<Unit> (
				this.WhenAnyValue(
					x => x.IsLoading, 
					x => x.IsValid, 
					(isLoading, IsValid) => !isLoading && IsValid),
				async _ => {
					try {
						IsLoading = true;
						var random = new Random();
						await Task.Delay (random.Next(250, 10000)) /* Fake Web Service Call */;
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

