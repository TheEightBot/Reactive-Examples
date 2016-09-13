using System;
using System.Linq;
using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using ReactiveUI;
using ReactiveExtensionExamples.Utilities;
using System.Threading.Tasks;
using System.Reactive;
using System.Collections.Generic;
using System.Diagnostics;

namespace ReactiveExtensionExamples.Pages
{
	public class Buffer : PageBase
	{
		Entry textEntry;
		StackLayout lastEntries;

		IObservable<string> textEntryObservable;

		protected override void SetupUserInterface ()
		{
			Title = "Rx - Buffer";

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
				.Buffer (TimeSpan.FromSeconds (3), TaskPoolScheduler.Default)
				.Select(argsList => 
					string.Join(
						Environment.NewLine, 
						argsList.Select(args => args.EventArgs.NewTextValue).Reverse().ToList()
					)
				);
		}

		protected override void SetupReactiveSubscriptions ()
		{
			textEntryObservable
				.Subscribe (text => {
					Device.BeginInvokeOnMainThread(() => {
						lastEntries.Children.Insert(
							0, 
							new Label { Text = text }
						);

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


