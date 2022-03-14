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

namespace ReactiveExtensionExamples.ViewModels.DynamicData
{
    public class SimpleDynamicDataViewModel : ViewModelBase
    {
        private SourceList<RssEntry> _searchResultSource = new SourceList<RssEntry>();

        [Reactive]
        public IEnumerable<RssEntry> SearchResults { get; private set; }

        [Reactive]
        public ReactiveCommand<Unit, Unit> Search { get; private set; }

        public SimpleDynamicDataViewModel ()
        {
            this.WhenActivated((CompositeDisposable disposables) =>
            {
                _searchResultSource =
                    new SourceList<RssEntry>()
                        .DisposeWith(disposables);

                _searchResultSource
                    .Connect()
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
                            var worldNews =
                                await RssDownloader
                                    .DownloadRss("https://www.reddit.com/r/worldnews/new/.rss", ct)
                                    .ConfigureAwait(false);

                            _searchResultSource
                                .Edit(
                                    innerList =>
                                    {
                                        innerList.Clear();
                                        innerList.AddRange(worldNews);
                                    });
                        })
                    .DisposeWith(disposables);
            });

        }
    }
}