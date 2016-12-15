using System;
using Xamarin.Forms;

namespace ReactiveExtensionExamples.Pages
{
	public class NonReactivePage : ContentPage
	{
		int clickCount = 0;

		Label buttonClickedInformation;
		Button myButton;

		public NonReactivePage ()
		{

			myButton = new Button { 
				Text = "This is a button"
			};

			myButton.Clicked += MyButton_Clicked;
			buttonClickedInformation = new Label { };

			Content = new StackLayout { 
				Children = {
					myButton,
					buttonClickedInformation
				}
			};
		}

		void MyButton_Clicked (object sender, EventArgs e)
		{
			clickCount++;
			buttonClickedInformation.Text = string.Format ("Clicked {0} times", clickCount);
		}
	}
}


