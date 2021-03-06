﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GestureSample.Views;

using Xamarin.Forms;
using GestureSample.paulPages;

namespace GestureSample
{
	public class App : Application
	{
		public static NavigationPage MainNavigation;

		public App()
		{
			var samplePages = new[] {
				"ContentPage",
				"Layouts",
				"Views",
				"Cells",
				"Tests",
				"Paul"
			};

			var mainPage = new MainPage() { Title = "Control Categories", BindingContext = samplePages };
			//MainPage = MainNavigation = new NavigationPage(mainPage);
			MainPage = new NavigationPage(new LoginPage());
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
