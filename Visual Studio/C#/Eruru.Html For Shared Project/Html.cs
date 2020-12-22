using System;
using System.Collections.Generic;
using System.IO;

namespace Eruru.Html {

	public class Html : IHtmlElement {

		public HtmlElement Root { get; } = new HtmlElement ();
		public HtmlElement HtmlElement { get; private set; }
		public HtmlElement Head { get; private set; }
		public HtmlElement Body { get; private set; }

		public static Html Parse (string text) {
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}
			return Load (new StringReader (text));
		}

		public static Html Load (string path) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			return Load (new StreamReader (path));
		}

		public static Html Load (TextReader textReader) {
			if (textReader is null) {
				throw new ArgumentNullException (nameof (textReader));
			}
			Html html = new Html ();
			using (HtmlTagReader reader = new HtmlTagReader (textReader)) {
				while (reader.ReadElement (out HtmlElement element)) {
					if (element is null) {
						continue;
					}
					html.Root.Elements.Add (element);
				}
			}
			html.HtmlElement = html.GetElementByTagName (HtmlKeyword.Html);
			html.Head = html.GetElementByTagName (HtmlKeyword.Head);
			html.Body = html.GetElementByTagName (HtmlKeyword.Body);
			return html;
		}

		public static implicit operator Html (string text) {
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}
			return Html.Parse (text);
		}

		#region IHtmlElement

		public string InnerHtml {

			get => Root.InnerHtml;

		}
		public string InnerText {

			get => Root.InnerText;

		}
		public HtmlElement this[int index] {

			get => Root[index];

		}

		public HtmlElement GetElementById (string id, int maxDepth = -1) {
			if (id is null) {
				throw new ArgumentNullException (nameof (id));
			}
			return Root.GetElementById (id, maxDepth);
		}

		public HtmlElement GetElementByTagName (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return Root.GetElementByTagName (name, maxDepth);
		}

		public HtmlElement GetElementByClassName (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return Root.GetElementByClassName (name, maxDepth);
		}

		public HtmlElement GetElementByName (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return Root.GetElementByName (name, maxDepth);
		}

		public HtmlElement GetElementByAttribute (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return Root.GetElementByAttribute (name, maxDepth);
		}
		public HtmlElement GetElementByAttribute (string name, string value, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			if (value is null) {
				throw new ArgumentNullException (nameof (value));
			}
			return Root.GetElementByAttribute (name, value, maxDepth);
		}

		public List<HtmlElement> GetElementsByTagName (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return Root.GetElementsByTagName (name, maxDepth);
		}

		public List<HtmlElement> GetElementsByClassName (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return Root.GetElementsByClassName (name, maxDepth);
		}

		public List<HtmlElement> GetElementsByName (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return Root.GetElementsByName (name, maxDepth);
		}

		public List<HtmlElement> GetElementsByAttribute (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return Root.GetElementsByAttribute (name, maxDepth);
		}
		public List<HtmlElement> GetElementsByAttribute (string name, string value, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			if (value is null) {
				throw new ArgumentNullException (nameof (value));
			}
			return Root.GetElementsByAttribute (name, value, maxDepth);
		}

		public void ForEachNode (HtmlFunc<HtmlElement, bool> func, int maxDepth = -1) {
			if (func is null) {
				throw new ArgumentNullException (nameof (func));
			}
			Root.ForEachNode (func, maxDepth);
		}

		public void ForEachTextNode (HtmlFunc<HtmlElement, bool> func, int maxDepth = -1) {
			if (func is null) {
				throw new ArgumentNullException (nameof (func));
			}
			Root.ForEachTextNode (func, maxDepth);
		}

		public void ForEachElement (HtmlFunc<HtmlElement, bool> func, int maxDepth = -1) {
			if (func is null) {
				throw new ArgumentNullException (nameof (func));
			}
			Root.ForEachElement (func, maxDepth);
		}

		public HtmlElement QuerySelector (string path) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			return Root.QuerySelector (path);
		}

		public List<HtmlElement> QuerySelectorAll (string path) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			return Root.QuerySelectorAll (path);
		}

		#endregion

	}

}