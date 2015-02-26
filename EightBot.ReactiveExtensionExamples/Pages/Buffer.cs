using System;
using System.Linq;
using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using ReactiveUI;
using EightBot.ReactiveExtensionExamples.Utilities;

namespace EightBot.ReactiveExtensionExamples.Pages
{
	public class Buffer : ContentPage
	{
		Entry textEntry;
		Label delayedLabel;

		public Buffer ()
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
				.Buffer (TimeSpan.FromSeconds (2.5))
				.Dump("Values")
				.ObserveOn (RxApp.MainThreadScheduler)
				.Subscribe (argsList => {
					if(argsList.Any())
						delayedLabel.Text = argsList.LastOrDefault().EventArgs.NewTextValue;
				});

		}
	}
}


