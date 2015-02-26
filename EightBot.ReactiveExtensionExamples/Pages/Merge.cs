using System;

using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using ReactiveUI;
using EightBot.ReactiveExtensionExamples.Utilities;

namespace EightBot.ReactiveExtensionExamples.Pages
{
	public class Merge : ContentPage
	{
		Label outputLabel;
		Button button1, button2;

		public Merge ()
		{

			button1 = new Button{ Text = "Peter Venkman" };

			var button1Clicked = Observable.FromEventPattern (x => button1.Clicked += x, x => button1.Clicked -= x);

			button2 = new Button{ Text = "Ray Stantz" };
			var button2Clicked = Observable.FromEventPattern (x => button2.Clicked += x, x => button2.Clicked -= x);

			Content = new StackLayout { 
				Padding = new Thickness(40d),
				Children = {
					button1, 
					button2,
					(outputLabel = new Label { XAlign = TextAlignment.Center, Text = "Whoa! It's Slimer!!!" }),

				}
			};
					
			button1Clicked
				.Merge(button2Clicked)
				.ObserveOn(RxApp.MainThreadScheduler)
				.Dump("Value")
				.Subscribe (args => outputLabel.Text = string.Format("Who merged the Streams?:{0}{1}", Environment.NewLine, ((Button)args.Sender).Text));

		}
	}
}


