using System;
using System.Collections.Generic;
using System.IO;

namespace Eruru.Html {

	public class HtmlDocument : HtmlNode {

		public HtmlElement Html { get; private set; }
		public HtmlElement Head { get; private set; }
		public HtmlElement Body { get; private set; }

		public HtmlDocument () : base (HtmlNodeType.Document, "#document", null) {

		}

		public static implicit operator HtmlDocument (string text) {
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}
			return Parse (text);
		}

		public static HtmlDocument Parse (string text) {
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}
			return Load (new StringReader (text));
		}

		public static HtmlDocument Load (string path) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			return Load (new StreamReader (path));
		}

		public static HtmlDocument Load (TextReader textReader) {
			if (textReader is null) {
				throw new ArgumentNullException (nameof (textReader));
			}
			HtmlDocument document = new HtmlDocument ();
			using (HtmlTagReader reader = new HtmlTagReader (textReader)) {
				while (reader.ReadNode (out HtmlNode node)) {
					if (node is null) {
						continue;
					}
					document.ChildNodes.Add (node);
				}
			}
			document.Html = document.GetElementByTagName (HtmlKeyword.Html) ?? throw new Exception ($"文档缺少名为\"{HtmlKeyword.Html}\"的根标签");
			document.Head = document.GetElementByTagName (HtmlKeyword.Head);
			document.Body = document.GetElementByTagName (HtmlKeyword.Body);
			return document;
		}

		public HtmlElement QuerySelector (string path) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			return Html.QuerySelector (path);
		}

		public List<HtmlElement> QuerySelectorAll (string path) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			return Html.QuerySelectorAll (path);
		}

	}

}