using System;
using System.Linq;
using ReactiveUI;
using System.Text.RegularExpressions;
using System.Drawing;

namespace ReactiveExtensionExamples.ViewModels
{
	public class ColorSlider : ReactiveObject
	{
		int _red;
		public int Red {
			get { return _red; }
			set { this.RaiseAndSetIfChanged (ref _red, value); }
		}

		int _green;
		public int Green {
			get { return _green; }
			set { this.RaiseAndSetIfChanged(ref _green, value); }
		}

		int _blue;
		public int Blue {
			get { return _blue; }
			set { this.RaiseAndSetIfChanged(ref _blue, value); }
		}
			
		ObservableAsPropertyHelper<Color> _color;
		public Color Color {
			get { return _color.Value; }
		}

		public ColorSlider ()
		{
			this.WhenAnyValue (
				x => x.Red, x => x.Green, x => x.Blue,
				(red, green, blue) => Color.FromArgb(255, red, green, blue)
			)
			.ToProperty(this, v => v.Color, out _color);
		}
	}
}

