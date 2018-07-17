using System;
using System.Linq;
using ReactiveUI;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Reactive.Disposables;

namespace ReactiveExtensionExamples.ViewModels
{
	public class ColorSlider : ViewModelBase
	{
		int _red;
		public int Red
        {
            get => _red;
            set => this.RaiseAndSetIfChanged(ref _red, value);
        }

        int _green;
		public int Green {
            get => _green;
            set => this.RaiseAndSetIfChanged(ref _green, value);
        }

        int _blue;
		public int Blue {
			get => _blue;
			set => this.RaiseAndSetIfChanged(ref _blue, value);
		}
			
		ObservableAsPropertyHelper<Color> _color;
		public Color Color  => _color?.Value ?? default(Color);

		public ColorSlider ()
		{
            this.WhenActivated(
                (CompositeDisposable disposables) =>
                {
                    this
                        .WhenAnyValue(
                            x => x.Red, x => x.Green, x => x.Blue,
                            (red, green, blue) => Color.FromArgb(255, red, green, blue))
                        .ToProperty(this, x => x.Color, out _color)
                        .DisposeWith(disposables);
                });
		}
	}
}

