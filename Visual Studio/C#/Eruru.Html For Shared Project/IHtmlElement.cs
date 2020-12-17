using System.Collections.Generic;

namespace Eruru.Html {

	public interface IHtmlElement {

		string InnerHtml { get; }

		string InnerText { get; }

		HtmlElement this[int index] { get; }

		HtmlElement GetElementById (string id);

		HtmlElement GetElementByTagName (string name);

		HtmlElement GetElementByClassName (string name);

		HtmlElement GetElementByName (string name);

		HtmlElement GetElementByAttribute (string name);
		HtmlElement GetElementByAttribute (string name, string value);

		List<HtmlElement> GetElementsByTagName (string name);

		List<HtmlElement> GetElementsByClassName (string name);

		List<HtmlElement> GetElementsByName (string name);

		List<HtmlElement> GetElementsByAttribute (string name);
		List<HtmlElement> GetElementsByAttribute (string name, string value);

		void ForEachNode (HtmlFunc<HtmlElement, bool> func);

		void ForEachTextNode (HtmlFunc<HtmlElement, bool> func);

		void ForEachElement (HtmlFunc<HtmlElement, bool> func);

	}

}