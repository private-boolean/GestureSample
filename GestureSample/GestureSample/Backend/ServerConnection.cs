using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace GestureSample.Backend
{
	class ServerConnection
	{

		private HttpClient client;


		public const string hostname = "192.168.32.19";
		//public const string hostname = "130.179.30.84";
		//private const string URL_ROOT = "http://posttestserver.com/post.php?dir=pauliscool";

		public ServerConnection()
		{
			client = new HttpClient();
			client.MaxResponseContentBufferSize = 256000;
		}

		public async Task SendDataAsync(List<Point> points)
		{
			string pointData = "";
			foreach(Point point in points)
			{
				pointData += point.X + "," + point.Y + " ";
			}
			var json = "points:" + pointData;
			StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

			HttpResponseMessage response = null;
			response = await client.PostAsync(URL_ROOT, content);
			
			if (response.IsSuccessStatusCode)
			{
				Debug.WriteLine(@"        Item successfully posted");
			}
		}
		
	}
}
