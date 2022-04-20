using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.XamForms;
using Xamarin.Forms;

namespace ReactiveExtensionExamples.UserInterface.Pages
{
    public class ReactiveUiSearch : ReactiveContentPage<ViewModels.SearchViewModel>
	{
        private Entry _textEntry;
        private ListView _searchResults;
        private RefreshView _pullToRefresh;
        private Button _search;
        private ActivityIndicator _loading;
        private SerialDisposable _searchErrorDisposable;
        
		public ReactiveUiSearch() {
			Title = "Rx - Search";

			this.ViewModel = new ViewModels.SearchViewModel();

			Content = new StackLayout
			{
				Padding = new Thickness(8d),
				Children =
                {
					(_textEntry = new Entry{ Placeholder = "Enter Search Terms" }),
                    (_search = new Button{ Text = "Search" }),
					(_loading = new ActivityIndicator{}),
                    (_pullToRefresh =
                        new RefreshView
                        {
                            Content =
                                _searchResults =
                                    new ListView(ListViewCachingStrategy.RecycleElement)
                                    {
                                        ItemTemplate = new DataTemplate(typeof(DuckDuckGoResultCell))
                                    },
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.FillAndExpand,
                        }),
				}
			};

            this.WhenActivated(
                (CompositeDisposable disposables) =>
            {
                _searchErrorDisposable?.Dispose();
                _searchErrorDisposable = new SerialDisposable();
            
                //TODO: RxSUI - Item 1 - Here we are just setting up bindings to our UI Elements
                //This is a two-way bind
                this.Bind(ViewModel, x => x.SearchQuery, c => c._textEntry.Text)
                    .DisposeWith(disposables);
                    
                this.BindCommand(ViewModel, x => x.Search, c => c._search, this.WhenAnyValue(x => x.ViewModel.SearchQuery))
                    .DisposeWith(disposables);

                //Once this event is fired off, it will start the refresh
                Observable
                    .FromEventPattern(
                        x => _pullToRefresh.Refreshing += x,
                        x => _pullToRefresh.Refreshing -= x)
                    .Select(_ => this.WhenAnyValue(x => x.ViewModel.SearchQuery).Take(1))
                    .Switch()
                    .InvokeCommand(this, x => x.ViewModel.Search)
                    .DisposeWith(disposables);

                //This will only trigger when the search command completes
                this.WhenAnyObservable(x => x.ViewModel.Search)
                    .Select(_ => false)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, ui => ui._pullToRefresh.IsRefreshing)
                    .DisposeWith(disposables);

                //This is a one-way bind
                this.OneWayBind(ViewModel, x => x.SearchResults, c => c._searchResults.ItemsSource)
                    .DisposeWith(disposables);
    
                //TODO: RxSUI - Item 2 - User error allows us to interact with our users and get feedback on how to handle an exception
                this.WhenAnyValue(x => x.ViewModel.SearchError)
                    .Where(x => x != null)
                    .Subscribe(searchError =>
                    {
                        _searchErrorDisposable.Disposable =
                            searchError
                                .RegisterHandler(async interaction =>
                                {
                                    var result = await this.DisplayAlert("Error", $"{interaction.Input}{Environment.NewLine}Would you like to retry?", "Retry", "Cancel");
                                    interaction.SetOutput(result);
                                });
                    })
                    .DisposeWith(disposables);
                    
                _searchErrorDisposable
                    .DisposeWith(disposables);
            });
		}

		class DuckDuckGoResultCell : ReactiveViewCell<ViewModels.SearchResult>
		{
            Image icon;

            Label displayText;
        
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

				icon = new Image
				{
					VerticalOptions = LayoutOptions.FillAndExpand,
					Aspect = Aspect.AspectFit
				};
				grid.Children.Add(icon, 0, 0);

				displayText = new Label
				{
					HorizontalOptions = LayoutOptions.FillAndExpand,
					VerticalOptions = LayoutOptions.FillAndExpand
				};
				grid.Children.Add(displayText, 1, 0);

				View = grid;

                this.WhenActivated((CompositeDisposable disposables) =>
                {
                    this.WhenAnyValue(x => x.ViewModel.ImageUrl)
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Subscribe(x => icon.Source = x)
                        .DisposeWith(disposables);
                        
                    this.OneWayBind(ViewModel, x => x.DisplayText, x => x.displayText.Text)
                        .DisposeWith(disposables);
                });
			}
		}
	}
}


