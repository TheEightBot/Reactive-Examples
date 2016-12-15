using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;

namespace ReactiveExtensionExamples.Pages
{
    public class Scan : PageBase
	{
		Button download;
		ListView rssFeed;

		IObservable<List<RssEntry>> itemsObservable;

		IDisposable akavacheQuery;

        readonly HttpClient client = new HttpClient();

		protected override void SetupUserInterface ()
		{
			Title = "Rx - Scan";


			Content = new StackLayout { 
				Children = {
					(rssFeed = new ListView { 
						ItemTemplate = new DataTemplate (typeof(RssEntryCell)),
						HasUnevenRows = true
					})
				}
			};
		}

		protected override void SetupReactiveObservables ()
		{
			itemsObservable = 
				Observable.Interval(TimeSpan.FromSeconds(10))
					.StartWith(0L)
					.SelectMany(async _ => await DownloadMultipleRss())
					.Scan(new List<RssEntry>(), 
						(accumulatedItems, newItems) => {
							var itemsToAdd = 
								newItems
									.Where(x => !accumulatedItems.Any(ai => ai.Id.Equals(x.Id)))
									.Select(x => { x.New = true; return x;})
									.ToList();

							foreach (var item in accumulatedItems)
								item.New = false;
							
							accumulatedItems.InsertRange(0, itemsToAdd);

							return
								accumulatedItems
									.OrderByDescending(x => x.New)
									.ThenByDescending(x => x.Updated)
									.Take(250)
									.ToList(); 
						});
		}

		protected override void SetupReactiveSubscriptions ()
		{
			itemsObservable
				.Subscribe(result => Device.BeginInvokeOnMainThread(() => rssFeed.ItemsSource = result))
				.DisposeWith(SubscriptionDisposables);
		}

		async Task<List<RssEntry>> DownloadMultipleRss(){
			var askReddit = DownloadRss ("https://www.reddit.com/r/AskReddit/new/.rss");
			var todayILearned = DownloadRss ("https://www.reddit.com/r/todayilearned/new/.rss");
			var news = DownloadRss ("https://www.reddit.com/r/news/new/.rss");
			var worldNews = DownloadRss ("https://www.reddit.com/r/worldnews/new/.rss");

			await Task.WhenAll (askReddit, todayILearned, news, worldNews).ConfigureAwait(false);

			return await Task.Run(() => {
				var masterList = new List<RssEntry> ();
				masterList.AddRange (askReddit.Result);
				masterList.AddRange (todayILearned.Result);
				masterList.AddRange (news.Result);
				masterList.AddRange (worldNews.Result);

				return masterList.GroupBy (x => x.Id).Select (x => x.First ()).ToList ();
			});
		}

		async Task<List<RssEntry>> DownloadRss (string url) {
			System.Diagnostics.Debug.WriteLine ($"Starting download at {DateTimeOffset.Now}");

			var rssStream = await client.GetStringAsync(url).ConfigureAwait(false);

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
										Id = entry?.Element(ns + "id")?.Value ?? string.Empty,
										Author = entry?.Element(ns + "author")?.Element(ns + "name")?.Value ?? string.Empty,
										Category = entry?.Element(ns + "category")?.Attribute("label")?.Value ?? string.Empty,
										Content = entry?.Element(ns + "content")?.Value ?? string.Empty,
										Updated = DateTimeOffset.Parse(entry?.Element(ns + "updated")?.Value).ToLocalTime().ToString("dd MMM yyyy hh:mm tt"),
										Title = entry?.Element(ns + "title")?.Value ?? string.Empty
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

			public string Id {
				get;
				set;
			}

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

			public bool New {
				get;
				set;
			}
		}

		class RssEntryCell : ViewCell {
			public RssEntryCell () {
				var stackLayout = new StackLayout {
					Padding = new Thickness(8d),
					Spacing = 4d
				};

				var newFlag = new Label {
					Text = "New",
					FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
					TextColor = Color.Accent,
					FontAttributes = FontAttributes.Italic
				};
				newFlag.SetBinding(Label.IsVisibleProperty, "New");
				stackLayout.Children.Add(newFlag);

				var title = new Label {
				};
				title.SetBinding(Label.TextProperty, "Title");
				stackLayout.Children.Add(title);

				var category = new Label {
					FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
					TextColor = Color.Gray
				};
				category.SetBinding(Label.TextProperty, "Category");
				stackLayout.Children.Add(category);

				var updated = new Label {
					FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
					FontAttributes = FontAttributes.Italic,
					TextColor = Color.Gray
				};
				updated.SetBinding(Label.TextProperty, "Updated");
				stackLayout.Children.Add(updated);

				var padding = new ContentView {};
				stackLayout.Children.Add(padding);

				View = stackLayout;
			}
		}
	}
}


