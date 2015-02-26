using System;
using System.Linq;
using ReactiveUI;
using System.Text.RegularExpressions;

namespace EightBot.ReactiveExtensionExamples.ViewModels
{
	public class LoginWithValidationWithProperty : ReactiveObject
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
			
		ObservableAsPropertyHelper<bool> _isValid;
		public bool IsValid {
			get { return _isValid.Value; }
		}

		public LoginWithValidationWithProperty ()
		{

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

