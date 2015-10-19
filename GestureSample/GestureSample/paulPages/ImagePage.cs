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
	public enum Orientation
	{
		LANDSCAPE,
		PORTRAIT
	}

	public class ImagePage : ContentPage
	{

		Orientation lastOrientation;
		Orientation currentOrientation;

		double _width = -1;
		double _height = -1;

		// widgets
		Label tapcounter;
		Label headerLabel = new Label
		{
			Text = "HEADER"
		};

		MR.Gestures.AbsoluteLayout tapPointsLayout;
		ContentView mContentView;
		Image mImage;

		Button quitButton;

		Button noTargetButton;

		Button doneButton;

		Button resetLabelsButton;


		int boxSize = 40;

		/// <summary>
		/// points that have been selected, in image coordinates
		/// </summary>
		List<Point> selectedPoints;

		public ImagePage()
		{
			// Layout controls
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

			tapcounter = new Label
			{
				Text = "TAPPED 0 TIMES"
			};

			ImageSource mImageSource = new UriImageSource
			{
				Uri = new Uri(ServerConnection.imageResource + "1")
			};
			
			mImage = new Image
			{
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				BackgroundColor = Color.Yellow,
				HeightRequest = 200,
				Aspect = Xamarin.Forms.Aspect.AspectFit,
				//Source = "http://developer.xamarin.com/demo/IMG_1415.JPG"
				//Source = ImageSource.FromResource("ResourceBitmapCode.Images.img.jpg")
				Source = mImageSource
			};


			AbsoluteLayout.SetLayoutFlags(mImage, AbsoluteLayoutFlags.None);

			tapPointsLayout = new MR.Gestures.AbsoluteLayout
			{
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center,
				BackgroundColor = Color.Teal
			};
			tapPointsLayout.Tapped += absoluteLayout_Tapped;

			ResetLabels();

			LayoutPage();
		}


		private void LayoutPage()
		{
			StackLayout buttonsLayout = new StackLayout
			{
				IsClippedToBounds = true,
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Children = { quitButton, noTargetButton, resetLabelsButton, doneButton }
			};

			mContentView = new ContentView
			{
				BackgroundColor = Color.Red,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Content = tapPointsLayout
			};
			mContentView.SizeChanged += mContentView_SizeChanged;
			mContentView.PropertyChanging += mContentView_PropertyChanging;


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
			
		}

		void mContentView_PropertyChanging(object sender, PropertyChangingEventArgs e)
		{
			if (e.PropertyName.Equals("Width"))
			{
				//ContentView changedView = sender as ContentView;
				//if (null == changedView)
				//{
				//	Debug.WriteLine("sender is wrong type.");
				//	return;
				//}

				////mImage.HeightRequest = 20;
				////mImage.WidthRequest = 20;

				//if (changedView.Width > changedView.Height)
				//{
				//	Debug.WriteLine("Was landscape");
				//	lastOrientation = Orientation.LANDSCAPE;
				//}
				//else
				//{
				//	Debug.WriteLine("Was portait");
				//	lastOrientation = Orientation.PORTRAIT;
				//}
				//if (lastOrientation == Orientation.PORTRAIT)
				//{
				//	mImage.WidthRequest = 20;
				//	mImage.HeightRequest = 20;
				//}
			}
		}

		protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated(width, height); // Important!

			if (width != _width || height != _height)
			{
				_width = width;
				_height = height;
				Debug.WriteLine("Size Allocated: " + width + "x" + height);

				//LayoutPage();
			}
		}

		async void mContentView_SizeChanged(object sender, EventArgs e)
		{
			ContentView changedView = sender as ContentView;
			if (null == changedView)
			{
				Debug.WriteLine("sender is wrong type.");
				return;
			}

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

			tapPointsLayout.Children.Clear();

			await Task.Delay(200);
			ReloadLabels();
			//AbsoluteLayout.SetLayoutBounds(mImage, new Rectangle(0, 0, changedView.Width, changedView.Height));
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
			MR.Gestures.TapEventArgs tappedArgs = evt as MR.Gestures.TapEventArgs;
			if (null != tappedArgs)
			{
				Point tapPoint = tappedArgs.Touches[0];
				Debug.WriteLine("Tap point: " + tapPoint.X + ", " + tapPoint.Y);
				// make sure that the tap is within the bounds of the view
				float range = boxSize / 2.0f;
				bool isInBounds =
					tapPoint.X - range >= 0 &&
					tapPoint.Y - range >= 0 &&
					tapPoint.X + range <= tapPointsLayout.Width &&
					tapPoint.Y + range <= tapPointsLayout.Height;
				if (isInBounds)
				{
					AddLabelPoint(tapPoint);
				}
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

			tapcounter.Text = "times tapped: " + selectedPoints.Count;
		}

		/// <summary>
		/// re-build the tapPointsLayout
		/// </summary>
		void ReloadLabels()
		{
			tapPointsLayout.Children.Clear();
			tapPointsLayout.Children.Add(mImage, new Point(0, 0));

			ImageDimensions mImageDimensions = GetImageDimensions(mImage.Source);


			double widthRequest = -1.0, heightRequest = -1.0;
			if (currentOrientation == Orientation.LANDSCAPE)
			{
				heightRequest = mContentView.Height;
				widthRequest = mContentView.Height * mImageDimensions.aspectRatio;
			} else if (currentOrientation == Orientation.PORTRAIT)
			{
				heightRequest = mContentView.Width / mImageDimensions.aspectRatio;
				widthRequest = mContentView.Width;
			}

			// make sure it will fit (what if image aspect ratio is weird???)
			if (heightRequest > mContentView.Height)
			{
				heightRequest = mContentView.Height;
				widthRequest = mContentView.Height * mImageDimensions.aspectRatio;
			}
			if (widthRequest > mContentView.Width)
			{
				heightRequest = mContentView.Width / mImageDimensions.aspectRatio;
				widthRequest = mContentView.Width;
			}


			mImage.HeightRequest = heightRequest;
			mImage.WidthRequest = widthRequest;
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

			Point indicatorPosn = new Point
			{
				X = tapPoint.X - (double)boxSize / 2.0,
				Y = tapPoint.Y - (double)boxSize / 2.0
			};

			Point imagePoint = TransformToImageCoordinates(tapPoint);

			selectedPoints.Add(imagePoint);

			tapPointsLayout.Children.Add(indicatorBox, indicatorPosn);
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

		public class ImageDimensions
		{
			public readonly int widthPixels = -1;
			public readonly int heightPixels = -1;

			// width / height
			public readonly double aspectRatio;

			// expand as needed; colour space, etc.

			public ImageDimensions(int widthPixels = -1, int heightPixels = -1)
			{
				this.widthPixels = widthPixels;
				this.heightPixels = heightPixels;
				this.aspectRatio = (double)widthPixels / (double)heightPixels;
			}
		}

		private static ImageDimensions GetImageDimensions(ImageSource mImageSource)
		{
			// for now just assume 455x256, but in future do platform-specific stuff
			return new ImageDimensions(455, 256);
		}
	}
}
