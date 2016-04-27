using System;
using Xamarin.Forms;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using ReactiveUI;

namespace EightBot.ReactiveExtensionExamples.Pages
{
	public class NavigationContainerPage : MasterDetailPage
	{
		readonly NavListPage navListPage = new NavListPage();

		public NavigationContainerPage ()
		{
			Master = new NavigationPage(navListPage){
				Title = "Reactive Examples",
				BarTextColor = Color.White,
				BarBackgroundColor = Color.Gray,
				Icon = "slideout.png"
			};

			this.MasterBehavior = MasterBehavior.Popover;

			navListPage.SelectedNavigation
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
						case NavigationPages.AsyncToObservable:
							selectedPage = new Pages.AsyncToObservable ();
							break;
						case NavigationPages.CombineLatest:
							selectedPage = new Pages.CombineLatest ();
							break;
						case NavigationPages.RxUiColorSlider:
							selectedPage = new Pages.ReactiveUiColorSlider();
							break;
						case NavigationPages.RxUiLogin:
							selectedPage = new Pages.ReactiveUiLogin();
							break;
						case NavigationPages.Akavache:
							selectedPage = new Pages.Akavache();
							break;
						default:
						break;
					}

					if(selectedPage != null)
						Detail = 
							new NavigationPage(selectedPage){
								BarBackgroundColor = Color.Gray,
								BarTextColor = Color.White,
							};

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
			AsyncToObservable,
			CombineLatest,
			RxUiColorSlider,
			RxUiLogin,
			Akavache
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
					
					Padding = new Thickness(8d),
					Spacing = 24d
				};
				scrollContainer.Content = navigationContainer;

				var delay = new NavigationButton { 
					Text = "Delay",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.Delay)),
				};
				navigationContainer.Children.Add(delay);

				var throttle = new NavigationButton { 
					Text = "Throttle",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.Throttle)),
				};
				navigationContainer.Children.Add(throttle);

				var buffer = new NavigationButton { 
					Text = "Buffer",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.Buffer)),
				};
				navigationContainer.Children.Add(buffer);

				var bufferWithWhere = new NavigationButton { 
					Text = "Buffer with Where",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.BufferWithWhere)),
				};
				navigationContainer.Children.Add(bufferWithWhere);

				var merge = new NavigationButton { 
					Text = "Merge",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.Merge)),
				};
				navigationContainer.Children.Add(merge);

				var sample = new NavigationButton { 
					Text = "Sample",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.Sample)),
				};
				navigationContainer.Children.Add(sample);

				var asyncToObservable = new NavigationButton { 
					Text = "Mix Async With Observables",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.AsyncToObservable)),
				};
				navigationContainer.Children.Add(asyncToObservable);

				var async = new NavigationButton { 
					Text = "Async",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.Async)),
				};
				navigationContainer.Children.Add(async);

				var asyncEvents = new NavigationButton { 
					Text = "Async Events",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.AsyncEvents)),
				};
				navigationContainer.Children.Add(asyncEvents);

				var combineLatest = new NavigationButton { 
					Text = "Combine Latest",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.CombineLatest)),
				};
				navigationContainer.Children.Add(combineLatest);

				var rxuiColorSlider = new NavigationButton { 
					Text = "RxUI - Color Slider",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.RxUiColorSlider)),
				};
				navigationContainer.Children.Add(rxuiColorSlider);

				var rxuiLogin = new NavigationButton { 
					Text = "RxUI - Login",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.RxUiLogin)),
				};
				navigationContainer.Children.Add(rxuiLogin);

				var akavache = new NavigationButton { 
					Text = "Akavache",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.Akavache)),
				};
				navigationContainer.Children.Add(akavache);

				Content = scrollContainer;
			}
		
			internal class NavigationButton : Button {

				public NavigationButton () {
					TextColor = Color.White;
					BackgroundColor = Color.Gray;
				}

			}
		}
	}
}

