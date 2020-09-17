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
using ReactiveExtensionExamples.Models;
using ReactiveExtensionExamples.Services.Api;

namespace ReactiveExtensionExamples.UserInterface.Pages
{
    public class Scan : PageBase
	{
		CollectionView rssFeed;

        readonly HttpClient client = new HttpClient();

		protected override void SetupUserInterface ()
		{
			Title = "Rx - Scan";


			Content = new StackLayout { 
				Children = {
					(rssFeed = new CollectionView { 
						ItemTemplate = new DataTemplate (typeof(Cells.RssEntryCell)),
						ItemSizingStrategy = ItemSizingStrategy.MeasureAllItems,
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

                var askReddit = RssDownloader.DownloadRss("https://www.reddit.com/r/AskReddit/new/.rss", ct);
                var todayILearned = RssDownloader.DownloadRss("https://www.reddit.com/r/todayilearned/new/.rss", ct);
                var news = RssDownloader.DownloadRss("https://www.reddit.com/r/news/new/.rss", ct);
                var worldNews = RssDownloader.DownloadRss("https://www.reddit.com/r/worldnews/new/.rss", ct);

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
	}
}


