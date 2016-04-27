using System;
using System.Linq;
using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using ReactiveUI;
using EightBot.ReactiveExtensionExamples.Utilities;
using System.Reactive.Disposables;
using System.Reactive;

namespace EightBot.ReactiveExtensionExamples.Pages
{
	public class CombineLatest: PageBase
	{
		BoxView colorDisplay;

		Slider red, green, blue;

		IObservable<int>
			redValueChangedObservable,
			greenValueChangedObservable,
			blueValueChangedObservable;

		public CombineLatest ()
		{
			Title = "Combine Latest";

			Content = new StackLayout { 
				Padding = new Thickness(40d),
				Children = {
					(colorDisplay = new BoxView{ HeightRequest = 250 }),

					new Label{ Text = "Red"},
					(red = new Slider(0, 255, 0)),

					new Label{ Text = "Green"},
					(green = new Slider(0, 255, 0)),

					new Label{ Text = "Blue"},
					(blue = new Slider(0, 255, 0))
				}
			};
		}
			

		protected override void SetupReactiveObservables ()
		{
			base.SetupReactiveObservables ();

			redValueChangedObservable =
				Observable
					.FromEventPattern<EventHandler<ValueChangedEventArgs>, ValueChangedEventArgs> (
						x => red.ValueChanged += x, 
						x => red.ValueChanged -= x
					)				
					.StartWith (new EventPattern<ValueChangedEventArgs> (null, new ValueChangedEventArgs (0, 0)))
					.Select(result => (int)result.EventArgs.NewValue);		

			greenValueChangedObservable =
				Observable
					.FromEventPattern<EventHandler<ValueChangedEventArgs>, ValueChangedEventArgs> (
						x => green.ValueChanged += x, 
						x => green.ValueChanged -= x
					)
					.StartWith (new EventPattern<ValueChangedEventArgs> (null, new ValueChangedEventArgs (0, 0)))
					.Select(result => (int)result.EventArgs.NewValue);
			
			blueValueChangedObservable =
				Observable
					.FromEventPattern<EventHandler<ValueChangedEventArgs>, ValueChangedEventArgs> (
						x => blue.ValueChanged += x, 
						x => blue.ValueChanged -= x
					)
					.StartWith(new EventPattern<ValueChangedEventArgs>(null, new ValueChangedEventArgs(0, 0)))
					.Select(result => (int)result.EventArgs.NewValue);
		}

		protected override void SetupReactiveSubscriptions ()
		{
			base.SetupReactiveSubscriptions ();

			SubscriptionDisposables.Add (
				redValueChangedObservable
					.CombineLatest (
						greenValueChangedObservable,
						blueValueChangedObservable,
						(r, g, b) => {
						return Color.FromRgb (r, g, b);
					})
					.Subscribe (color => {
						Device.BeginInvokeOnMainThread(() => colorDisplay.BackgroundColor = color);
					})
			);
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();

			SubscriptionDisposables.Clear ();
		}
	}
}


