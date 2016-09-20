using System;
using System.Linq;
using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using ReactiveUI;
using ReactiveExtensionExamples.Utilities;
using System.Reactive;
using System.Reactive.Disposables;

namespace ReactiveExtensionExamples.Pages
{
	public class ReactiveUiSearch : ReactiveContentPage<ViewModels.SearchViewModel>
	{
		Entry textEntry;
		Button search;
		ListView searchResults;
		ActivityIndicator _loading;

		readonly CompositeDisposable bindingsDisposable = new CompositeDisposable();

		public ReactiveUiSearch() {
			Title = "Rx - Search";

			this.ViewModel = new ViewModels.SearchViewModel();

			Content = new StackLayout
			{
				Padding = new Thickness(8d),
				Children = {
					(textEntry = new Entry{ Placeholder = "Enter Search Terms" }),
					(search = new Button{ Text = "Search", IsEnabled = false }),
					(_loading = new ActivityIndicator{}),
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

			//TODO: RxSUI - Item 1 - Here we are just setting up bindings to our UI Elements
			//This is a two-way bind
			this.Bind(ViewModel, x => x.SearchQuery, c => c.textEntry.Text)
				.DisposeWith(bindingsDisposable);

			//This is a one-way bind
			this.OneWayBind(ViewModel, x => x.SearchResults, c => c.searchResults.ItemsSource)
				.DisposeWith(bindingsDisposable);

			//This is a command binding
			this.BindCommand(ViewModel, x => x.Search, c => c.search)
				.DisposeWith(bindingsDisposable);

			this.WhenAnyObservable(x => x.ViewModel.Search.IsExecuting)
			    .BindTo(_loading, c => c.IsRunning)
			    .DisposeWith(bindingsDisposable);

			//TODO: RxSUI - Item 2 - User error allows us to interact with our users and get feedback on how to handle an exception
			UserError
				.RegisterHandler(async (UserError arg) => {
					var result = await this.DisplayAlert("Search Failed", $"{arg.ErrorMessage}{Environment.NewLine}Retry search?", "Yes", "No");
					return result ? RecoveryOptionResult.RetryOperation : RecoveryOptionResult.CancelOperation;
				})
         		.DisposeWith(bindingsDisposable);
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			//TODO: RxSUI - Item 3 - We disposae of all of our bindings and any other subscriptions
			bindingsDisposable.Clear();
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


