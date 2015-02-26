using System;
using ReactiveUI;

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

		public Login ()
		{
		}
	}
}

