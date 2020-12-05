namespace Eruru.Html {

	public enum HtmlElementType {

		Root = 1 << 0,
		Define = 1 << 1,
		Single = 1 << 2,
		Double = 1 << 3,
		Text = 1 << 4,
		Comment = 1 << 5,
		Tag = Define | Single | Double

	}

}