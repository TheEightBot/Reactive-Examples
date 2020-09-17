using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.XamForms;
using Xamarin.Forms;
using System.Runtime.InteropServices;
using ReactiveExtensionExamples.ViewModels.DynamicData;

namespace ReactiveExtensionExamples.UserInterface.Pages.DynamicData
{
    public class SortDynamicData : ReactiveContentPage<ViewModels.DynamicData.SortDynamicDataViewModel>
	{
		Button sortUpdatedAscending, sortUpdatedDescending, sortTitleAscending, sortTitleDescending;
		CollectionView searchResults;
		ActivityIndicator _loading;
        SerialDisposable _searchErrorDisposable;
        
		public SortDynamicData () {
			Title = "Rx - Sort Dynamic Data";

			this.ViewModel = new ViewModels.DynamicData.SortDynamicDataViewModel();

			Content = new StackLayout
			{
				Padding = new Thickness(8d),
				Children = {
					(sortTitleAscending = new Button{ Text = "Title - Ascending" }),
					(sortTitleDescending = new Button{ Text = "Title - Descending" }),
					(sortUpdatedAscending = new Button{ Text = "Updated - Ascending" }),
					(sortUpdatedDescending = new Button{ Text = "Updated - Descending" }),
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
				Observable
					.Merge(
						sortTitleAscending.Events().Clicked.Select(_ => SortType.TitleAscending),
						sortTitleDescending.Events().Clicked.Select(_ => SortType.TitleDescending),
						sortUpdatedAscending.Events().Clicked.Select(_ => SortType.DateTimeAscending),
						sortUpdatedDescending.Events().Clicked.Select(_ => SortType.DateTimeDescending))
					.Do(x => this.ViewModel.SelectedSortType = x)
					.Subscribe()
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


