using System;
using System.Linq;
using ReactiveUI;
using System.Text.RegularExpressions;

namespace EightBot.ReactiveExtensionExamples.ViewModels
{
	public class LoginWithValidation : ReactiveObject
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

		bool _isValid = false;
		public bool IsValid {
			get { return _isValid; }
			set { this.RaiseAndSetIfChanged (ref _isValid, value); }
		}

		public LoginWithValidation ()
		{

			this.WhenAnyValue (e => e.EmailAddress, p => p.Password)
				.Subscribe ((emailAddressAndPassword) => {
					IsValid = 
						/* Item 1 is our email address */
						(
							!string.IsNullOrEmpty(emailAddressAndPassword.Item1)
							&&
							Regex.Matches(emailAddressAndPassword.Item1, "^\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$").Count == 1
						)
						&&
						/* Item 2 is our password */
						(
							!string.IsNullOrEmpty(emailAddressAndPassword.Item2) 
							&&
							emailAddressAndPassword.Item2.Length > 5
						);

				});
		}
	}
}

