using System;
using System.Collections.Generic;
using System.IO;

namespace Eruru.Html {

	public class HtmlDocument : HtmlNode, IHtmlDocument {

		public HtmlElement Html { get; private set; }
		public HtmlElement Head { get; private set; }
		public HtmlElement Body { get; private set; }

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
			document.ForEachElement (element => {
				if (HtmlApi.Equals (element.LocalName, HtmlKeyword.Html)) {
					document.Html = element;
					return false;
				}
				return true;
			});
			if (document.Html is null) {
				throw new Exception ($"文档缺少名为\"{HtmlKeyword.Html}\"的根标签");
			}
			document.Head = document.GetElementByTagName (HtmlKeyword.Head);
			document.Body = document.GetElementByTagName (HtmlKeyword.Body);
			return document;
		}

		public HtmlElement GetElementById (string id, int maxDepth = -1) {
			if (id is null) {
				throw new ArgumentNullException (nameof (id));
			}
			return Html.GetElementById (id, maxDepth);
		}

		public HtmlElement GetElementByTagName (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return Html.GetElementByTagName (name, maxDepth);
		}

		public HtmlElement GetElementByClassName (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return Html.GetElementByClassName (name, maxDepth);
		}

		public HtmlElement GetElementByName (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return Html.GetElementByName (name, maxDepth);
		}

		public HtmlElement GetElementByAttribute (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return Html.GetElementByAttribute (name, maxDepth);
		}

		public HtmlElement GetElementByAttribute (string name, string value, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return Html.GetElementByAttribute (name, maxDepth);
		}

		public List<HtmlElement> GetElementsByTagName (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return Html.GetElementsByTagName (name, maxDepth);
		}

		public List<HtmlElement> GetElementsByClassName (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return Html.GetElementsByClassName (name, maxDepth);
		}

		public List<HtmlElement> GetElementsByName (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return Html.GetElementsByName (name, maxDepth);
		}

		public List<HtmlElement> GetElementsByAttribute (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return Html.GetElementsByAttribute (name, maxDepth);
		}

		public List<HtmlElement> GetElementsByAttribute (string name, string value, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return Html.GetElementsByAttribute (name, maxDepth);
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