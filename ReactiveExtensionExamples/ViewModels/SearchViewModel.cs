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

namespace ReactiveExtensionExamples.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {

        string _searchQuery;

        public string SearchQuery
        {
            get { return _searchQuery; }
            set { this.RaiseAndSetIfChanged(ref _searchQuery, value); }
        }

        readonly ReactiveList<SearchResult> _searchResults = new ReactiveList<SearchResult>();
        public ReactiveList<SearchResult> SearchResults => _searchResults;

        ReactiveCommand<string, List<SearchResult>> _search;
        public ReactiveCommand<string, List<SearchResult>> Search
        {
            get { return _search; }
            private set { this.RaiseAndSetIfChanged(ref _search, value); }
        }

        public Interaction<string, Unit> SearchError { get; private set; } = new Interaction<string, Unit>();

        public SearchViewModel()
        {
            this.WhenActivated((CompositeDisposable disposables) =>
            {
                // ReactiveCommand has built-in support for background operations and
                // guarantees that this block will only run exactly once at a time, and
                // that the CanExecute will auto-disable and that property IsExecuting will
                // be set according whilst it is running.
                Search =
                    ReactiveCommand
                    .CreateFromTask<string, List<SearchResult>>(
                        // Here we're describing here, in a *declarative way*, the conditions in
                        // which the Search command is enabled.  Now our Command IsEnabled is
                        // perfectly efficient, because we're only updating the UI in the scenario
                        // when it should change.
                        async searchQuery =>
                        {
                            var random = new Random(Guid.NewGuid().GetHashCode());
                            await Task.Delay(random.Next(250, 2500));

                            //This is just here so simulate a network type exception
                            if (DateTime.Now.Second % 9 == 0)
                                throw new TimeoutException("Unable to connect to web service");

                            var searchService = Locator.CurrentMutable.GetService<IDuckDuckGoApi>();
                            var searchResult = await searchService.Search(searchQuery);

                            return searchResult
                                .RelatedTopics
                                .Select(rt =>
                                    new SearchResult
                                    {
                                        DisplayText = rt.Text,
                                        ImageUrl = rt?.Icon?.Url ?? string.Empty
                                    })
                                .ToList();
                        },
                        Observable
                            .CombineLatest(
                                this.WhenAnyValue(vm => vm.SearchQuery).Select(searchQuery => !string.IsNullOrEmpty(searchQuery)).DistinctUntilChanged(),
                                this.WhenAnyObservable(x => x.Search.IsExecuting).DistinctUntilChanged(),
                                (hasSearchQuery, isExecuting) => hasSearchQuery && !isExecuting)
                            .Do(cps => System.Diagnostics.Debug.WriteLine($"Can Perform Search: {cps}"))
                            .DistinctUntilChanged())
                    .DisposeWith(disposables);

                //// ReactiveCommands are themselves IObservables, whose value are the results
                //// from the async method, guaranteed to arrive on the given scheduler.
                //// We're going to take the list of search results that the background
                //// operation loaded, and them into our SearchResults.
                Search
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(searchResults =>
                    {
                        using(SearchResults.SuppressChangeNotifications()){
                            SearchResults.Clear();
                            SearchResults.AddRange(searchResults);
                        }
                    })
                    .DisposeWith(disposables);

                // ThrownExceptions is any exception thrown from the CreateFromObservable piped
                // to this Observable. Subscribing to this allows you to handle errors on
                // the UI thread.
                Search
                    .ThrownExceptions
                    .Subscribe(async ex =>
                    {
                        await SearchError.Handle("Potential Network Connectivity Error");
                    })
                    .DisposeWith(disposables);

                //Behaviors
                this
                    .WhenAnyValue(x => x.SearchQuery)
                    .Throttle(TimeSpan.FromSeconds(.75), TaskPoolScheduler.Default)
                    .Do(x => System.Diagnostics.Debug.WriteLine($"Throttle fired for {x}"))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .InvokeCommand(Search)
                    .DisposeWith(disposables);
            });

        }
    }
    
    public class SearchResult : ReactiveObject
        {

            string _imageUrl;

            public string ImageUrl
            {
                get { return _imageUrl; }
                set { this.RaiseAndSetIfChanged(ref _imageUrl, value); }
            }

            string _displayText;

            public string DisplayText
            {
                get { return _displayText; }
                set { this.RaiseAndSetIfChanged(ref _displayText, value); }
            }

        }
}