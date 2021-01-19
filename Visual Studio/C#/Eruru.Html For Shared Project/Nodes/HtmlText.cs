namespace Eruru.Html {

	public class HtmlText : HtmlNode {

		public HtmlText (string nodeValue, HtmlElement parentElement) : base (HtmlNodeType.Text, "#text", nodeValue, parentElement) {

		}

	}

}