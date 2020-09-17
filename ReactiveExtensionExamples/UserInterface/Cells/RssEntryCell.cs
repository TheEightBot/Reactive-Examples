using System;
using System.Reactive.Linq;
using ReactiveExtensionExamples.Models;
using ReactiveExtensionExamples.ViewModels;
using ReactiveUI;
using ReactiveUI.XamForms;
using Xamarin.Forms;

namespace ReactiveExtensionExamples.UserInterface.Cells
{
	class RssEntryCell : ReactiveContentView<RssEntry>
	{
		Label newFlag;

		public RssEntryCell ()
		{
			var stackLayout = new StackLayout
			{
				Padding = new Thickness(8d, 0d),
				Spacing = 4d
			};

			newFlag = new Label
			{
				Text = "New",
				FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
				TextColor = Color.Accent,
				FontAttributes = FontAttributes.Italic,
			};
			stackLayout.Children.Add(newFlag);

			var title = new Label
			{
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
				LineBreakMode = LineBreakMode.TailTruncation,
				MaxLines = 3,
				HeightRequest = 70,
			};
			stackLayout.Children.Add(title);

			var category = new Label
			{
				FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
				TextColor = Color.Gray,
				HeightRequest = 20,
			};
			stackLayout.Children.Add(category);

			var updated = new Label
			{
				FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
				FontAttributes = FontAttributes.Italic,
				TextColor = Color.Gray,
				HeightRequest = 20,
			};
			stackLayout.Children.Add(updated);

			var padding =
				new ContentView
				{
					BackgroundColor = Color.Silver,
					HeightRequest = 2,
					Margin = new Thickness(-8, 0),
				};
			stackLayout.Children.Add(padding);

			Content = stackLayout;

			this.WhenAnyValue(x => x.ViewModel)
				.IsNotNull()
				.ObserveOn(RxApp.MainThreadScheduler)
				.Do(
					vm =>
					{
						title.Text = vm.Title;
						category.Text = vm.Category;
						updated.Text = vm.Updated.ToLocalTime().ToString("dd MMM yyyy hh:mm tt");
					})
				.Subscribe();

			this.WhenAnyValue(x => x.ViewModel.New)
				.ObserveOn(RxApp.MainThreadScheduler)
				.BindTo(this, x => x.newFlag.IsVisible);
		}
	}
}
