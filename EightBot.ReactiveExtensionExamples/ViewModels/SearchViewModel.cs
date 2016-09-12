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
using EightBot.ReactiveExtensionExamples.Services;
using EightBot.ReactiveExtensionExamples.Services.Api;
using ReactiveUI;
using Splat;

namespace EightBot.ReactiveExtensionExamples.ViewModels
{
    public class SearchViewModel : ReactiveObject, ISupportsActivation
    {
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
				this.WhenAnyValue(vm => vm.SearchQuery, value => !string.IsNullOrWhiteSpace(value)), 
				async _ => {
					try
					{
						var searchService = Locator.CurrentMutable.GetService<IDuckDuckGoApi>();
						var result = await searchService.Search(SearchQuery);
						return result;
					}
					catch (Exception ex)
					{
						System.Diagnostics.Debug.WriteLine(ex);
					}
					return new DuckDuckGoSearchResult();
				}, RxApp.MainThreadScheduler);

			// ReactiveCommands are themselves IObservables, whose value are the results
			// from the async method, guaranteed to arrive on the given scheduler.
			// We're going to take the list of search results that the background
			// operation loaded, and them into our SearchResults.
			Search.Subscribe(searchResult =>
            {
				SearchResults.Clear();
					
				foreach (var item in searchResult
						.RelatedTopics
						.Select(rt =>
							new SearchResult
							{
								DisplayText = rt.Text,
								ImageUrl = rt?.Icon?.Url ?? string.Empty
							}))
				{
					SearchResults.Add(item);
				}


            });

            // ThrownExceptions is any exception thrown from the CreateFromObservable piped
            // to this Observable. Subscribing to this allows you to handle errors on
            // the UI thread.
            Search.ThrownExceptions.Subscribe(ex => {
                UserError.Throw("Potential Network Connectivity Error", ex);
            });

            this.WhenAnyValue(x => x.SearchQuery)
				.Sample(TimeSpan.FromSeconds(1), TaskPoolScheduler.Default)
				.InvokeCommand(Search);
        }

		string _searchQuery;

		public string SearchQuery
		{
			get { return _searchQuery; }
			set { this.RaiseAndSetIfChanged(ref _searchQuery, value); }
		}

        public IReactiveCommand<DuckDuckGoSearchResult> Search { get; private set; }

        public ObservableCollection<SearchResult> SearchResults { get; private set; }

        ViewModelActivator ISupportsActivation.Activator { get; } = new ViewModelActivator();

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
