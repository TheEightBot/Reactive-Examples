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
			var primaryNavPage = new NavigationPage(navListPage){
				Title = "Reactive Examples",
			};
			primaryNavPage.SetDynamicResource (VisualElement.StyleProperty, Values.Styles.ReactiveNavigation);

			Master = primaryNavPage;

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

					if(selectedPage != null) {
						var detailNavPage = new NavigationPage(selectedPage) {};

						detailNavPage.SetDynamicResource(VisualElement.StyleProperty, Values.Styles.ReactiveNavigation);

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
					Spacing = 24d
				};

				scrollContainer.Content = navigationContainer;

				var delay = new Button { 
					Text = "Delay",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.Delay)),
				};
				delay.SetDynamicResource(VisualElement.StyleProperty, Values.Styles.ReactiveButton);
				navigationContainer.Children.Add(delay);

				var throttle = new Button { 
					Text = "Throttle",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.Throttle)),
				};
				throttle.SetDynamicResource(VisualElement.StyleProperty, Values.Styles.ReactiveButton);
				navigationContainer.Children.Add(throttle);

				var buffer = new Button { 
					Text = "Buffer",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.Buffer)),
				};
				buffer.SetDynamicResource(VisualElement.StyleProperty, Values.Styles.ReactiveButton);
				navigationContainer.Children.Add(buffer);

				var bufferWithWhere = new Button { 
					Text = "Buffer with Where",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.BufferWithWhere)),
				};
				bufferWithWhere.SetDynamicResource(VisualElement.StyleProperty, Values.Styles.ReactiveButton);
				navigationContainer.Children.Add(bufferWithWhere);

				var sample = new Button { 
					Text = "Sample",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.Sample)),
				};
				sample.SetDynamicResource(VisualElement.StyleProperty, Values.Styles.ReactiveButton);
				navigationContainer.Children.Add(sample);

				var merge = new Button { 
					Text = "Merge",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.Merge)),
				};
				merge.SetDynamicResource(VisualElement.StyleProperty, Values.Styles.ReactiveButton);
				navigationContainer.Children.Add(merge);

				var combineLatest = new Button { 
					Text = "Combine Latest",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.CombineLatest)),
				};
				combineLatest.SetDynamicResource(VisualElement.StyleProperty, Values.Styles.ReactiveButton);
				navigationContainer.Children.Add(combineLatest);

				var asyncToObservable = new Button { 
					Text = "Mix Async With Observables",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.AsyncToObservable)),
				};
				asyncToObservable.SetDynamicResource(VisualElement.StyleProperty, Values.Styles.ReactiveButton);
				navigationContainer.Children.Add(asyncToObservable);

				var reactiveAsync = new Button { 
					Text = "Async",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.Async)),
				};
				reactiveAsync.SetDynamicResource(VisualElement.StyleProperty, Values.Styles.ReactiveButton);
				navigationContainer.Children.Add(reactiveAsync);

				var asyncEvents = new Button { 
					Text = "Async Events",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.AsyncEvents)),
				};
				asyncEvents.SetDynamicResource(VisualElement.StyleProperty, Values.Styles.ReactiveButton);
				navigationContainer.Children.Add(asyncEvents);

				var rxuiColorSlider = new Button { 
					Text = "RxUI - Color Slider",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.RxUiColorSlider)),
				};
				rxuiColorSlider.SetDynamicResource(VisualElement.StyleProperty, Values.Styles.ReactiveButton);
				navigationContainer.Children.Add(rxuiColorSlider);

				var rxuiLogin = new Button { 
					Text = "RxUI - Login",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.RxUiLogin)),
				};
				rxuiLogin.SetDynamicResource(VisualElement.StyleProperty, Values.Styles.ReactiveButton);
				navigationContainer.Children.Add(rxuiLogin);

				var akavache = new Button { 
					Text = "Akavache",
					Command = new Command(() => selectedNavigation.OnNext(NavigationPages.Akavache)),
				};
				akavache.SetDynamicResource(VisualElement.StyleProperty, Values.Styles.ReactiveButton);
				navigationContainer.Children.Add(akavache);

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

