using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Xamarin.Forms;

namespace GestureSample.paulPages
{
	public class ImagePage : ContentPage
	{
		Image mImage;

		Frame imageFrame;
		Label tapcounter;
		Label headerLabel = new Label
		{
			Text = "HEADER"
		};

		int timesTapped = 0;
		MR.Gestures.AbsoluteLayout tapPointsLayout;

		int boxSize = 10;

		/// <summary>
		/// points that have been selected, in image coordinates
		/// </summary>
		List<Point> selectedPoints;

		public ImagePage()
		{
			LayoutPage();
		}


		private void LayoutPage()
		{
			// Layout controls
			Button quitButton;

			Button noTargetButton;

			Button doneButton;

			Button loadImageButton;

			Button resetLabelsButton;


			quitButton = new Button
			{
				Text = "Quit"
			};
			quitButton.Clicked += quitButton_Clicked;

			noTargetButton = new Button
			{
				Text = "No Target"
			};
			noTargetButton.Clicked += noTargetButton_Clicked;

			doneButton = new Button
			{
				Text = "Done"
			};
			doneButton.Clicked += doneButton_Clicked;

			resetLabelsButton = new Button
			{
				Text = "Reset"
			};
			resetLabelsButton.Clicked += resetLabelsButton_Clicked;

			mImage = new Image
			{
				HeightRequest = 200,
				BackgroundColor = Color.Yellow,
				//Aspect = Xamarin.Forms.Aspect.AspectFit,
				Source = "http://developer.xamarin.com/demo/IMG_1415.JPG"
			};

			AbsoluteLayout.SetLayoutFlags(mImage, AbsoluteLayoutFlags.None);

			StackLayout buttonsLayout = new StackLayout
			{
				IsClippedToBounds = true,
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Children = { quitButton, noTargetButton, resetLabelsButton, doneButton }
			};

			tapcounter = new Label
			{
				Text = "TAPPED 0 TIMES"
			};

			tapPointsLayout = new MR.Gestures.AbsoluteLayout
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.Fill,
				BackgroundColor = Color.Teal
			};

			tapPointsLayout.Tapped += absoluteLayout_Tapped;

			imageFrame = new Frame
			{
				Padding = new Thickness(5),
				
				IsClippedToBounds = true,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Content = mImage,
				BackgroundColor = Color.Yellow
			};

			ResetLabels();

			Content = new StackLayout
			{
				VerticalOptions = LayoutOptions.Fill,
				Children = {
					headerLabel,

					tapPointsLayout,

					tapcounter,

					buttonsLayout
				}
			};

			selectedPoints = new List<Point>();
		}

		//-------------------
		// UI CALLBACKS
		//-------------------
		void resetLabelsButton_Clicked(object sender, EventArgs e)
		{
			ResetLabels();
		}

		void quitButton_Clicked(object sender, EventArgs e)
		{
			Quit();
		}

		void noTargetButton_Clicked(object sender, EventArgs e)
		{
			NoTarget();
		}

		void doneButton_Clicked(object sender, EventArgs e)
		{
			Done();
		}

		void loadImageButton_Clicked(object sender, EventArgs e)
		{
			LoadImage();
		}

		void absoluteLayout_Tapped(object sender, EventArgs evt)
		{
			timesTapped++;
			tapcounter.Text = "times tapped: " + timesTapped;

			Debug.WriteLine("Times tapped: " + timesTapped);
			Debug.WriteLine("Image width: " + mImage.Width);
			Debug.WriteLine("Image req width: " + mImage.WidthRequest);
			
			MR.Gestures.TapEventArgs tappedArgs = evt as MR.Gestures.TapEventArgs;
			if (null != tappedArgs)
			{
				Point tapPoint = tappedArgs.Touches[0];
				Debug.WriteLine("Tap point: " + tapPoint.X + ", " + tapPoint.Y);
				AddLabelPoint(tapPoint);
			}
		}

		//--------------------
		// FUNCTIONALITY
		//--------------------

		/// <summary>
		/// remove all labels
		/// </summary>
		void ResetLabels()
		{
			selectedPoints = new List<Point>();
			tapPointsLayout.Children.Clear();
			tapPointsLayout.Children.Add(mImage, new Point(0, 0));
			//tapPointsLayout.IsClippedToBounds = true;
		}

		/// <summary>
		/// transform a tapped point to image coordinates, and store the point.
		/// </summary>
		/// <param name="tapPoint"></param>
		void AddLabelPoint(Point tapPoint)
		{
			// draw a boxview on the point that got tapped
			BoxView indicatorBox = new BoxView
			{
				Color = Color.Accent,
				WidthRequest = boxSize,
				HeightRequest = boxSize
			};

			Rectangle indicatorPosn = new Rectangle
			{
				X = tapPoint.X,
				Y = tapPoint.Y
			};

			Point imagePoint = TransformToImageCoordinates(tapPoint);

			selectedPoints.Add(imagePoint);

			tapPointsLayout.Children.Add(indicatorBox, tapPoint);
		}

		/// <summary>
		/// Load an image to display
		/// </summary>
		void LoadImage()
		{
			//mImage.Source = new UriImageSource
			//{
			//	Uri = new Uri("http://130.179.130.179:9998/jsonmoxy/image/" + nextImage)
			//};
		}

		/// <summary>
		/// stop labelling, go to main menu. Do not save results.
		/// </summary>
		void Quit()
		{
			Debug.WriteLine("Quit");
			headerLabel.Text = "QUIT";
		}

		/// <summary>
		/// User could not find a target
		/// </summary>
		void NoTarget()
		{
			Debug.WriteLine("No target found");
			headerLabel.Text = "NO TARGET";
		}

		/// <summary>
		/// Save results and advance to next image, if there is one
		/// </summary>
		void Done()
		{
			//nextImage++;

			LoadImage();
		}

		/// <summary>
		/// transform a point from screen-space to image space by scaling it according to image scaling/translation
		/// (Until this is implemented, just return a copy of the original point)
		/// </summary>
		/// <param name="tapPoint">point as tapped on screen</param>
		/// <returns>the corresponding point in image-space coordinates</returns>
		private Point TransformToImageCoordinates(Point tapPoint)
		{
			return new Point(tapPoint.X, tapPoint.Y);
		}
	}
}
