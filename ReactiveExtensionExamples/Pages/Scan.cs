using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;
using System.Threading;

namespace ReactiveExtensionExamples.Pages
{
    public class Scan : PageBase
	{
		Button download;
		ListView rssFeed;
        
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

		protected override void SetupReactiveExtensions ()
		{
			Observable
                .Interval(TimeSpan.FromMilliseconds(300))
				.StartWith(0L)
				.Select(_ => Observable.FromAsync(cancellationToken => DownloadMultipleRss(cancellationToken)))
                .Switch()
				.Scan(new List<RssEntry>(), 
					(accumulatedItems, newItems) => 
                    {
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
					})
                .Where(x => x?.Any(rss => rss.New) ?? false)
                .Subscribe(result => Device.BeginInvokeOnMainThread(() => rssFeed.ItemsSource = result))
                .DisposeWith(SubscriptionDisposables);
		}

		Task<IEnumerable<RssEntry>> DownloadMultipleRss(CancellationToken ct){
            return Task.Run<IEnumerable<RssEntry>>(async () =>
            {

                System.Diagnostics.Debug.WriteLine($"Starting download at {DateTimeOffset.Now}");

                var askReddit = DownloadRss("https://www.reddit.com/r/AskReddit/new/.rss", ct);
                var todayILearned = DownloadRss("https://www.reddit.com/r/todayilearned/new/.rss", ct);
                var news = DownloadRss("https://www.reddit.com/r/news/new/.rss", ct);
                var worldNews = DownloadRss("https://www.reddit.com/r/worldnews/new/.rss", ct);

                var tcs = new TaskCompletionSource<object>();

                ct.Register(() => tcs.TrySetCanceled(), false);

                await Task.WhenAny(Task.WhenAll(askReddit, todayILearned, news, worldNews), tcs.Task).ConfigureAwait(false);

                var masterList = new List<RssEntry>();

                if (!ct.IsCancellationRequested && !tcs.Task.IsCanceled)
                { 
                    masterList.AddRange (await askReddit.ConfigureAwait(false));
                    masterList.AddRange (await todayILearned.ConfigureAwait(false));
                    masterList.AddRange (await news.ConfigureAwait(false));
                    masterList.AddRange (await worldNews.ConfigureAwait(false));
                }
                
                var filteredList = masterList.GroupBy (x => x.Id).Select (x => x.First ()).ToList ();

                if (ct.IsCancellationRequested)
                {
                    System.Diagnostics.Debug.WriteLine ($"Cancelled download at {DateTimeOffset.Now}");
                    return Enumerable.Empty<RssEntry>();
                }

                return filteredList;
			});
		}

		async Task<IEnumerable<RssEntry>> DownloadRss (string url, CancellationToken ct) {
			var rssStreamResponse = await client.GetAsync(url, ct).ConfigureAwait(false);

            if(!ct.IsCancellationRequested && rssStreamResponse.IsSuccessStatusCode)
            {
                var rssStream = await rssStreamResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                
                if(!ct.IsCancellationRequested)
                {
                    return
                        await Task.Run (() => 
                        {
                            XNamespace ns = "http://www.w3.org/2005/Atom";
                            
                            var entries = 
                                XDocument
                                    .Parse(rssStream)
                                    .Root
                                    .Descendants(ns + "entry");
                            
                                return entries
                                    .Select(entry =>
                                            new RssEntry {
                                    Id = entry?.Element(ns + "id")?.Value ?? string.Empty,
                                    Author = entry?.Element(ns + "author")?.Element(ns + "name")?.Value ?? string.Empty,
                                    Category = entry?.Element(ns + "category")?.Attribute("label")?.Value ?? string.Empty,
                                    Content = entry?.Element(ns + "content")?.Value ?? string.Empty,
                                    Updated = DateTimeOffset.Parse(entry?.Element(ns + "updated")?.Value).ToLocalTime().ToString("dd MMM yyyy hh:mm tt"),
                                    Title = entry?.Element(ns + "title")?.Value ?? string.Empty
                                })
                                .OrderByDescending(rssEntry => rssEntry.Updated)
                                .ToList ();
                        }, ct)
                        .ConfigureAwait(false);
                }
            }

            return Enumerable.Empty<RssEntry>();
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


