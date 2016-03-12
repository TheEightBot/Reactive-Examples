using System;
using System.Linq;
using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using ReactiveUI;
using EightBot.ReactiveExtensionExamples.Utilities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reactive;

namespace EightBot.ReactiveExtensionExamples.Pages
{
	public class BufferWithWhere : PageBase
	{
		Entry textEntry;
		StackLayout lastEntries;

		IObservable<IList<EventPattern<TextChangedEventArgs>>> textEntryObservable;

		protected override void SetupUserInterface ()
		{
			Title = "Rx - Buffer with Filtering Where";

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
					.Buffer (TimeSpan.FromSeconds (2.5))
					.Where (argsList => argsList != null && argsList.Any ());
		}

		protected override void SetupReactiveSubscriptions ()
		{
			textEntryObservable
				.ObserveOn (RxApp.MainThreadScheduler)
				.Subscribe (async argsList => {
					var processedValues = 
						await Task.Run (() => 
							string.Join(
								Environment.NewLine, 
								argsList.Select(args => args.EventArgs.NewTextValue).Reverse().ToList()
							)
						);

					if(argsList.Any())
						lastEntries.Children.Insert(
							0, 
							new Label { Text = processedValues }
						);
				})
				.DisposeWith(SubscriptionDisposables);
		}
	}
}


