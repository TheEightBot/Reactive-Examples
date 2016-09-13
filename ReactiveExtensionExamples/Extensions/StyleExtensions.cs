using System;

using Xamarin.Forms;

namespace ReactiveExtensionExamples.Extensions
{
	public static class StyleExtensions
	{
		public static Style Extend (this Style style)
		{
			var newStyle = new Style (style.TargetType) {
				BasedOn = style
			}; 
			return newStyle;
		}

		public static Style Set<T> (this Style style, BindableProperty property, T value)
		{
			style.Setters.Add (new Setter () { Property = property, Value = value });
			return style;
		}
	}
}


