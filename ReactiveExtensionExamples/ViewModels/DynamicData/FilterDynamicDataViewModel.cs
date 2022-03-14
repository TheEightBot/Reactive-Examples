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

namespace ReactiveExtensionExamples.ViewModels.DynamicData
{
    public class FilterDynamicDataViewModel : ViewModelBase
    {
        [Reactive]
        public string SearchQuery { get; set; }

        private SourceList<RssEntry> _searchResultSource = new SourceList<RssEntry>();

        [Reactive]
        public IEnumerable<RssEntry> SearchResults { get; private set; }

        [Reactive]
        public int ResultCount { get; set; }

        [Reactive]
        public ReactiveCommand<Unit, Unit> Search { get; private set; }

        [Reactive]
        public ReactiveCommand<Unit, Unit> CancelSearch { get; private set; }


        public FilterDynamicDataViewModel ()
        {
            this.WhenActivated((CompositeDisposable disposables) =>
            {
                _searchResultSource = new SourceList<RssEntry>().DisposeWith(disposables);

                var filter =
                    this.WhenAnyValue(x => x.SearchQuery)
                        .SubscribeOn(RxApp.TaskpoolScheduler)
                        .Select(
                            search =>
                            {
                                var searchIsEmpty = string.IsNullOrEmpty(search);

                                return new Func<RssEntry, bool>(
                                    value =>
                                    {
                                        if (searchIsEmpty)
                                        {
                                            return true;
                                        }

                                        return FuzzySharp.Fuzz.PartialRatio(value.Title, search) > 75;
                                    });
                            });

                var filteredData =
                    _searchResultSource
                        .Connect()
                        .SubscribeOn(RxApp.MainThreadScheduler)
                        .Filter(filter)
                        .Publish()
                        .RefCount();

                filteredData
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(out var searchResultsBinding)
                    .Subscribe()
                    .DisposeWith(disposables);

                this.SearchResults = searchResultsBinding;

                filteredData
                    .Count()
                    .BindTo(this, x => x.ResultCount)
                    .DisposeWith(disposables);

                CancelSearch = 
                    ReactiveCommand
                        .Create(() => {})
                        .DisposeWith(disposables);

                Search =
                    ReactiveCommand
                        .CreateFromObservable(
                            () =>
                            {
                                return Observable
                                    .StartAsync(
                                        async (ct) =>
                                        {
                                            var worldNews = await RssDownloader.DownloadRss("https://www.reddit.com/r/worldnews/new/.rss", ct).ConfigureAwait(false);

                                            if(ct.IsCancellationRequested)
                                            {
                                                return;
                                            }

                                            _searchResultSource
                                                .Edit(
                                                    innerList =>
                                                    {
                                                        innerList.Clear();
                                                        innerList.AddRange(worldNews);
                                                    });
                                        })
                                    .TakeUntil(this.CancelSearch)
                                    .SelectUnit();
                            })
                    .DisposeWith(disposables);

                Observable
                    .Merge(
                        this.ThrownExceptions,
                        Search.ThrownExceptions)
                    .Do(ex => System.Diagnostics.Debug.WriteLine($"ERROR!!: {ex}"));
            });

        }
    }
}