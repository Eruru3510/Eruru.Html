using System;
using Eruru.Html;

namespace ConsoleApp1 {

	class Program {

		static void Main (string[] args) {
			Console.Title = nameof (ConsoleApp1);
			try {
				Html html = Html.Load (@"..\..\..\Assets\www.zhihu.com.html");
				HtmlElement[] cardsElement = html.GetElementsByClassName ("Card TopstoryItem TopstoryItem--old TopstoryItem-isRecommend");
				foreach (HtmlElement cardElement in cardsElement) {
					foreach (HtmlElement aElement in cardElement.GetElementsByTagName ("a")) {
						Console.WriteLine (aElement.InnerHtml);
					}
				}
			} catch (Exception exception) {
				Console.WriteLine (exception);
			}
			Console.ReadLine ();
		}

	}

}