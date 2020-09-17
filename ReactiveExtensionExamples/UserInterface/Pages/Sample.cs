using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xamarin.Forms;

namespace ReactiveExtensionExamples.UserInterface.Pages
{
	public class Sample : PageBase
	{
		Entry textEntry;
		WebView webView;

		protected override void SetupUserInterface ()
		{
			Title = "Rx - Sample";

			Content = new StackLayout { 
				Padding = new Thickness(8d),
				Children = {
					(textEntry = new Entry{ Placeholder = "Enter Search Terms" }),
					(webView = new WebView {
						VerticalOptions = LayoutOptions.FillAndExpand, 
						HorizontalOptions = LayoutOptions.FillAndExpand,
					})
				}
			};
		}

		protected override void SetupReactiveExtensions ()
		{
			Observable
				.FromEventPattern<EventHandler<TextChangedEventArgs>, TextChangedEventArgs> (
					x => textEntry.TextChanged += x, 
					x => textEntry.TextChanged -= x)
				.Select(args => 
					string.Format("https://frinkiac.com/?q={0}", args.EventArgs.NewTextValue.Replace(" ", "+")))
                .Subscribe (
                    searchUrl => Device.BeginInvokeOnMainThread(() => webView.Source = searchUrl),
                    ex => Device.BeginInvokeOnMainThread(() => webView.Source = "https://frinkiac.com/caption/S04E05/1135500"))
                .DisposeWith(SubscriptionDisposables);
		}
	}
}


