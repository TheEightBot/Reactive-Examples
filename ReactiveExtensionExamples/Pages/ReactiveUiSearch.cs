using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.XamForms;
using Xamarin.Forms;
using System.Runtime.InteropServices;

namespace ReactiveExtensionExamples.Pages
{
    public class ReactiveUiSearch : ReactiveContentPage<ViewModels.SearchViewModel>
	{
		Entry textEntry;
		ListView searchResults;
        Button search;
		ActivityIndicator _loading;
        SerialDisposable _searchErrorDisposable;
        
		public ReactiveUiSearch() {
			Title = "Rx - Search";

			this.ViewModel = new ViewModels.SearchViewModel();

			Content = new StackLayout
			{
				Padding = new Thickness(8d),
				Children = {
					(textEntry = new Entry{ Placeholder = "Enter Search Terms" }),
                    (search = new Button{ Text = "Search" }),
					(_loading = new ActivityIndicator{}),
					(searchResults = new ListView(ListViewCachingStrategy.RecycleElement) {
						VerticalOptions = LayoutOptions.FillAndExpand,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						ItemTemplate = new DataTemplate(typeof(DuckDuckGoResultCell))
					})
				}
			};

            this.WhenActivated((CompositeDisposable disposables) =>
            {
                _searchErrorDisposable?.Dispose();
                _searchErrorDisposable = new SerialDisposable();
            
                //TODO: RxSUI - Item 1 - Here we are just setting up bindings to our UI Elements
                //This is a two-way bind
                this.Bind(ViewModel, x => x.SearchQuery, c => c.textEntry.Text)
                    .DisposeWith(disposables);
                    
                this.BindCommand(ViewModel, x => x.Search, c => c.search, this.WhenAnyValue(x => x.ViewModel.SearchQuery))
                    .DisposeWith(disposables);
    
                //This is a one-way bind
                this.OneWayBind(ViewModel, x => x.SearchResults, c => c.searchResults.ItemsSource)
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
                                    await this.DisplayAlert("Error", interaction.Input, "OK");
                                    interaction.SetOutput(Unit.Default);
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


