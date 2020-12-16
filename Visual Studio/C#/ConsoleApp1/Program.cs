using System;
using Eruru.Html;

namespace ConsoleApp1 {

	class Program {

		static void Main (string[] args) {
			Console.Title = nameof (ConsoleApp1);
			Html html = Html.Load (@"D:\1.txt");
			HtmlElement[] tboxes = html.GetElementsByAttribute ("class", "tbox");
			foreach (HtmlElement ssbox in tboxes[1].GetElementsByClassName ("ssbox")) {
				HtmlElement title = ssbox.GetElementByClassName ("title");
				HtmlElement sbar = ssbox.GetElementByClassName ("sbar");
				Console.WriteLine (title.GetElementByTagName ("a").GetAttributeValue ("href"));
				Console.WriteLine (sbar.GetElementByTagName ("a").GetAttributeValue ("href"));
			}
			Console.ReadLine ();
		}

		static void TestZhiHu () {
			Html html = Html.Load (@"..\..\..\Assets\www.zhihu.com.html");
			HtmlElement[] cardsElement = html.GetElementsByClassName ("Card TopstoryItem TopstoryItem--old TopstoryItem-isRecommend");
			foreach (HtmlElement cardElement in cardsElement) {
				foreach (HtmlElement aElement in cardElement.GetElementsByTagName ("a")) {
					Console.WriteLine (aElement.InnerHtml);
				}
			}
		}

	}

}