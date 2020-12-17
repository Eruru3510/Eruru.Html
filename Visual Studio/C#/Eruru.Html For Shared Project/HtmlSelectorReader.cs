using System;
using System.IO;
using Eruru.TextTokenizer;

namespace Eruru.Html {

	public class HtmlSelectorReader : TextTokenizer<HtmlTokenType> {

		public HtmlSelectorReader (TextReader textReader) : base (
			HtmlTokenType.End,
			HtmlTokenType.String,
			HtmlTokenType.Integer,
			HtmlTokenType.Decimal,
			HtmlTokenType.String
		) {
			if (textReader is null) {
				throw new ArgumentNullException (nameof (textReader));
			}
			TextReader = textReader;
			Add (HtmlKeyword.EqualSign, HtmlTokenType.EqualSign);
			Add (HtmlKeyword.Comma, HtmlTokenType.Comma);
			Add (HtmlKeyword.NumberSign, HtmlTokenType.NumberSign);
			Add (HtmlKeyword.Dot, HtmlTokenType.Dot);
			Add (HtmlKeyword.LeftBracket, HtmlTokenType.LeftBracket);
			Add (HtmlKeyword.RightBracket, HtmlTokenType.RightBracket);
		}

	}

}