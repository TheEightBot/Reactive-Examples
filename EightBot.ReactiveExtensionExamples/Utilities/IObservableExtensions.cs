using System;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Threading;

namespace EightBot.ReactiveExtensionExamples.Utilities
{
	public static class IObservableExtensions
	{
		public static IObservable<T> Dump<T>(this IObservable<T> source, string name)
		{
			source.Subscribe(
				i=>System.Diagnostics.Debug.WriteLine("{0}-->{1}", name, i), 
				ex=>System.Diagnostics.Debug.WriteLine("{0} failed-->{1}", name, ex.Message),
				()=>System.Diagnostics.Debug.WriteLine("{0} completed", name));

			return source;
		}

		public static TDisposable DisposeWith<TDisposable> (this TDisposable observable, CompositeDisposable disposables) where TDisposable : class, IDisposable
		{
			if (observable != null)
				disposables.Add (observable);

			return observable;
		}
	}
}

