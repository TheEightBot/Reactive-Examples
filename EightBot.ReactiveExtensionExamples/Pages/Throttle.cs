using System;

using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using ReactiveUI;
using EightBot.ReactiveExtensionExamples.Utilities;
using System.Reactive;

namespace EightBot.ReactiveExtensionExamples.Pages
{
	public class Throttle : PageBase
	{
		Entry textEntry;
		Label delayedLabel;

		IObservable<EventPattern<TextChangedEventArgs>> textEntryObservable;

		protected override void SetupUserInterface ()
		{
			Content = new StackLayout { 
				Padding = new Thickness(40d),
				Children = {
					(textEntry = new Entry{ Placeholder = "Enter Some Text" }),
					(delayedLabel = new Label { HorizontalTextAlignment = TextAlignment.Center})
				}
			};
		}

		protected override void SetupReactiveObservables ()
		{
			textEntryObservable =
				Observable
					.FromEventPattern<EventHandler<TextChangedEventArgs>, TextChangedEventArgs> (
						x => textEntry.TextChanged += x, 
						x => textEntry.TextChanged -= x
					)
					.Throttle (TimeSpan.FromSeconds (2.5));
		}

		protected override void SetupReactiveSubscriptions ()
		{
			textEntryObservable
				.ObserveOn (RxApp.MainThreadScheduler)
				.Subscribe (args => delayedLabel.Text = args.EventArgs.NewTextValue)
				.DisposeWith(SubscriptionDisposables);
		}
	}
}


