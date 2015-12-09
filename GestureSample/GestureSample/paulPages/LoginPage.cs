using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Emit;
using System.Text;
using Xamarin.Forms;
using Flurl;
using System.Diagnostics;
using Sockets.Plugin;
using Sockets.Plugin.Abstractions;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using GestureSample.Backend.Data;

namespace GestureSample.paulPages
{
	public class LoginPage : ContentPage
	{
		private const string APP_ID = "35834182346-pcrulft4sn6be6voqjegihmkhq9dodlp.apps.googleusercontent.com";
		private const string CLIENT_SECRET = "XMJw91EzbEiCTmkM3Ah5RtwV";
		private const int LISTEN_PORT = 9989;

		public Entry emailEntry;
		public Entry passwordEntry;

		public Button okButton;
		public Button loginButton;

		public WebView webView;


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
				Text = "Proceed"
			};
			okButton.Clicked += okButton_Clicked;

			loginButton = new Button
			{
				Text = "Login with Google"
			};
			loginButton.Clicked += loginButton_Clicked;

			webView = new WebView
			{
				Source = "http://google.ca",
				WidthRequest = 100,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			Content = new StackLayout
			{
				Children = {
					new Label { Text = "Hello ContentPage" },
					//emailEntry,
					//passwordEntry,
					loginButton,
					okButton,
					webView
				}
			};

			webView.IsVisible = false;
		}

		async void DidConnect(object sender, TcpSocketListenerConnectEventArgs args)
		{
			ITcpSocketClient client = args.SocketClient;

			StreamReader reader = new StreamReader(client.ReadStream);
			int numLines = 10;
			int linesRead = 0;
			try
			{
				
				while (linesRead < numLines )
				{
					string line = reader.ReadLine();
					await HandleHttpAuthCode(line, linesRead);
					//Debug.WriteLine(line);
					linesRead++;
				}
			} catch (IOException)
			{
				Debug.WriteLine("-----------------REACHED END YO. read " + linesRead + " lines -------------");
			}
		}

		private async Task HandleHttpAuthCode(string responseLine, int lineNumber)
		{
			Debug.WriteLine("Parsing... " + responseLine);
			string[] stringTokens= responseLine.Split(new char[] { ' ' });
			switch(lineNumber)
			{
				case 0:
					string codeString = stringTokens[1].Substring(7);
					GoogleOauthResponse resp = await RequestToken(codeString);
					//TODO: store token stuff. for now just display it to show that it works.
					Debug.WriteLine("Received Token: " + resp.access_token);
					Device.BeginInvokeOnMainThread(() => DisplayAlert("Received Token!", "token: " + resp.access_token + "\nexpires: " + resp.expires_in + "\nrefresh token: " + resp.refresh_token, "Sweet.")); // don't care about user response. no need to await.
					break;
				default:
					// do nothing
					break;
			}
		}

		private async Task<GoogleOauthResponse> RequestToken(string authorizationString)
		{
			Debug.WriteLine("CODE REQUEST:" + authorizationString);

			HttpClient mClient = new HttpClient();
			mClient.BaseAddress =  new Uri("https://www.googleapis.com/");
			mClient.DefaultRequestHeaders.Accept.Clear();
			mClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


			StringBuilder tokenRequestStringBuilder = new StringBuilder();
			tokenRequestStringBuilder.Append("code=" + authorizationString + "&");
			tokenRequestStringBuilder.Append("client_id=" + APP_ID + "&");
			tokenRequestStringBuilder.Append("client_secret=" + CLIENT_SECRET + "&");
			tokenRequestStringBuilder.Append("redirect_uri=http://localhost:" + LISTEN_PORT + "&");
			tokenRequestStringBuilder.Append("grant_type=authorization_code");

			StringContent mHttpContent = new StringContent(tokenRequestStringBuilder.ToString());
			mHttpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
			string httpContentString = await mHttpContent.ReadAsStringAsync();
			Debug.WriteLine("HTTP REQUEST:-------------------\n" + httpContentString);
			HttpResponseMessage response = await mClient.PostAsync("oauth2/v4/token", mHttpContent);
			//Debug.WriteLine("TOKEN REQUEST RESPONSE: _-----------------\n" + response.Content);
			string responseJsonString = await response.Content.ReadAsStringAsync();
			GoogleOauthResponse responseDataStruct = JsonConvert.DeserializeObject<GoogleOauthResponse>(responseJsonString);
			return responseDataStruct;
		}

		async void loginButton_Clicked(object sender, EventArgs e)
		{
			StringBuilder sb = new StringBuilder("https://accounts.google.com/o/oauth2/auth");
			// append query strings
			sb.Append("?");
			sb.Append("scope=email%20profile&");
			sb.Append("redirect_uri=http://localhost:" + LISTEN_PORT + "&");
			sb.Append("response_type=code&");
			sb.Append("client_id=" + APP_ID);

			Debug.WriteLine("url: " + sb.ToString());

			webView.Source = sb.ToString();
			webView.IsVisible = true;


			TcpSocketListener listener = new TcpSocketListener();

			listener.ConnectionReceived += DidConnect;

			await listener.StartListeningAsync(LISTEN_PORT);


			//HttpClient mClient = new HttpClient
			//{
			//	BaseAddress = new Uri("https://accounts.google.com/o/oauth2/auth")
			//};

			//http://stackoverflow.com/questions/17096201/build-query-string-for-system-net-httpclient-get
		}

		void okButton_Clicked(object sender, EventArgs e)
		{
			Navigation.PushModalAsync(new InfoPage());
		}
	}
}
