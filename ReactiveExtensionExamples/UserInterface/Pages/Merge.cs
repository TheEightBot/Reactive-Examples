using System;
using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive;
using System.Reactive.Disposables;

namespace ReactiveExtensionExamples.UserInterface.Pages
{
	public class Merge : PageBase
	{
		Label outputLabel;
		Button button1, button2, button3, button4;

		protected override void SetupUserInterface ()
		{
			Title = "Rx - Merge";

			button1 = new Button{ Text = "Peter Venkman" };

			button2 = new Button{ Text = "Ray Stantz" };

			button3 = new Button{ Text = "Egon Spengler" };

			button4 = new Button{ Text = "Winston Zeddemore" };

			Content = new StackLayout { 
				Padding = new Thickness(8d),
				Spacing = 16d,
				Children = {
					button1, 
					button2,
					button3,
					button4,
					(outputLabel = 
						new Label { 
							HorizontalTextAlignment = TextAlignment.Center, 
							Text = "Whoa, It's Slimer! Blast 'em!" 
						}
					),

				}
			};


		}

		protected override void SetupReactiveExtensions ()
		{		    
            Observable
                .Merge(
                    Observable
                        .FromEventPattern (
                            x => button1.Clicked += x, 
                            x => button1.Clicked -= x),
                    Observable
                        .FromEventPattern (
                            x => button2.Clicked += x, 
                            x => button2.Clicked -= x),
                    Observable
                        .FromEventPattern (
                            x => button3.Clicked += x, 
                            x => button3.Clicked -= x),
                    Observable
                        .FromEventPattern (
                            x => button4.Clicked += x, 
                            x => button4.Clicked -= x))
				.Select (args => 
					string.Format ("Who merged the Streams?{0}{1}", 
						Environment.NewLine, ((Button)args.Sender).Text))
                                .Subscribe (text => Device.BeginInvokeOnMainThread(() => outputLabel.Text = text))
                .DisposeWith(SubscriptionDisposables);
                            
		}
	}
}


