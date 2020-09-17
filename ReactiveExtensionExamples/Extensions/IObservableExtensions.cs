using System;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Threading;
using Xamarin.Forms;
using ReactiveUI;
using System.Reactive;

namespace ReactiveExtensionExamples
{
	public static class IObservableExtensions
	{
		public static TDisposable DisposeWith<TDisposable> (this TDisposable observable, CompositeDisposable disposables) where TDisposable : class, IDisposable
		{
			if (observable != null)
				disposables.Add (observable);

			return observable;
		}

		public static IObservable<Unit> SelectUnit<TDoneCare>(this IObservable<TDoneCare> observable)
        {
			return observable.Select(x => Unit.Default);
        }

		public static IObservable<TValue> IsNotNull<TValue> (this IObservable<TValue> observable)
			where TValue : class
		{
			return observable.Where(x => x != null);
		}

		public static IDisposable NavigateTo <T> (this IObservable<T> observable, VisualElement ve, Func<T, Page> createPage, bool animated = true)
		{
			return observable
				.ObserveOn(RxApp.TaskpoolScheduler)
				.Select(x => createPage(x))
				.ObserveOn(RxApp.MainThreadScheduler)
				.Do(async page => await ve.Navigation.PushAsync(page, animated))
				.Subscribe();
		}
	}
}

