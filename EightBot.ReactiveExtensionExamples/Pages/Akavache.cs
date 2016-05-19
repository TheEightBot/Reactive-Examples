using System;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Akavache;
using EightBot.ReactiveExtensionExamples.Utilities;
using ModernHttpClient;
using ReactiveUI;
using Xamarin.Forms;
using System.Collections.Generic;

namespace EightBot.ReactiveExtensionExamples.Pages
{
	public class Akavache : PageBase
	{
		Button download;
		ListView rssFeed;

		IObservable<EventPattern<object>> downloadClickedObservable;

		IDisposable akavacheQuery;

		protected override void SetupUserInterface ()
		{
			Title = "Rx - Akavache";

			download = new Button{ 
				Text = "Update RSS Feed",
			};
			download.SetDynamicResource(VisualElement.StyleProperty, Values.Styles.ReactiveButton);

			Content = new StackLayout { 
				Children = {
					download, 
					(rssFeed = new ListView { 
						ItemTemplate = new DataTemplate (typeof(RssEntryCell)),
						HasUnevenRows = true
					})
				}
			};
		}

		protected override void SetupReactiveObservables ()
		{
			downloadClickedObservable =
				Observable
					.FromEventPattern (x => download.Clicked += x, x => download.Clicked -= x);
		}

		protected override void SetupReactiveSubscriptions ()
		{
			downloadClickedObservable
				.Subscribe (args => {
					Device.BeginInvokeOnMainThread(() => download.IsEnabled = false);

					akavacheQuery = 
						BlobCache.InMemory
							.GetAndFetchLatest (
								"RssFeed",
								() => DownloadRss (),
								absoluteExpiration: DateTimeOffset.Now.AddSeconds (10d)
							)
							.Do (_ => System.Diagnostics.Debug.WriteLine ($"Received update at {DateTimeOffset.Now}"))
							.Subscribe(
								result => Device.BeginInvokeOnMainThread(() => rssFeed.ItemsSource = result),
								() => {
									Device.BeginInvokeOnMainThread(() => download.IsEnabled = true);
									akavacheQuery?.Dispose();
								});
				})
				.DisposeWith(SubscriptionDisposables);
		}

		async Task<List<RssEntry>> DownloadRss () {
			System.Diagnostics.Debug.WriteLine ($"Starting download at {DateTimeOffset.Now}");
			var client = new HttpClient(new NativeMessageHandler ());
			var rssStream = await client.GetStringAsync("https://www.reddit.com/.rss").ConfigureAwait(false);

			var parsedEntries =
				await Task.Run (
					() => {
						XNamespace ns = "http://www.w3.org/2005/Atom";
						var entries = 
							XDocument
								.Parse(rssStream)
								.Root
								.Descendants(ns + "entry");

						var rssEntries = 
							entries
								.Select(entry =>
									new RssEntry {
										Author = entry?.Element(ns + "author")?.Element(ns + "name")?.Value ?? string.Empty,
										Category = entry?.Element(ns + "category")?.Attribute(ns + "term")?.Value ?? string.Empty,
										Content = entry?.Element(ns + "content")?.Value ?? string.Empty,
										Updated = DateTimeOffset.Parse(entry?.Element(ns + "updated")?.Value).ToLocalTime().ToString("dd MMM yyyy hh:mm tts"),
										Title = entry?.Element(ns + "title")?.Value ?? string.Empty,
									}
								)
								.OrderByDescending(rssEntry => rssEntry.Updated)
								.ToList ();

						return rssEntries;
					})
					.ConfigureAwait(false);

			return parsedEntries;
		}

		class RssEntry {
			public string Author {
				get;
				set;
			}

			public string Category {
				get;
				set;
			}

			public string Content {
				get;
				set;
			}

			public string Updated {
				get;
				set;
			}

			public string Title {
				get;
				set;
			}
		}

		class RssEntryCell : ViewCell {
			public RssEntryCell () {
				var stackLayout = new StackLayout {};

				var title = new Label {
					Margin = new Thickness (8d)
				};
				title.SetBinding(Label.TextProperty, "Title");
				stackLayout.Children.Add(title);

				var updated = new Label {
					Margin = new Thickness (8d),
					FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
					FontAttributes = FontAttributes.Italic
				};
				updated.SetBinding(Label.TextProperty, "Updated");
				stackLayout.Children.Add(updated);

				View = stackLayout;
			}
		}
	}
}


