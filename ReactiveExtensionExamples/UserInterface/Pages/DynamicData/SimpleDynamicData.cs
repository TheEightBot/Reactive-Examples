using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.XamForms;
using Xamarin.Forms;
using System.Runtime.InteropServices;

namespace ReactiveExtensionExamples.UserInterface.Pages.DynamicData
{
    public class SimpleDynamicData : ReactiveContentPage<ViewModels.DynamicData.SimpleDynamicDataViewModel>
	{
		Entry textEntry;
		CollectionView searchResults;
        Button search;
		ActivityIndicator _loading;
        SerialDisposable _searchErrorDisposable;
        
		public SimpleDynamicData () {
			Title = "Rx - Simple Dynamic Data";

			this.ViewModel = new ViewModels.DynamicData.SimpleDynamicDataViewModel();

			Content = new StackLayout
			{
				Padding = new Thickness(8d),
				Children = {
					(textEntry = new Entry{ Placeholder = "Enter Search Terms" }),
                    (search = new Button{ Text = "Search" }),
					(_loading = new ActivityIndicator{}),
					(searchResults = new CollectionView() {
						VerticalOptions = LayoutOptions.FillAndExpand,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						ItemTemplate = new DataTemplate(typeof(Cells.RssEntryCell)),
						ItemSizingStrategy = ItemSizingStrategy.MeasureFirstItem,
					})
				}
			};

            this.WhenActivated((CompositeDisposable disposables) =>
            {            
                this.Bind(ViewModel, x => x.SearchQuery, c => c.textEntry.Text)
                    .DisposeWith(disposables);
                    
                this.BindCommand(ViewModel, x => x.Search, c => c.search, this.WhenAnyValue(x => x.ViewModel.SearchQuery))
                    .DisposeWith(disposables);
    
                //This is a one-way bind
                this.OneWayBind(ViewModel, x => x.SearchResults, c => c.searchResults.ItemsSource)
                    .DisposeWith(disposables);

				this.WhenAnyValue(x => x.ViewModel)
					.Where(x => x != null)
					.SelectUnit()
					.InvokeCommand(this, x => x.ViewModel.Search)
					.DisposeWith(disposables);
			});
		}
	}
}


