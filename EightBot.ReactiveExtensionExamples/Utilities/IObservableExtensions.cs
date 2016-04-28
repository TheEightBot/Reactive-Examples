using System;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Threading;

namespace EightBot.ReactiveExtensionExamples.Utilities
{
	public static class IObservableExtensions
	{
		public static TDisposable DisposeWith<TDisposable> (this TDisposable observable, CompositeDisposable disposables) where TDisposable : class, IDisposable
		{
			if (observable != null)
				disposables.Add (observable);

			return observable;
		}
	}
}

