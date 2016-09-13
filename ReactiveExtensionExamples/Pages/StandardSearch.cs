using System;
using System.Linq;
using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using ReactiveUI;
using ReactiveExtensionExamples.Utilities;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Splat;
using ReactiveExtensionExamples.Services.Api;
using System.Threading;

namespace ReactiveExtensionExamples.Pages
{
	public class StandardSearch : ContentPage
	{
		Entry textEntry;
		Button search;
		ListView searchResults;

		//TODO: SUI - Item 1 - Instance variables to manage UI state
		TimeSpan searchDelay = TimeSpan.FromSeconds(.75);
		DateTimeOffset lastSearch = DateTimeOffset.MinValue;
		string lastSearchTerm = string.Empty;
		DelayExecute searchDelayTimer;

		//TODO: SUI - Item 2 - Nothing special here, just UI setup
		public StandardSearch() {
			Title = "Standard Search";

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

			//TODO: SUI - Item 3 - Here is where it all starts... Event subscriptions ahoy!
			textEntry.TextChanged += TextEntry_TextChanged;
			search.Clicked += Search_Clicked;
		}


		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			searchDelayTimer?.Dispose();

			//TODO: SUI - Item 4 - What happens if we forget to unsubscribe to these events?
			textEntry.TextChanged -= TextEntry_TextChanged;
			search.Clicked -= Search_Clicked;
		}


		//TODO: SUI - Item 5.1 - Here we are getting our method calls setup to perform the search
		void TextEntry_TextChanged(object sender, TextChangedEventArgs e)
		{
			searchDelayTimer?.Dispose();
			searchDelayTimer = new DelayExecute(async () => await DoSearch(textEntry.Text, true), 750);
		}

		//TODO: SUI - Item 5.2 - Here we are getting our method calls setup to perform the search
		async void Search_Clicked(object sender, EventArgs e)
		{
			await DoSearch(textEntry.Text, false);
		}

		async Task DoSearch(string searchText, bool immediateSearch) {
			try
			{
				if (string.IsNullOrEmpty(searchText))
				{ 
					Device.BeginInvokeOnMainThread(() => searchResults.ItemsSource = null);
					return;
				}

				//TODO: SUI - Item 6 - We have no stellar way to manage search delay. It needs to go somewhere
				if (immediateSearch) {
					var searchTime = DateTimeOffset.Now;
					if ((searchTime - lastSearch) < searchDelay || searchText.Equals(lastSearchTerm))
						return;

					lastSearch = searchTime;
					lastSearchTerm = searchText;
				}

				//TODO: SUI - Item 7 - Lots of state management going on here and many opportunities to fail
				Device.BeginInvokeOnMainThread(() => search.IsEnabled = false);

				if (string.IsNullOrEmpty(searchText))
				{
					Device.BeginInvokeOnMainThread(() => searchResults.ItemsSource = null);
					return;
				}

				var searchService = Locator.CurrentMutable.GetService<IDuckDuckGoApi>();
				var searchResult = await searchService.Search(searchText);
				var formattedSearchResults =
					searchResult.RelatedTopics
						.Select(rt =>
							new ViewModels.SearchViewModel.SearchResult
							{
								DisplayText = rt.Text,
								ImageUrl = rt?.Icon?.Url ?? string.Empty
							})
			            .ToList();

				Device.BeginInvokeOnMainThread(() => searchResults.ItemsSource = formattedSearchResults);
			}
			catch (Exception)
			{
				Device.BeginInvokeOnMainThread(async () => await this.DisplayAlert("Exception", "There was a failure performing a search", "OK"));
			}
			finally
			{
				Device.BeginInvokeOnMainThread(() => search.IsEnabled = true);
			}
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

		internal sealed class DelayExecute : CancellationTokenSource
		{
			internal DelayExecute(Action callback, int millisecondsDueTime)
			{
				Task.Delay(millisecondsDueTime, Token).ContinueWith((t, s) =>
				{
					while (!IsCancellationRequested)
					{
						callback.Invoke();
					}
				}, null, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing)
					Cancel();

				base.Dispose(disposing);
			}
		}
	}
}


