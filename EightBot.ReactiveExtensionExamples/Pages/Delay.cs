using System;

using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using ReactiveUI;
using EightBot.ReactiveExtensionExamples.Utilities;

namespace EightBot.ReactiveExtensionExamples.Pages
{
	public class Delay : ContentPage
	{
		Entry textEntry;
		Label delayedLabel;

		public Delay ()
		{
			Content = new StackLayout { 
				Padding = new Thickness(40d),
				Children = {
					(textEntry = new Entry{ Placeholder = "Enter Some Text" }),
					(delayedLabel = new Label { XAlign = TextAlignment.Center})
				}
			};

			Observable
				.FromEventPattern<EventHandler<TextChangedEventArgs>, TextChangedEventArgs> (
					x => textEntry.TextChanged += x, 
					x => textEntry.TextChanged -= x)
				.Delay (TimeSpan.FromSeconds (2.5))
				.Dump("Values")
				.ObserveOn (RxApp.MainThreadScheduler)
				.Subscribe (args => delayedLabel.Text = args.EventArgs.NewTextValue);

		}
	}
}


