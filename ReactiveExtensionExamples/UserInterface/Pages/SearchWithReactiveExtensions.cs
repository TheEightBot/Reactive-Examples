using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveExtensionExamples.Services.Api;
using Splat;
using Xamarin.Forms;
using ReactiveUI;
using System.Collections;
using System.Collections.Generic;

namespace ReactiveExtensionExamples.UserInterface.Pages
{
    public class SearchWithReactiveExtensions : ContentPage
	{
		Entry textEntry;
		Button search;
		ListView searchResults;

		//TODO: SWRE - Item 1 - Ah, CompositeDisposable, our new friend
		readonly CompositeDisposable eventSubscriptions = new CompositeDisposable();

		public SearchWithReactiveExtensions() {
			Title = "Search with Reactive Extensions";

			Content = new StackLayout
			{
				Padding = new Thickness(8d),
				Children = {
					(textEntry = new Entry{ Placeholder = "Enter Search Terms" }),
					(search = new Button{ Text = "Search" }),
					(searchResults = new ListView {
						VerticalOptions = LayoutOptions.FillAndExpand,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						HasUnevenRows = true,
						ItemTemplate = new DataTemplate(typeof(DuckDuckGoResultCell))
					})
				}
			};
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();


			var textChangedObservable =
				Observable
					//We can convert events into observables. Here is an example of how to do it for text changing on an entry
					.FromEventPattern<TextChangedEventArgs>(
						x => textEntry.TextChanged += x,
						x => textEntry.TextChanged -= x)
					//We can use linq to filter or transform the values that are emitted from the events
					.Select(x => x.EventArgs.NewTextValue)
					//Reactive Extensions has many ways to further refine our flow of data
					//Here, we are going to throttle the data, this means that only the last value will be sent after a delay
					.Throttle(TimeSpan.FromSeconds(.75), TaskPoolScheduler.Default);


			var buttonClickedObservable =
				Observable
					//Again, we create observables from an event. This time is is for a button click
					.FromEventPattern(
						x => search.Clicked += x,
						x => search.Clicked -= x)
					//We don't even have to use the values that come from the event.
					//Here, we tell it to use the value from the text entry
					.Select(_ => textEntry.Text);

            eventSubscriptions.Add(

                Observable
                    //We can merge two or more streams into a single one, this way we can process consistently
                    .Merge(textChangedObservable, buttonClickedObservable)
                    .Where(searchText => !string.IsNullOrWhiteSpace(searchText))
                    .Select(searchText =>
                        Observable
                            .FromAsync(
                                async cancellationToken =>
                                {
                                    var searchService = Locator.Current.GetService<IDuckDuckGoApi>();
                                    var searchResult = await searchService.Search(searchText, cancellationToken).ConfigureAwait(false);
                                    
                                    var formattedSearchResults =
                                        searchResult.RelatedTopics
                                            .Select(
                                                rt =>
                                                    new ViewModels.SearchResult
                                                    {
                                                        DisplayText = rt.Text,
                                                        ImageUrl = rt?.Icon?.Url ?? string.Empty
                                                    });

                                    if (cancellationToken.IsCancellationRequested)
                                        throw new TaskCanceledException();

                                    return formattedSearchResults;
                                })
                            .Catch(Observable.Return(Enumerable.Empty<ViewModels.SearchResult>())))
                    .Switch()
                    //This creates the subscription and provides a way to gather our data
                    .Subscribe(
                        results =>
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                try
                                {
                                    search.IsEnabled = false;

                                    searchResults.ItemsSource = results;
                                }
                                finally
                                {
                                    search.IsEnabled = true;
                                }
                            });                        
                        })
                        
			);
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			//TODO: SWRE - Item 4 - Cleanup our subscriptions
			eventSubscriptions.Clear();
		}

		class DuckDuckGoResultCell : ViewCell
		{
			public DuckDuckGoResultCell()
			{
				var grid = new Grid
				{
					Padding = new Thickness(8d),
					ColumnDefinitions = { 
						new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
						new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) }
					}
				};

				var icon = new Image
				{
					VerticalOptions = LayoutOptions.FillAndExpand,
					Aspect = Aspect.AspectFit
				};
				icon.SetBinding(Image.SourceProperty, "ImageUrl");
				grid.Children.Add(icon, 0, 0);

				var displayText = new Label
				{
					HorizontalOptions = LayoutOptions.FillAndExpand,
					VerticalOptions = LayoutOptions.FillAndExpand
				};
				displayText.SetBinding(Label.TextProperty, "DisplayText");
				grid.Children.Add(displayText, 1, 0);

				View = grid;
			}
		}
	}
}


