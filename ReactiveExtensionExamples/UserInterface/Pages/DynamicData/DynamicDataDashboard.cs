using System;
using Xamarin.Forms;

namespace ReactiveExtensionExamples.UserInterface.Pages.DynamicData
{
    public class DynamicDataDashboard : PageBase
    {
        ScrollView _mainScroll;

        StackLayout _mainLayout;

        Button _simpleDynamicData, _filterDynamicData, _sortDynamicData, _sourceCacheDynamicData;

        public DynamicDataDashboard ()
        {
        }

        protected override void SetupUserInterface ()
        {
            _simpleDynamicData =
                new Button
                {
                    Text = "Simple"
                };

            _filterDynamicData =
                new Button
                {
                    Text = "Filter"
                };

            _sortDynamicData =
                new Button
                {
                    Text = "Sort"
                };

            _sourceCacheDynamicData =
                new Button
                {
                    Text = "Source Cache"
                };

            _mainLayout =
                new StackLayout
                {
                    Children =
                    {
                        _simpleDynamicData,
                        _filterDynamicData,
                        _sortDynamicData,
                        _sourceCacheDynamicData,
                    }
                };

            _mainScroll =
                new ScrollView
                {
                    Content = _mainLayout,
                };

            Content = _mainScroll;
        }

        protected override void SetupReactiveExtensions ()
        {
            _simpleDynamicData
                .Events()
                .Clicked
                .NavigateTo(
                    this,
                    _ => new SimpleDynamicData())
                .DisposeWith(SubscriptionDisposables);

            _filterDynamicData
                .Events()
                .Clicked
                .NavigateTo(
                    this,
                    _ => new FilterDynamicData())
                .DisposeWith(SubscriptionDisposables);

            _sortDynamicData
                .Events()
                .Clicked
                .NavigateTo(
                    this,
                    _ => new SortDynamicData())
                .DisposeWith(SubscriptionDisposables);

            _sourceCacheDynamicData
                .Events()
                .Clicked
                .NavigateTo(
                    this,
                    _ => new SourceCacheDynamicData())
                .DisposeWith(SubscriptionDisposables);
        }
    }
}
