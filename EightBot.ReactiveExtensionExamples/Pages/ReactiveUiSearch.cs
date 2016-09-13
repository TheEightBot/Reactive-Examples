using System;
using System.Linq;
using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using ReactiveUI;
using EightBot.ReactiveExtensionExamples.Utilities;
using System.Reactive;
using System.Reactive.Disposables;

namespace EightBot.ReactiveExtensionExamples.Pages
{
	public class ReactiveUiSearch : ReactiveContentPage<ViewModels.SearchViewModel>
	{
		Entry textEntry;
		ListView searchResults;

		readonly CompositeDisposable _bindingsDisposable = new CompositeDisposable();

		IObservable<string>
			textEntryObservable;

		public ReactiveUiSearch() {
			Title = "Rx - Search";

			this.ViewModel = new ViewModels.SearchViewModel();


			Content = new StackLayout
			{
				Padding = new Thickness(8d),
				Children = {
					(textEntry = new Entry{ Placeholder = "Enter Search Terms" }),
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

			Observable
				.FromEventPattern<EventHandler<TextChangedEventArgs>, TextChangedEventArgs>(
					x => textEntry.TextChanged += x,
					x => textEntry.TextChanged -= x
				)
				.Select(args => args.EventArgs.NewTextValue)
				.BindTo(ViewModel, vm => vm.SearchQuery)
				.DisposeWith(_bindingsDisposable);

			this.WhenAnyValue(x => x.ViewModel.SearchResults)
				.BindTo(searchResults, c => c.ItemsSource)
			    .DisposeWith(_bindingsDisposable);
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_bindingsDisposable.Clear();
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


