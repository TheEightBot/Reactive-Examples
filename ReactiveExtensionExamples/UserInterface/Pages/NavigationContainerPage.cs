using System;
using Xamarin.Forms;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using ReactiveUI;

namespace ReactiveExtensionExamples.UserInterface.Pages
{
	public class NavigationContainerPage : MasterDetailPage
	{
		readonly NavListPage navListPage = new NavListPage();

		public NavigationContainerPage ()
		{
			var primaryNavPage = new NavigationPage(navListPage){
				Title = "Reactive Examples",
			};

			Master = primaryNavPage;

			this.MasterBehavior = MasterBehavior.Popover;
			this.IsPresented = true;

			navListPage
                .SelectedNavigation
				.ObserveOn (RxApp.MainThreadScheduler)
				.StartWith(NavigationPages.Delay)
				.Subscribe (navPage => {

					Page selectedPage = null;
					switch (navPage) {
						case NavigationPages.Async:
							selectedPage = new Pages.Async();
							break;
						case NavigationPages.AsyncEvents:
							selectedPage = new Pages.AsyncEvent();
							break;
						case NavigationPages.Delay:
							selectedPage = new Pages.Delay();
							break;
						case NavigationPages.Throttle:
							selectedPage = new Pages.Throttle();
							break;
						case NavigationPages.Buffer:
							selectedPage = new Pages.Buffer();
							break;
						case NavigationPages.BufferWithWhere:
							selectedPage = new Pages.BufferWithWhere();
							break;
						case NavigationPages.Merge:
							selectedPage = new Pages.Merge();
							break;
						case NavigationPages.Sample:
							selectedPage = new Pages.Sample();
							break;
						case NavigationPages.Scan:
							selectedPage = new Pages.Scan();
							break;
						case NavigationPages.AsyncToObservable:
							selectedPage = new Pages.AsyncToObservable ();
							break;
						case NavigationPages.CombineLatest:
							selectedPage = new Pages.CombineLatest ();
							break;
						case NavigationPages.StandardSearch:
							selectedPage = new Pages.StandardSearch();
							break;
						case NavigationPages.DynamicDataDashboard:
							selectedPage = new Pages.DynamicData.DynamicDataDashboard();
							break;
						case NavigationPages.SearchWithReactiveExtensions:
							selectedPage = new Pages.SearchWithReactiveExtensions();
							break;
						case NavigationPages.RxUiSearch:
							selectedPage = new Pages.ReactiveUiSearch();
							break;						
						case NavigationPages.RxUiColorSlider:
							selectedPage = new Pages.ReactiveUiColorSlider();
							break;
						case NavigationPages.RxUiLogin:
							selectedPage = new Pages.ReactiveUiLogin();
							break;
                        case NavigationPages.RxUiXamarinEssentials:
                            selectedPage = new Pages.ReactiveUiEssentials();
                            break;
						default:
						break;
					}

					if(selectedPage != null) {
						var detailNavPage = new NavigationPage(selectedPage) {};

						Detail = detailNavPage;
					}

					this.IsPresented = false;
				
				});
		}

		enum NavigationPages {
			Async,
			AsyncEvents,
			Delay,
			Throttle,
			Buffer,
			BufferWithWhere,
			Merge,
			Sample,
			Scan,
			AsyncToObservable,
			CombineLatest,
			RxUiColorSlider,
			RxUiLogin,
			StandardSearch,
			SearchWithReactiveExtensions,
			RxUiSearch,
            RxUiXamarinEssentials,
			DynamicDataDashboard,
		}

		class NavListPage : ContentPage {
			readonly Subject<NavigationPages> selectedNavigation = new Subject<NavigationPages>();
			public IObservable<NavigationPages> SelectedNavigation { get { return selectedNavigation.AsObservable(); } }

			public NavListPage () {
				Title = "Reactive Examples";

				var scrollContainer = new ScrollView {
					BackgroundColor = Color.FromHex("#dddddd"),
				};

				var navigationContainer = new StackLayout {
					Spacing = 8d
				};

				scrollContainer.Content = navigationContainer;

				var delay = new Button { 
					Text = "Delay",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.Delay)),
				};
				navigationContainer.Children.Add(delay);

				var throttle = new Button { 
					Text = "Throttle",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.Throttle)),
				};
				navigationContainer.Children.Add(throttle);

				var buffer = new Button { 
					Text = "Buffer",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.Buffer)),
				};
				navigationContainer.Children.Add(buffer);

				var bufferWithWhere = new Button { 
					Text = "Buffer with Where",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.BufferWithWhere)),
				};
				navigationContainer.Children.Add(bufferWithWhere);

				var sample = new Button { 
					Text = "Sample",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.Sample)),
				};
				navigationContainer.Children.Add(sample);

				var scan = new Button { 
					Text = "Scan",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.Scan)),
				};
				navigationContainer.Children.Add(scan);

				var merge = new Button { 
					Text = "Merge",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.Merge)),
				};
				navigationContainer.Children.Add(merge);

				var combineLatest = new Button { 
					Text = "Combine Latest",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.CombineLatest)),
				};
				navigationContainer.Children.Add(combineLatest);

				var asyncToObservable = new Button { 
					Text = "Mix Async With Observables",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.AsyncToObservable)),
				};
				navigationContainer.Children.Add(asyncToObservable);

				var reactiveAsync = new Button { 
					Text = "Async",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.Async)),
				};
				navigationContainer.Children.Add(reactiveAsync);

				var asyncEvents = new Button { 
					Text = "Async Events",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.AsyncEvents)),
				};
				navigationContainer.Children.Add(asyncEvents);

				var standardSearch = new Button
				{
					Text = "Standard Search",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.StandardSearch)),
				};
				navigationContainer.Children.Add(standardSearch);

				var dynamicDataDashboard = new Button
				{
					Text = "Dynamic Data",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.DynamicDataDashboard)),
				};
				navigationContainer.Children.Add(dynamicDataDashboard);

				var searchWithReactiveExtensions = new Button
				{
					Text = "Search with Reactive Extensions",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.SearchWithReactiveExtensions)),
				};
				navigationContainer.Children.Add(searchWithReactiveExtensions);

				var rxuiSearch = new Button
				{
					Text = "RxUI - Search",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.RxUiSearch)),
				};
				navigationContainer.Children.Add(rxuiSearch);

				var rxuiColorSlider = new Button { 
					Text = "RxUI - Color Slider",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.RxUiColorSlider)),
				};
				navigationContainer.Children.Add(rxuiColorSlider);

				var rxuiLogin = new Button { 
					Text = "RxUI - Login",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.RxUiLogin)),
				};
				navigationContainer.Children.Add(rxuiLogin);
                
                var rxuiEssentials = new Button { 
                    Text = "RxUI - Xamarin Essentials",
                    Command = new Command(() => selectedNavigation.OnNext(NavigationPages.RxUiXamarinEssentials)),
                };
                navigationContainer.Children.Add(rxuiEssentials);

				var reactiveLogo = new Image { 
					Source = "reactive_logo.png",
					Aspect = Aspect.AspectFit,
					Margin = new Thickness(8d)
				};
				navigationContainer.Children.Add(reactiveLogo);

				Content = scrollContainer;
			}
		}
	}
}

