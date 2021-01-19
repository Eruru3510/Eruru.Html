namespace Eruru.Html {

	public class HtmlComment : HtmlNode {

		public HtmlComment (string nodeValue, HtmlElement parentElement) : base (HtmlNodeType.Comment, "#comment", nodeValue, parentElement) {

		}

	}

}