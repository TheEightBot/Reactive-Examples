using System;
using Xamarin.Forms;

namespace EightBot.ReactiveExtensionExamples.Pages
{
	public class PageBase : ContentPage
	{
		public PageBase ()
		{
			SetupUserInterface ();

			SetupReactiveExtensions ();
		}

		protected virtual void SetupUserInterface () {}

		protected virtual void SetupReactiveExtensions () {}
	}
}

