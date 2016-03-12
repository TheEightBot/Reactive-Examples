using System;
using Xamarin.Forms;
using System.Reactive.Disposables;

namespace EightBot.ReactiveExtensionExamples.Pages
{
	public class PageBase : ContentPage
	{
		protected readonly CompositeDisposable SubscriptionDisposables = new CompositeDisposable ();

		public PageBase ()
		{
			SetupUserInterface ();

			SetupReactiveObservables ();
		}

		protected virtual void SetupUserInterface () {}

		protected virtual void SetupReactiveObservables () {}

		protected virtual void SetupReactiveSubscriptions () {}

		protected override void OnAppearing ()
		{
			SetupReactiveSubscriptions ();

			base.OnAppearing ();
		}

		protected override void OnDisappearing ()
		{
			SubscriptionDisposables.Clear ();

			base.OnDisappearing ();
		}
	}
}

