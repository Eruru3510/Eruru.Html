using System;
using System.Collections.Generic;

namespace Eruru.Html {

	public class HtmlElement : HtmlNode, IHtmlDocument {

		public string LocalName { get; }
		public List<HtmlAttribute> Attributes { get; }
		public List<string> ClassList {

			get => GetAttributeNode (HtmlKeyword.Class)?.Values;

		}
		public string OuterHtml {

			get => Serialize (true, false);

		}

		public HtmlElement (string localName, List<HtmlAttribute> attributes, HtmlElement parentElement) :
		base (HtmlNodeType.Element, localName.ToUpper (), null, parentElement) {
			LocalName = localName ?? throw new ArgumentNullException (nameof (localName));
			Attributes = attributes ?? throw new ArgumentNullException (nameof (attributes));
		}
		internal HtmlElement (List<HtmlNode> childNodes) : base (childNodes) {
			if (childNodes is null) {
				throw new ArgumentNullException (nameof (childNodes));
			}
		}

		public HtmlAttribute GetAttributeNode (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			if (Attributes is null) {
				return null;
			}
			foreach (HtmlAttribute attribute in Attributes) {
				if (HtmlApi.Equals (attribute.Name, name)) {
					return attribute;
				}
			}
			return null;
		}

		public string GetAttribute (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return GetAttributeNode (name)?.Value;
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