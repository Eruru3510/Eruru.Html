using System;
using System.Collections.Generic;

namespace Eruru.Html {

	public class HtmlElement : HtmlNode {

		public string LocalName { get; }
		public List<HtmlAttribute> Attributes { get; }
		public List<string> ClassList {

			get => GetAttributeNode (HtmlKeyword.Class)?.Values;

		}
		public string OuterHtml {

			get => Serialize (true, false);

		}

		public HtmlElement (string localName, List<HtmlAttribute> attributes) : base (HtmlNodeType.Element, localName.ToUpper (), null) {
			LocalName = localName ?? throw new ArgumentNullException (nameof (localName));
			Attributes = attributes ?? throw new ArgumentNullException (nameof (attributes));
		}
		public HtmlElement (string localName) : this (localName, new List<HtmlAttribute> ()) {
			if (localName is null) {
				throw new ArgumentNullException (nameof (localName));
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

		public HtmlElement QuerySelector (string path) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			using (HtmlSelector selector = new HtmlSelector (this)) {
				return selector.QuerySelector (path);
			}
		}

		public List<HtmlElement> QuerySelectorAll (string path) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			using (HtmlSelector selector = new HtmlSelector (this)) {
				return selector.QuerySelectorAll (path);
			}
		}

	}

}