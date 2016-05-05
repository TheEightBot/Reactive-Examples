using System;
using Xamarin.Forms;
using EightBot.ReactiveExtensionExamples.Extensions;

namespace EightBot.ReactiveExtensionExamples.Values
{
	public static class Styles
	{
		public const string
			ReactiveNavigation = "ReactiveNavigation",
			ReactiveButton = "ReactiveButton",
			ReactiveEntry = "ReactiveEntry",
			ReactiveActivityIndicator = "ReactiveActivityIndicator";

		static Color 
			Indigo = Color.FromHex("#4C108C"),
			MediumVioletRed = Color.FromHex("#B7178C");

		static Styles () {
		}

		public static void Initialize (){
			if(Application.Current.Resources == null)
				Application.Current.Resources = new ResourceDictionary ();

			Application.Current.Resources [ReactiveNavigation] = CreateReactiveNavigationStyle ();
			Application.Current.Resources [ReactiveButton] = CreateReactiveButtonStyle ();
			Application.Current.Resources [ReactiveEntry] = CreateReactiveEntryStyle ();
			Application.Current.Resources [ReactiveActivityIndicator] = CreateReactiveActivityIndicatorStyle ();
		}

		static Style CreateReactiveNavigationStyle (){
			return new Style (typeof(NavigationPage))
				.Set (NavigationPage.BarBackgroundColorProperty, Indigo)
				.Set (NavigationPage.BarTextColorProperty, Color.White)
				.Set (NavigationPage.IconProperty, "slideout.png");
		}

		static Style CreateReactiveButtonStyle (){
			return new Style (typeof(Button))
				.Set (Button.BackgroundColorProperty, MediumVioletRed)
				.Set (Button.TextColorProperty, Color.White)
				.Set (Button.MarginProperty, new Thickness(8d));
		}

		static Style CreateReactiveEntryStyle (){
			return new Style (typeof(Entry))
				.Set (Entry.TextColorProperty, MediumVioletRed)
				.Set (Entry.MarginProperty, new Thickness(8d));
		}

		static Style CreateReactiveActivityIndicatorStyle (){
			return new Style (typeof(ActivityIndicator))
				.Set (ActivityIndicator.ColorProperty, MediumVioletRed);
		}
	}
}


