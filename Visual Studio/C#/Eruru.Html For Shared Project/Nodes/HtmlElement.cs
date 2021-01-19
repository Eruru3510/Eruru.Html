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

		public HtmlElement GetElementById (string id, int maxDepth = -1) {
			if (id is null) {
				throw new ArgumentNullException (nameof (id));
			}
			return GetElement (element => {
				return HtmlApi.Equals (element.GetAttribute (HtmlKeyword.ID), id);
			}, maxDepth);
		}

		public HtmlElement GetElementByTagName (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return GetElement (element => {
				return HtmlApi.Equals (element.LocalName, name);
			}, maxDepth);
		}

		public HtmlElement GetElementByClassName (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			string[] classes = HtmlApi.Split (name);
			HtmlAttribute attribute;
			return GetElement (element => {
				attribute = element.GetAttributeNode (HtmlKeyword.Class);
				if (attribute is null || !HtmlApi.Contains (classes, attribute)) {
					return false;
				}
				return true;
			}, maxDepth);
		}

		public HtmlElement GetElementByName (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return GetElement (element => {
				return HtmlApi.Equals (element.GetAttribute (HtmlKeyword.Name), name);
			}, maxDepth);
		}

		public HtmlElement GetElementByAttribute (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			HtmlAttribute attribute;
			return GetElement (element => {
				attribute = element.GetAttributeNode (name);
				if (attribute is null) {
					return false;
				}
				return true;
			}, maxDepth);
		}
		public HtmlElement GetElementByAttribute (string name, string value, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			if (value is null) {
				throw new ArgumentNullException (nameof (value));
			}
			return GetElement (element => {
				return HtmlApi.Equals (element.GetAttribute (name), value);
			}, maxDepth);
		}

		public List<HtmlElement> GetElementsByTagName (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return GetElements (element => {
				return HtmlApi.Equals (element.LocalName, name);
			}, maxDepth);
		}

		public List<HtmlElement> GetElementsByClassName (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			string[] classes = HtmlApi.Split (name);
			HtmlAttribute attribute;
			return GetElements (element => {
				attribute = element.GetAttributeNode (HtmlKeyword.Class);
				if (attribute is null || !HtmlApi.Contains (classes, attribute)) {
					return false;
				}
				return true;
			}, maxDepth);
		}

		public List<HtmlElement> GetElementsByName (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return GetElements (element => {
				return HtmlApi.Equals (element.GetAttribute (HtmlKeyword.Name), name);
			}, maxDepth);
		}

		public List<HtmlElement> GetElementsByAttribute (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			HtmlAttribute attribute;
			return GetElements (element => {
				attribute = element.GetAttributeNode (name);
				if (attribute is null) {
					return false;
				}
				return true;
			}, maxDepth);
		}
		public List<HtmlElement> GetElementsByAttribute (string name, string value, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			if (value is null) {
				throw new ArgumentNullException (nameof (value));
			}
			return GetElements (element => {
				return HtmlApi.Equals (element.GetAttribute (name), value);
			}, maxDepth);
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

		HtmlElement GetElement (HtmlFunc<HtmlElement, bool> func, int maxDepth = -1) {
			if (func is null) {
				throw new ArgumentNullException (nameof (func));
			}
			HtmlElement targetElement = null;
			ForEachElement (element => {
				if (func (element)) {
					targetElement = element;
					return false;
				}
				return true;
			}, maxDepth);
			return targetElement;
		}

		List<HtmlElement> GetElements (HtmlFunc<HtmlElement, bool> func, int maxDepth = -1) {
			if (func is null) {
				throw new ArgumentNullException (nameof (func));
			}
			List<HtmlElement> elements = new List<HtmlElement> ();
			ForEachElement (element => {
				if (func (element)) {
					elements.Add (element);
				}
				return true;
			}, maxDepth);
			return elements;
		}

	}

}