using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Xamarin.Forms;

namespace GestureSample.paulPages
{
	public class NewImagePage : ContentPage
	{

		ContentView imageContainer;
		AbsoluteLayout tapPointsLayout;
		Image mImage;

		public NewImagePage()
		{
			mImage = new Image
			{
				Source = "http://developer.xamarin.com/demo/IMG_1415.JPG?width=512"
			};

			tapPointsLayout = new AbsoluteLayout
			{
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center,
				BackgroundColor = Color.Teal
			};
			tapPointsLayout.Children.Add(mImage);

			imageContainer = new ContentView
			{
				Content = tapPointsLayout,
				VerticalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.Yellow
			};
			imageContainer.SizeChanged += imageContainer_SizeChanged;

			Content = new StackLayout
			{
				Children = {
					new Label { Text = "Hello ContentPage" },
					imageContainer,
					new Label { Text = "PAUL IS COOL"}
				}
			};

			//Content.SizeChanged += Content_SizeChanged;
		}

		//void Content_SizeChanged(object sender, EventArgs e)
		//{
		//	Debug.WriteLine("Changed");
		//}

		void imageContainer_SizeChanged(object sender, EventArgs e)
		{
			//tapPointsLayout.Children.Clear();

			//if (tapPointsLayout.Children.Count > 0)
			//{
			//	tapPointsLayout.Children.Clear();
			//} else
			//{
			//	tapPointsLayout.Children.Add(mImage);
			//}

			double widthUpd = ((ContentView)sender).Width;
			double heightUpd = ((ContentView)sender).Height;

			if (widthUpd > heightUpd)
			{
				mImage.HeightRequest = heightUpd;
				mImage.WidthRequest = -1.0;
			} else
			{
				mImage.WidthRequest = widthUpd;
				mImage.HeightRequest = -1.0;
			}
		}
	}
}
