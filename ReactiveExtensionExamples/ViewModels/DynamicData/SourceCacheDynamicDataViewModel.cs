using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ReactiveExtensionExamples.Services;
using ReactiveExtensionExamples.Services.Api;
using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using ReactiveUI.Legacy;
using DynamicData;
using System.ComponentModel;
using ReactiveExtensionExamples.Models;
using System.Threading;
using System.Net.Http;
using ReactiveUI.Fody.Helpers;
using DynamicData.Aggregation;
using DynamicData.Binding;

namespace ReactiveExtensionExamples.ViewModels.DynamicData
{
    public class SourceCacheDynamicDataViewModel : ViewModelBase
    {
        [Reactive]
        public string SearchQuery { get; set; }

        private SourceCache<RssEntry, string> _searchResultSource;

        [Reactive]
        public IEnumerable<RssEntry> SearchResults { get; private set; }

        [Reactive]
        public int ResultCount { get; set; }

        [Reactive]
        public ReactiveCommand<Unit, Unit> Search { get; private set; }

        public SourceCacheDynamicDataViewModel ()
        {
            this.WhenActivated((CompositeDisposable disposables) =>
            {
                _searchResultSource = new SourceCache<RssEntry, string>(x => x.Id).DisposeWith(disposables);

                _searchResultSource
                    .Connect()
                    .SubscribeOn(RxApp.TaskpoolScheduler)
                    .OnItemAdded(x => x.New = true)
                    .OnItemUpdated((current, previous) => current.New = false)
                    .Sort(SortExpressionComparer<RssEntry>.Descending(x => x.New).ThenByDescending(x => x.Updated))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(out var searchResultsBinding)
                    .Subscribe()
                    .DisposeWith(disposables);

                this.SearchResults = searchResultsBinding;

                Search =
                    ReactiveCommand
                    .CreateFromTask(
                        async (ct) =>
                        {
                            var rss = await RssDownloader.DownloadRss("https://www.reddit.com/r/worldnews/new/.rss", ct).ConfigureAwait(false);

                            _searchResultSource.AddOrUpdate(rss);
                        })
                    .DisposeWith(disposables);

                Observable
                    .Interval(TimeSpan.FromSeconds(5))
                    .SelectUnit()
                    .InvokeCommand(this, x => x.Search)
                    .DisposeWith(disposables);
            });

        }
    }
}