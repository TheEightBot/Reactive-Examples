using System.Linq;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xamarin.Essentials;
using System.Numerics;
using System;
using System.Drawing;

namespace ReactiveExtensionExamples.ViewModels
{
    public class XamarinEssentials : ViewModelBase
	{
        ObservableAsPropertyHelper<double> _x;
        public double X => _x.Value;
        
        ObservableAsPropertyHelper<double> _y;
        public double Y => _y.Value;
    
        ObservableAsPropertyHelper<double> _z;
        public double Z => _z.Value;
    
        ObservableAsPropertyHelper<Color> _color;
        public Color Color  => _color?.Value ?? default(Color);
    
		public XamarinEssentials ()
		{
            this.WhenActivated(
                (CompositeDisposable disposables) =>
                {
                    var gyroscopeChanged = 
                        Observable
                            .FromEvent<EventHandler<AccelerometerChangedEventArgs>, AccelerometerChangedEventArgs>(
                                x => Accelerometer.ReadingChanged += x,
                                x => Accelerometer.ReadingChanged -= x)
                            .SubscribeOn(RxApp.TaskpoolScheduler)
                            .Select(x => x.Reading.Acceleration)
                            .StartWith(Vector3.Zero)
                            .Do(x => 
                            {
                                if (!Accelerometer.IsMonitoring)
                                    Accelerometer.Start(SensorSpeed.UI);
                            })
                            .Finally(() => {
                                Accelerometer.Stop();
                            })
                            .Throttle(TimeSpan.FromMilliseconds(20))
                            .Do(x => System.Diagnostics.Debug.WriteLine($"ACC: {x}"))
                            .Publish()
                            .RefCount();

                    gyroscopeChanged
                        .Select(x => (double)x.X)
                        .ToProperty(this, x => x.X, out _x)
                        .DisposeWith(disposables);

                    gyroscopeChanged
                        .Select(x => (double)x.Y)
                        .ToProperty(this, x => x.Y, out _y)
                        .DisposeWith(disposables);
                        
                    gyroscopeChanged
                        .Select(x => (double)x.Z)
                        .ToProperty(this, x => x.Z, out _z)
                        .DisposeWith(disposables);      
                        
                    gyroscopeChanged
                        .Select(x => {
                            var r = (int)Math.Min(255, Math.Abs(x.X * 255));
                            var g = (int)Math.Min(255, Math.Abs(x.Y * 255));
                            var b = (int)Math.Min(255, Math.Abs(x.Z * 255));
                            return Color.FromArgb(r, g, b);
                        })
                        .ToProperty(this, x => x.Color, out _color)
                        .DisposeWith(disposables);                  
                    
                });
		}
	}
}

