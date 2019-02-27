using System;
using System.Net;

namespace TddExMicrotest
{
	public class GameUrls
	{
		public const string WORDS = "https://www.wordgamedictionary.com/word-lists/{0}-letter-words/{0}-letter-words.json";
		public const string OTHER = "https://httpstat.us/{0}";
		public const string TURTLES = "https://www.turtles.com/api/{0}/command/{1}";
	}

	public interface ITheInternet
	{
		string Get(string url, params object[] parameters);
	}

	public class TheInternet : ITheInternet
	{
		public WebClient _client = new WebClient();

		public string Get(string url, params object[] parameters)
		{
			var urlWithValues = string.Format(url, parameters);
			Console.WriteLine("Downloading from " + urlWithValues);
			return _client.DownloadString(urlWithValues);
		}
	}
}