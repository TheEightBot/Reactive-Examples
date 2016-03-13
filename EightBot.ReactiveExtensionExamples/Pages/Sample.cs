using System;
using System.Linq;
using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using ReactiveUI;
using EightBot.ReactiveExtensionExamples.Utilities;
using System.Reactive;

namespace EightBot.ReactiveExtensionExamples.Pages
{
	public class Sample : PageBase
	{
		Entry textEntry;
		WebView webView;

		IObservable<EventPattern<WebNavigatingEventArgs>>
			webViewNavigatingObservable;

		IObservable<EventPattern<WebNavigatedEventArgs>>
			webViewNavigatedObservable;

		IObservable<string>
			textEntryObservable;

		protected override void SetupUserInterface ()
		{
			Title = "Rx - Sample";

			Content = new StackLayout { 
				Padding = new Thickness(8d),
				Children = {
					(textEntry = new Entry{ Placeholder = "Enter Search Terms" }),
					(webView = new WebView {
						VerticalOptions = LayoutOptions.FillAndExpand, 
						HorizontalOptions = LayoutOptions.FillAndExpand,
					})
				}
			};
		}

		protected override void SetupReactiveObservables ()
		{
			webViewNavigatingObservable =
				Observable
					.FromEventPattern<EventHandler<WebNavigatingEventArgs>, WebNavigatingEventArgs> (
						x => webView.Navigating += x, 
						x => webView.Navigating -= x
					);

			webViewNavigatedObservable =
				Observable
					.FromEventPattern<EventHandler<WebNavigatedEventArgs>, WebNavigatedEventArgs> (
						x => webView.Navigated += x, 
						x => webView.Navigated -= x
					);

			textEntryObservable =
				Observable
					.FromEventPattern<EventHandler<TextChangedEventArgs>, TextChangedEventArgs> (
						x => textEntry.TextChanged += x, 
						x => textEntry.TextChanged -= x
					)
					.Sample (TimeSpan.FromSeconds (3))
					.Select(args => 
						string.Format("https://frinkiac.com/?q={0}", args.EventArgs.NewTextValue.Replace(" ", "+"))
					);
		}

		protected override void SetupReactiveSubscriptions ()
		{
			webViewNavigatingObservable
				.ObserveOn (RxApp.MainThreadScheduler)
				.Subscribe (_ => webView.FadeTo(0.5d))
				.DisposeWith(SubscriptionDisposables);

			webViewNavigatedObservable
				.ObserveOn (RxApp.MainThreadScheduler)
				.Subscribe (_ => webView.FadeTo(1d))
				.DisposeWith(SubscriptionDisposables);

			textEntryObservable
				.ObserveOn (RxApp.MainThreadScheduler)
				.Subscribe (searchUrl => {
					try {
						webView.Source = searchUrl;
					} catch (Exception) {
						webView.Source = "https://frinkiac.com/caption/S04E05/1135500";
					}
				})
				.DisposeWith(SubscriptionDisposables);
		}
	}
}


