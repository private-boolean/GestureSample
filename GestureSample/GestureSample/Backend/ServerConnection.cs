using GestureSample.Backend.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace GestureSample.Backend
{
	class ServerConnection
	{

		private HttpClient client;


		//public const string hostAddress = "http://192.168.32.21";
		public const string hostAddress = "http://130.179.30.84";
		//private const string URL_ROOT = "http://posttestserver.com/post.php?dir=pauliscool";

		public const string hostname = hostAddress + ":9998/featurefinder";
		public const string imageResource = hostname + "/image/";
		public const string labelResource = hostname + "/label/";

		public ServerConnection()
		{
			client = new HttpClient();
			client.MaxResponseContentBufferSize = 256000;
		}

		public async Task SendDataAsync(List<Point> points)
		{
			string pointData = "";
			foreach (Point point in points)
			{
				pointData += point.X + "," + point.Y + " ";
			}

			FFLabel mLabel = new FFLabel(
				0,
				"lol@lol.ca",
				999,
				LabelType.TARDIGRADE_BODY,
				pointData,
				DateTime.Now,
				0);

			MemoryStream stream1 = new MemoryStream();
			//DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
			//settings.DateTimeFormat = new DateTimeFormat("yyyy-MMM-dd-hh:mm:ss");
			DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(FFLabel));
			ser.WriteObject(stream1, mLabel);
			byte[] streamBytes = stream1.ToArray();
			string jsonString = Encoding.UTF8.GetString(streamBytes, 0, streamBytes.Length);
			Debug.WriteLine("JSON String: " + jsonString);

			// send it off		
			StringContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

			HttpResponseMessage response = null;
			response = await client.PostAsync(labelResource, content);

			if (response.IsSuccessStatusCode)
			{
				Debug.WriteLine(@"        Item successfully posted");
			}
		}

	}
}
