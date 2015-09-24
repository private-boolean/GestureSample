using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Xamarin.Forms;

namespace GestureSample.paulPages
{
	public enum Orientation
	{
		LANDSCAPE,
		PORTRAIT
	}

	public class ImagePage : ContentPage
	{
		Orientation lastOrientation;
		Orientation currentOrientation;

		Image mImage;

		Frame imageFrame;
		Label tapcounter;
		Label headerLabel = new Label
		{
			Text = "HEADER"
		};

		int timesTapped = 0;
		MR.Gestures.AbsoluteLayout tapPointsLayout;
		ContentView mContentView;

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
				//Source = ImageSource.FromResource("ResourceBitmapCode.Images.img.jpg")
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
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center,
				BackgroundColor = Color.Teal
			};

			mContentView = new ContentView
			{
				BackgroundColor = Color.Red,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Content = tapPointsLayout
			};
			mContentView.SizeChanged += mContentView_SizeChanged;
			mContentView.PropertyChanging += mContentView_PropertyChanging;

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

					mContentView,

					tapcounter,

					buttonsLayout
				}
			};

			selectedPoints = new List<Point>();
		}

		void mContentView_PropertyChanging(object sender, PropertyChangingEventArgs e)
		{
			if (e.PropertyName.Equals("Width"))
			{
				ContentView changedView = sender as ContentView;
				if (null == changedView)
				{
					Debug.WriteLine("sender is wrong type.");
					return;
				}

				//mImage.HeightRequest = 20;
				//mImage.WidthRequest = 20;

				if (changedView.Width > changedView.Height)
				{
					Debug.WriteLine("Was landscape");
					lastOrientation = Orientation.LANDSCAPE;
				}
				else
				{
					Debug.WriteLine("Was portait");
					lastOrientation = Orientation.PORTRAIT;
				}
				if (lastOrientation == Orientation.PORTRAIT)
				{
					mImage.WidthRequest = 20;
					mImage.HeightRequest = 20;
				}
			}
		}

		
		void mContentView_SizeChanged(object sender, EventArgs e)
		{
			ContentView changedView = sender as ContentView;
			if (null == changedView)
			{
				Debug.WriteLine("sender is wrong type.");
				return;
			}

			////changedView.Content = null;
			////tapPointsLayout.Children.Clear();

			////Rectangle rect = new Rectangle(0, 0, 3, 3);
			////mImage.SetValue(AbsoluteLayout.LayoutBoundsProperty, rect);

			if (changedView.Width > changedView.Height)
			{
				Debug.WriteLine("Now Landscape");
				currentOrientation = Orientation.LANDSCAPE;
			}
			else
			{
				Debug.WriteLine("Now portrait");
				currentOrientation = Orientation.PORTRAIT;
			}

			// change image
			//if (lastOrientation != currentOrientation)
			//{
			//	mImage.HeightRequest = 20;
			//	mImage.WidthRequest = 20;

			//	//changedView.ForceLayout();
			//} else
			//{
				if (currentOrientation == Orientation.PORTRAIT)
				{
					mImage.WidthRequest = changedView.Width;
					mImage.HeightRequest = -1;
				} else if (currentOrientation == Orientation.LANDSCAPE)
				{
					mImage.HeightRequest = changedView.Height;
					mImage.WidthRequest = -1;
				}
			//}
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
