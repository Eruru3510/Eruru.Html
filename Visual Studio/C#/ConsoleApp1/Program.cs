using System;
using System.Collections.Generic;
using Eruru.Html;

namespace ConsoleApp1 {

	class Program {

		static void Main (string[] args) {
			Console.Title = nameof (ConsoleApp1);
			Html html = Html.Load (@"D:\1.html");
			foreach (HtmlElement element in html.QuerySelectorAll (".tbox .ssbox")) {
				HtmlElement titleA = element.QuerySelector (".title a");
				HtmlElement sbarA = element.QuerySelector (".sbar a");
				if (titleA is null || sbarA is null) {
					continue;
				}
				Console.WriteLine ($"{titleA.InnerText}\t{titleA.GetAttributeValue ("href")}\t{sbarA.GetAttributeValue ("href")}");
			}
			TestZhiHu ();
			Console.ReadLine ();
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