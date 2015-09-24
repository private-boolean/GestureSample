using GestureSample.Backend;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GestureSample.paulPages
{
	public class ImagePage : ContentPage
	{
		Image mImage;

		//BoxView mBox;

		Frame imageFrame;
		Label tapcounter;
		Label headerLabel = new Label
		{
			Text = "HEADER"
		};

		MR.Gestures.AbsoluteLayout tapPointsLayout; // hold tap indicators and image
		ContentView tapPointsLayoutContainer; // hold tapPointsLayout and size it correctly

		int tapIndicatorSize = 10;

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
			// declare buttons
			Button quitButton;

			Button noTargetButton;

			Button doneButton;

			Button resetLabelsButton;

			// initialize buttons
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

			// image to display
			mImage = new Image
			{
				BackgroundColor = Color.Yellow,
				VerticalOptions = LayoutOptions.Fill,
				HorizontalOptions = LayoutOptions.Fill,
				Aspect = Xamarin.Forms.Aspect.AspectFit,
				Source = "http://developer.xamarin.com/demo/IMG_1415.JPG?width=512"
				//Source = "http://130.179.30.84:9998/featurefinder/image/1"
			};

			tapPointsLayout = new MR.Gestures.AbsoluteLayout
			{
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center,
				BackgroundColor = Color.Teal
			};
			tapPointsLayout.Children.Add(mImage);
			tapPointsLayout.Tapped += absoluteLayout_Tapped;

			tapPointsLayoutContainer = new ContentView
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.Fuchsia,
				Content = tapPointsLayout
			};
			tapPointsLayoutContainer.SizeChanged += tapPointsLayoutContainer_SizeChanged;

			Content = new StackLayout
			{
				VerticalOptions = LayoutOptions.Fill,
				Children = {
					headerLabel,

					tapPointsLayoutContainer,

					tapcounter,

					buttonsLayout
				}
			};
		}

		//-------------------
		// UI CALLBACKS
		//-------------------
		void tapPointsLayoutContainer_SizeChanged(object sender, EventArgs e)
		{
			ResizeImage(sender as ContentView);
			//ResetLabels();
		}

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
			tapcounter.Text = "times tapped: " + selectedPoints.Count;

			VisualElement senderView = sender as VisualElement;
			if (senderView != null)
			{
				double viewWidth = senderView.Width;
				double viewHeight = senderView.Height;

				MR.Gestures.TapEventArgs tappedArgs = evt as MR.Gestures.TapEventArgs;
				if (null != tappedArgs)
				{

					Point tapPoint = tappedArgs.Touches[0];
					tapPoint.X = tapPoint.X / viewWidth;
					tapPoint.Y = tapPoint.Y / viewHeight;

					Debug.WriteLine("Tap point: " + tapPoint.X + ", " + tapPoint.Y);
					AddLabelPoint(tapPoint);
				} else
				{
					Debug.WriteLine("tappedargs is wrong type");
				}
			}
			else
				Debug.WriteLine("SenderView is wrong type.");
		}

		//--------------------
		// FUNCTIONALITY
		//--------------------

		void ResizeImage(ContentView mContentView)
		{
			double width = -1.0;
			double height = -1.0;

			if (mContentView.Width > mContentView.Height)
			{
				// landscape: height is limiting factor
				//mImage.HeightRequest = mContentView.Height;
				//mImage.WidthRequest = -1;
				height = mContentView.Height;
				Debug.WriteLine("Landscape");
			}
			else
			{
				// portrait: width is limiting factor
				//mImage.WidthRequest = mContentView.Width;
				//mImage.HeightRequest = -1;
				width = mContentView.Width;
				Debug.WriteLine("Portrait");
			}
			AbsoluteLayout.SetLayoutBounds(mImage, new Rectangle(0, 0, width, height));
		}

		/// <summary>
		/// remove all labels
		/// </summary>
		void ResetLabels()
		{
			selectedPoints = new List<Point>();
			tapPointsLayout.Children.Clear();

			tapPointsLayout.Children.Add(mImage, new Point(0, 0));
			//tapPointsLayout.IsClippedToBounds = true;

			tapcounter.Text = "times tapped: " + selectedPoints.Count;
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
				WidthRequest = tapIndicatorSize,
				HeightRequest = tapIndicatorSize
			};

			Rectangle indicatorPosn = new Rectangle
			{
				X = tapPoint.X,
				Y = tapPoint.Y
			};

			Point imagePoint = TransformToImageCoordinates(tapPoint);

			selectedPoints.Add(imagePoint);

			MR.Gestures.AbsoluteLayout.SetLayoutFlags(indicatorBox, (AbsoluteLayoutFlags.XProportional | AbsoluteLayoutFlags.YProportional));

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

			//LoadImage();

			ServerConnection connection = new ServerConnection();
			connection.SendDataAsync(selectedPoints);
			
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
