using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace Eruru.Html {

	public class Html {

		public string InnerHtml {

			get => Element.InnerHtml;

		}

		HtmlElement Element { get; } = new HtmlElement ();

		public static Html Parse (string text) {
			Html html = new Html ();
			using (HtmlTagReader reader = new HtmlTagReader (new StringReader (text))) {
				while (reader.ReadElement (out HtmlElement element)) {
					if (element is null) {
						continue;
					}
					html.Element.Elements.Add (element);
				}
			}
			return html;
		}

		public static Html Load (string path) {
			Html html = new Html ();
			using (HtmlTagReader reader = new HtmlTagReader (new StreamReader (path))) {
				while (reader.ReadElement (out HtmlElement element)) {
					if (element is null) {
						continue;
					}
					html.Element.Elements.Add (element);
				}
			}
			return html;
		}

		public HtmlElement GetElementById (string id) {
			if (id is null) {
				throw new ArgumentNullException (nameof (id));
			}
			return Element.GetElementById (id);
		}

		public HtmlElement[] GetElementsByTagName (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return Element.GetElementsByTagName (name);
		}

		public HtmlElement[] GetElementsByClassName (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return Element.GetElementsByClassName (name);
		}

		public HtmlElement[] GetElementsByName (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return Element.GetElementsByName (name);
		}

	}

}