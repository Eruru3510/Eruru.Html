using System;
using Eruru.Html;

namespace ConsoleApp1 {

	class Program {

		static void Main (string[] args) {
			Console.Title = nameof (ConsoleApp1);
			HtmlDocument htmlDocument = HtmlDocument.Load (@"D:\Untitled-1.html");
			Console.WriteLine (htmlDocument.QuerySelector ("#a").NextSibling.TextContent);
			Console.ReadLine ();
		}

	}

}