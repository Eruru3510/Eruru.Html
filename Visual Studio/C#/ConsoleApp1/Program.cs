using System;
using System.Collections.Generic;
using Eruru.Html;

namespace ConsoleApp1 {

	class Program {

		static void Main (string[] args) {
			Console.Title = nameof (ConsoleApp1);
			Html html = Html.Load (@"D:\1.html");
			foreach (HtmlElement element in html.QuerySelectorAll (".tbox .ssbox .title a,ul[class=links] li")) {
				Console.WriteLine (element.InnerText);
			}
			Test ();
			TestZhiHu ();
			Console.ReadLine ();
		}

		static void Test () {
			Html html = Html.Load (@"D:\1.html");
			List<HtmlElement> tboxes = html.GetElementsByAttribute ("class", "tbox");
			List<HtmlElement> ssboxes = tboxes[1].GetElementsByClassName ("ssbox");
			foreach (HtmlElement ssbox in ssboxes) {
				HtmlElement title = ssbox.GetElementByClassName ("title");
				HtmlElement sbar = ssbox.GetElementByClassName ("sbar");
				Console.WriteLine (title.GetElementByTagName ("a").GetAttributeValue ("href"));
				Console.WriteLine (sbar.GetElementByTagName ("a").GetAttributeValue ("href"));
			}
		}

		static void TestZhiHu () {
			Html html = Html.Load (@"..\..\..\Assets\www.zhihu.com.html");
			List<HtmlElement> cardsElement = html.GetElementsByClassName ("Card TopstoryItem TopstoryItem--old TopstoryItem-isRecommend");
			foreach (HtmlElement cardElement in cardsElement) {
				foreach (HtmlElement aElement in cardElement.GetElementsByTagName ("a")) {
					Console.WriteLine (aElement.InnerText);
				}
			}
		}

	}

}