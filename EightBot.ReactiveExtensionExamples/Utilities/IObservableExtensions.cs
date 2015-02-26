using System;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Threading;

namespace EightBot.ReactiveExtensionExamples.Utilities
{
	public static class IObservableExtensions
	{
//		public static IObservable<T> Spy<T>(this IObservable<T> source, string opName = null)
//		{
//			opName = opName ?? "IObservable";
//			System.Diagnostics.Debug.WriteLine("{0}: Observable obtained on Thread: {1}",
//				opName,
//				Thread.CurrentThread.ManagedThreadId);
//
//			return Observable.Create<T>(obs =>
//				{
//					System.Diagnostics.Debug.WriteLine("{0}: Subscribed to on Thread: {1}",
//						opName,
//						System.Threading.threa
//						Thread.CurrentThread.ManagedThreadId);
//
//					try
//					{
//						var subscription = source
//							.Do(x => System.Diagnostics.Debug.WriteLine("{0}: OnNext({1}) on Thread: {2}",
//								opName,
//								x,
//								Thread.CurrentThread.ManagedThreadId),
//								ex => System.Diagnostics.Debug.WriteLine("{0}: OnError({1}) on Thread: {2}",
//									opName,
//									ex,
//									Thread.CurrentThread.ManagedThreadId),
//								() => System.Diagnostics.Debug.WriteLine("{0}: OnCompleted() on Thread: {1}",
//									opName,
//									Thread.CurrentThread.ManagedThreadId)
//							)
//							.Subscribe(obs);
//						return new CompositeDisposable(
//							subscription,
//							Disposable.Create(() => System.Diagnostics.Debug.WriteLine(
//								"{0}: Cleaned up on Thread: {1}",
//								opName,
//								Thread.CurrentThread.ManagedThreadId)));
//					}
//					finally
//					{
//						System.Diagnostics.Debug.WriteLine("{0}: Subscription completed.", opName);
//					}
//				});
//		}

		public static IObservable<T> Dump<T>(this IObservable<T> source, string name)
		{
			source.Subscribe(
				i=>System.Diagnostics.Debug.WriteLine("{0}-->{1}", name, i), 
				ex=>System.Diagnostics.Debug.WriteLine("{0} failed-->{1}", name, ex.Message),
				()=>System.Diagnostics.Debug.WriteLine("{0} completed", name));

			return source;
		}
	}
}

