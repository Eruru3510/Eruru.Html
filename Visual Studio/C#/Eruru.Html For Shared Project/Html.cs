using System;
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
			return Build (new StringReader (text));
		}

		public static Html Load (string path) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			return Build (new StreamReader (path));
		}

		public static Html Load (TextReader textReader) {
			if (textReader is null) {
				throw new ArgumentNullException (nameof (textReader));
			}
			return Build (textReader);
		}

		static Html Build (TextReader textReader) {
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

		#region IHtmlElement

		public string InnerHtml {

			get => Root.InnerHtml;

		}

		public HtmlElement this[int index] {

			get => Root[index];

		}

		public HtmlElement GetElementById (string id) {
			if (id is null) {
				throw new ArgumentNullException (nameof (id));
			}
			return Root.GetElementById (id);
		}

		public HtmlElement GetElementByTagName (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return Root.GetElementByTagName (name);
		}

		public HtmlElement GetElementByClassName (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return Root.GetElementByClassName (name);
		}

		public HtmlElement GetElementByName (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return Root.GetElementByName (name);
		}

		public HtmlElement[] GetElementsByTagName (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return Root.GetElementsByTagName (name);
		}

		public HtmlElement[] GetElementsByClassName (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return Root.GetElementsByClassName (name);
		}

		public HtmlElement[] GetElementsByName (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return Root.GetElementsByName (name);
		}

		public bool ForEachElement (HtmlFunc<HtmlElement, bool> func) {
			if (func is null) {
				throw new ArgumentNullException (nameof (func));
			}
			return Root.ForEachElement (func);
		}

		#endregion

	}

}