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

namespace ReactiveExtensionExamples.ViewModels
{
    public class SearchViewModel : ReactiveObject
    {

		string _searchQuery;

		public string SearchQuery
		{
			get { return _searchQuery; }
			set { this.RaiseAndSetIfChanged(ref _searchQuery, value); }
		}

		ReactiveCommand<List<SearchResult>> _search;
		public ReactiveCommand<List<SearchResult>> Search
		{
			get { return _search; }
			private set { this.RaiseAndSetIfChanged(ref _search, value); }
		}

		ObservableCollection<SearchResult> _searchResults;
		public ObservableCollection<SearchResult> SearchResults
		{
			get { return _searchResults; }
			private set { this.RaiseAndSetIfChanged(ref _searchResults, value); }
		}

		public SearchViewModel()
        {
			SearchResults = new ObservableCollection<SearchResult>();

			// ReactiveCommand has built-in support for background operations and
            // guarantees that this block will only run exactly once at a time, and
            // that the CanExecute will auto-disable and that property IsExecuting will
            // be set according whilst it is running.
			Search = ReactiveCommand.CreateAsyncTask(
				
				// Here we're describing here, in a *declarative way*, the conditions in
				// which the Search command is enabled.  Now our Command IsEnabled is
				// perfectly efficient, because we're only updating the UI in the scenario
				// when it should change.
				Observable
					.CombineLatest(
						this.WhenAnyValue(vm => vm.SearchQuery).Select(searchQuery => !string.IsNullOrEmpty(searchQuery)).DistinctUntilChanged(),
						this.WhenAnyObservable(x => x.Search.IsExecuting).DistinctUntilChanged(),
						(hasSearchQuery, isExecuting) => hasSearchQuery && !isExecuting)
					.Do(cps => System.Diagnostics.Debug.WriteLine($"Can Perform Search: {cps}"))
					.DistinctUntilChanged(),

				async _ => {
					var random = new Random(Guid.NewGuid().GetHashCode());
					await Task.Delay(random.Next(250, 2500));

					//This is just here so simulate a network type exception
					if (DateTime.Now.Second % 3 == 0)
						throw new TimeoutException("Unable to connec to web service");

					var searchService = Locator.CurrentMutable.GetService<IDuckDuckGoApi>();
					var searchResult = await searchService.Search(SearchQuery);

					return searchResult
						.RelatedTopics
						.Select(rt =>
							new SearchResult
							{
								DisplayText = rt.Text,
								ImageUrl = rt?.Icon?.Url ?? string.Empty
							})
						.ToList();
				});

			//// ReactiveCommands are themselves IObservables, whose value are the results
			//// from the async method, guaranteed to arrive on the given scheduler.
			//// We're going to take the list of search results that the background
			//// operation loaded, and them into our SearchResults.
			Search
				.ObserveOn(RxApp.MainThreadScheduler)
				.Subscribe(searchResult =>
	            {
					SearchResults.Clear();

					foreach (var item in searchResult)
						SearchResults.Add(item);
	            });

            // ThrownExceptions is any exception thrown from the CreateFromObservable piped
            // to this Observable. Subscribing to this allows you to handle errors on
            // the UI thread.
            Search.ThrownExceptions
				.Subscribe(async ex => {
				    var result = await UserError.Throw("Potential Network Connectivity Error", ex);

					if (result == RecoveryOptionResult.RetryOperation && Search.CanExecute(null))
						Search.Execute(null);
				});

			this.WhenAnyValue(x => x.SearchQuery)
				.Throttle(TimeSpan.FromSeconds(.75), TaskPoolScheduler.Default)
			    .Do(x => System.Diagnostics.Debug.WriteLine($"Throttle fired for {x}"))
			    .ObserveOn(RxApp.MainThreadScheduler)
			    .InvokeCommand(Search);
        }

		public class SearchResult : ReactiveObject { 
		
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
}
