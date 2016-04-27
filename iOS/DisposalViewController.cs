using System;
using UIKit;
using Foundation;
using System.Reactive.Disposables;

namespace EightBot.ReactiveExtensionExamples.iOS
{
	public class DisposalViewController : UIViewController
	{
		public DisposalViewController ()
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		}

		readonly CompositeDisposable disposableNotifications = new CompositeDisposable();

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			disposableNotifications.Add (
				NSNotificationCenter.DefaultCenter
					.AddObserver (
						UIApplication.UserDidTakeScreenshotNotification, 
						_ => System.Diagnostics.Debug.WriteLine ("You Took a Screenshot"))
			);

			disposableNotifications.Add (
				NSNotificationCenter.DefaultCenter
					.AddObserver (
						UIDevice.BatteryLevelDidChangeNotification,
						_ => System.Diagnostics.Debug.WriteLine ("The battery changed"))
			);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			disposableNotifications.Clear ();
		}
	}
}

