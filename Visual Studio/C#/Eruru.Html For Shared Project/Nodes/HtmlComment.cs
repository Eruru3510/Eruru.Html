namespace Eruru.Html {

	public class HtmlComment : HtmlNode {

		public HtmlComment (string nodeValue) : base (HtmlNodeType.Comment, "#comment", nodeValue) {

		}

	}

}