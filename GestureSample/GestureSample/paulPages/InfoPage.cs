using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GestureSample.paulPages
{
	public class InfoPage : ContentPage
	{
		Button okButton;

		public InfoPage()
		{
			okButton = new Button
			{
				Text = "OK",
			};
			okButton.Clicked += okButton_Clicked;


			Content = new StackLayout
			{
				Children = {
					new Label { Text = "Please identify tardigrade bodies" },
					okButton
				}
			};
		}

		void okButton_Clicked(object sender, EventArgs e)
		{
			LoadNextPageAsync();
		}

		async void LoadNextPageAsync()
		{
			await Navigation.PushModalAsync(new ImagePage());
		}
	}
}
