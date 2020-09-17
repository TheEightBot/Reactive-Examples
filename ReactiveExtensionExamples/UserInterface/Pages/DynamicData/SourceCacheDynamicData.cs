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
    public class SourceCacheDynamicData : ReactiveContentPage<ViewModels.DynamicData.SourceCacheDynamicDataViewModel>
	{
		CollectionView searchResults;
		ActivityIndicator _loading;
        SerialDisposable _searchErrorDisposable;
        
		public SourceCacheDynamicData () {
			Title = "Rx - Source Cache Dynamic Data";

			this.ViewModel = new ViewModels.DynamicData.SourceCacheDynamicDataViewModel();

			Content = new StackLayout
			{
				Padding = new Thickness(8d),
				Children = {
					(_loading = new ActivityIndicator{}),
					(searchResults = new CollectionView() {
						VerticalOptions = LayoutOptions.FillAndExpand,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						ItemTemplate = new DataTemplate(typeof(Cells.RssEntryCell)),
						ItemSizingStrategy = ItemSizingStrategy.MeasureFirstItem,
						ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepScrollOffset,
					})
				}
			};

            this.WhenActivated((CompositeDisposable disposables) =>
            {            
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


