using System;
using System.Collections.Generic;
using System.IO;

namespace Eruru.Html {

	public class HtmlDocument : HtmlNode, IHtmlDocument {

		public HtmlElement Html { get; private set; }
		public HtmlElement Head { get; private set; }
		public HtmlElement Body { get; private set; }

		internal HtmlElement Root;

		public HtmlDocument () : base (HtmlNodeType.Document, "#document", null, null) {

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
			document.Root = new HtmlElement (document.ChildNodes);
			document.Html = document.GetElementByTagName (HtmlKeyword.Html);
			document.Head = document.GetElementByTagName (HtmlKeyword.Head);
			document.Body = document.GetElementByTagName (HtmlKeyword.Body);
			return document;
		}

		public new HtmlElement GetElementById (string id, int maxDepth = -1) {
			if (id is null) {
				throw new ArgumentNullException (nameof (id));
			}
			return base.GetElementById (id, maxDepth);
		}

		public new HtmlElement GetElementByTagName (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return base.GetElementByTagName (name, maxDepth);
		}

		public new HtmlElement GetElementByClassName (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return base.GetElementByClassName (name, maxDepth);
		}

		public new HtmlElement GetElementByName (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return base.GetElementByName (name, maxDepth);
		}

		public new HtmlElement GetElementByAttribute (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return base.GetElementByAttribute (name, maxDepth);
		}

		public new HtmlElement GetElementByAttribute (string name, string value, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return base.GetElementByAttribute (name, maxDepth);
		}

		public new List<HtmlElement> GetElementsByTagName (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return base.GetElementsByTagName (name, maxDepth);
		}

		public new List<HtmlElement> GetElementsByClassName (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return base.GetElementsByClassName (name, maxDepth);
		}

		public new List<HtmlElement> GetElementsByName (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return base.GetElementsByName (name, maxDepth);
		}

		public new List<HtmlElement> GetElementsByAttribute (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return base.GetElementsByAttribute (name, maxDepth);
		}

		public new List<HtmlElement> GetElementsByAttribute (string name, string value, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return base.GetElementsByAttribute (name, maxDepth);
		}

		public new HtmlElement QuerySelector (string path) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			return base.QuerySelector (path);
		}

		public new List<HtmlElement> QuerySelectorAll (string path) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			return base.QuerySelectorAll (path);
		}

	}

}