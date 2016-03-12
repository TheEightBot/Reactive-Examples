using System;

using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using ReactiveUI;
using EightBot.ReactiveExtensionExamples.Utilities;
using System.Collections.Generic;
using System.Reactive;

namespace EightBot.ReactiveExtensionExamples.Pages
{
	public class Delay : PageBase
	{
		Entry textEntry;
		StackLayout lastEntries;

		IObservable<EventPattern<TextChangedEventArgs>> textEntryObservable;

		protected override void SetupUserInterface ()
		{
			Title = "Rx - Delay";

			Content = new StackLayout { 
				Padding = new Thickness(40d),
				Children = {
					(textEntry = new Entry{ Placeholder = "Enter Some Text" }),
					new ScrollView { 
						VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand,
						Content = (lastEntries = new StackLayout{
							VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand
						})
					}
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
					.Delay (TimeSpan.FromSeconds (2.5));
		}

		protected override void SetupReactiveSubscriptions ()
		{
			textEntryObservable
				.ObserveOn (RxApp.MainThreadScheduler)
				.Subscribe (args => 
					lastEntries.Children
					.Insert(
						0, 
						new Label { Text = args.EventArgs.NewTextValue })
				)
				.DisposeWith(SubscriptionDisposables);
		}
	}
}


