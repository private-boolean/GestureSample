using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace GestureSample.paulPages
{
	public class LoginPage : ContentPage
	{
		public Entry emailEntry;
		public Entry passwordEntry;

		public Button okButton;

		public LoginPage()
		{
			emailEntry = new Entry
			{
				Keyboard = Keyboard.Email,
				Placeholder = "Email address"
			};

			passwordEntry = new Entry
			{
				IsPassword = true,
				Placeholder = "Password"
			};

			okButton = new Button
			{
				Text = "Login"
			};
			okButton.Clicked += okButton_Clicked;


			Content = new StackLayout
			{
				Children = {
					new Label { Text = "Hello ContentPage" },
					emailEntry,
					passwordEntry,
					okButton
				}
			};
		}

		void okButton_Clicked(object sender, EventArgs e)
		{
			Navigation.PushModalAsync(new InfoPage());
		}
	}
}
