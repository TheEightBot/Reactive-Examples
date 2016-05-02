using System;

using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using ReactiveUI;
using EightBot.ReactiveExtensionExamples.Utilities;
using System.Reactive;
using System.Collections.Generic;

namespace EightBot.ReactiveExtensionExamples.Pages
{
	public class Throttle : PageBase
	{
		Entry textEntry;
		StackLayout lastEntries;

		IObservable<string> textEntryObservable;

		protected override void SetupUserInterface ()
		{
			Title = "Rx - Throttle";

			Content = new StackLayout { 
				Padding = new Thickness(8d),
				Spacing = 16d,
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
					.Throttle (TimeSpan.FromSeconds (3), TaskPoolScheduler.Default)
					.Select(args => args.EventArgs.NewTextValue);
		}

		protected override void SetupReactiveSubscriptions ()
		{
			textEntryObservable
				.Subscribe (text => {
					Device.BeginInvokeOnMainThread(() => {
						lastEntries.Children
							.Insert(
								0, 
								new Label { Text = text });
						
						lastEntries.Children
							.Insert(
								1, 
								new Label { 
									Text = string.Format("Received at {0:H:mm:ss}", DateTime.Now), 
									FontAttributes = FontAttributes.Italic, 
									FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
									TextColor = Color.Gray
								});
						
						lastEntries.Children
							.Insert(
								2, 
								new BoxView { BackgroundColor = Color.Gray, HeightRequest = 2d });
					});
				})
				.DisposeWith(SubscriptionDisposables);
		}
	}
}


