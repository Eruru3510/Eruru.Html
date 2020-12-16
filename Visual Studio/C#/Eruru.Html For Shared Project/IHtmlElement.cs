namespace Eruru.Html {

	public interface IHtmlElement {

		string InnerHtml { get; }

		HtmlElement this[int index] { get; }

		HtmlElement GetElementById (string id);

		HtmlElement GetElementByTagName (string name);

		HtmlElement GetElementByClassName (string name);

		HtmlElement GetElementByName (string name);

		HtmlElement GetElementByAttribute (string name);
		HtmlElement GetElementByAttribute (string name, string value);

		HtmlElement[] GetElementsByTagName (string name);

		HtmlElement[] GetElementsByClassName (string name);

		HtmlElement[] GetElementsByName (string name);

		HtmlElement[] GetElementsByAttribute (string name);
		HtmlElement[] GetElementsByAttribute (string name, string value);

		bool ForEachElement (HtmlFunc<HtmlElement, bool> func);

	}

}