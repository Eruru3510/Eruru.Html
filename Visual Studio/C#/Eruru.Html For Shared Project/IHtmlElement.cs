using System.Collections.Generic;

namespace Eruru.Html {

	public interface IHtmlElement {

		string InnerHtml { get; }
		string InnerText { get; }
		HtmlElement this[int index] { get; }

		HtmlElement GetElementById (string id, int maxDepth = -1);

		HtmlElement GetElementByTagName (string name, int maxDepth = -1);

		HtmlElement GetElementByClassName (string name, int maxDepth = -1);

		HtmlElement GetElementByName (string name, int maxDepth = -1);

		HtmlElement GetElementByAttribute (string name, int maxDepth = -1);
		HtmlElement GetElementByAttribute (string name, string value, int maxDepth = -1);

		List<HtmlElement> GetElementsByTagName (string name, int maxDepth = -1);

		List<HtmlElement> GetElementsByClassName (string name, int maxDepth = -1);

		List<HtmlElement> GetElementsByName (string name, int maxDepth = -1);

		List<HtmlElement> GetElementsByAttribute (string name, int maxDepth = -1);
		List<HtmlElement> GetElementsByAttribute (string name, string value, int maxDepth = -1);

		HtmlElement QuerySelector (string path);

		List<HtmlElement> QuerySelectorAll (string path);

		void ForEachNode (HtmlFunc<HtmlElement, bool> func, int maxDepth = -1);

		void ForEachTextNode (HtmlFunc<HtmlElement, bool> func, int maxDepth = -1);

		void ForEachElement (HtmlFunc<HtmlElement, bool> func, int maxDepth = -1);

	}

}