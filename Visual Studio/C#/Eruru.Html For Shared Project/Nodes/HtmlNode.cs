using System;
using System.Collections.Generic;
using System.Text;

namespace Eruru.Html {

	public abstract class HtmlNode {

		public HtmlNodeType NodeType { get; }
		public string NodeName { get; }
		public string NodeValue { get; }
		public string InnerHtml {

			get => Serialize (false, false);

		}
		public string TextContent {

			get {
				StringBuilder stringBuilder = new StringBuilder (NodeValue);
				ForEachText (text => {
					stringBuilder.Append (text.NodeValue);
					return true;
				});
				return stringBuilder.ToString ();
			}

		}
		public HtmlElement ParentElement { get; }
		public HtmlNode PreviousSibling { get; internal set; }
		public HtmlNode NextSibling { get; internal set; }
		public HtmlElement PreviousElementSibling {

			get {
				switch (PreviousSibling) {
					case null:
						return null;
					case HtmlElement element:
						return element;
					default:
						return NextSibling.NextElementSibling;
				}
			}

		}
		public HtmlElement NextElementSibling {

			get {
				switch (NextSibling) {
					case null:
						return null;
					case HtmlElement element:
						return element;
					default:
						return NextSibling.NextElementSibling;
				}
			}

		}
		public List<HtmlNode> ChildNodes { get; }
		public List<HtmlElement> Children {

			get {
				List<HtmlElement> elements = new List<HtmlElement> ();
				foreach (HtmlNode node in ChildNodes) {
					if (node is HtmlElement element) {
						elements.Add (element);
					}
				}
				return elements;
			}

		}
		public HtmlNode this[int index] {

			get => ChildNodes[index];

		}

		public HtmlNode (HtmlNodeType nodeType, string nodeName, string nodeValue, HtmlElement parentElement) {
			NodeType = nodeType;
			NodeName = nodeName ?? throw new ArgumentNullException (nameof (nodeName));
			NodeValue = nodeValue;
			ChildNodes = new List<HtmlNode> ();
			ParentElement = parentElement;
		}
		internal HtmlNode (List<HtmlNode> childNodes) {
			ChildNodes = childNodes ?? throw new ArgumentNullException (nameof (childNodes));
		}

		protected string Serialize (bool isOuter, bool compress) {
			StringBuilder stringBuilder = new StringBuilder ();
			Serialize (stringBuilder, isOuter, compress);
			return stringBuilder.ToString ();
		}

		protected bool ForEach (HtmlFunc<HtmlNode, bool> func, int maxDepth = -1) {
			if (func is null) {
				throw new ArgumentNullException (nameof (func));
			}
			if (ChildNodes is null) {
				return true;
			}
			foreach (HtmlNode node in ChildNodes) {
				if (!func (node)) {
					return false;
				}
				if (maxDepth < 0 || maxDepth > 0) {
					if (!node.ForEach (func, maxDepth - 1)) {
						return false;
					}
				}
			}
			return true;
		}

		protected void ForEachNode (HtmlFunc<HtmlNode, bool> func, int maxDepth = -1) {
			if (func is null) {
				throw new ArgumentNullException (nameof (func));
			}
			ForEach (node => {
				if (!func (node)) {
					return false;
				}
				return true;
			}, maxDepth);
		}

		protected void ForEachText (HtmlFunc<HtmlText, bool> func, int maxDepth = -1) {
			if (func is null) {
				throw new ArgumentNullException (nameof (func));
			}
			ForEach (node => {
				if (node is HtmlText text) {
					if (!func (text)) {
						return false;
					}
				}
				return true;
			}, maxDepth);
		}

		protected void ForEachElement (HtmlFunc<HtmlElement, bool> func, int maxDepth = -1) {
			if (func is null) {
				throw new ArgumentNullException (nameof (func));
			}
			ForEach (node => {
				if (node is HtmlElement element) {
					if (!func (element)) {
						return false;
					}
				}
				return true;
			}, maxDepth);
		}

		protected HtmlElement GetElementById (string id, int maxDepth = -1) {
			if (id is null) {
				throw new ArgumentNullException (nameof (id));
			}
			return GetElement (element => {
				return HtmlApi.Equals (element.GetAttribute (HtmlKeyword.ID), id);
			}, maxDepth);
		}

		protected HtmlElement GetElementByTagName (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return GetElement (element => {
				return HtmlApi.Equals (element.LocalName, name);
			}, maxDepth);
		}

		protected HtmlElement GetElementByClassName (string name, int maxDepth = -1) {
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

		protected HtmlElement GetElementByName (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return GetElement (element => {
				return HtmlApi.Equals (element.GetAttribute (HtmlKeyword.Name), name);
			}, maxDepth);
		}

		protected HtmlElement GetElementByAttribute (string name, int maxDepth = -1) {
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
		protected HtmlElement GetElementByAttribute (string name, string value, int maxDepth = -1) {
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

		protected List<HtmlElement> GetElementsByTagName (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return GetElements (element => {
				return HtmlApi.Equals (element.LocalName, name);
			}, maxDepth);
		}

		protected List<HtmlElement> GetElementsByClassName (string name, int maxDepth = -1) {
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

		protected List<HtmlElement> GetElementsByName (string name, int maxDepth = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return GetElements (element => {
				return HtmlApi.Equals (element.GetAttribute (HtmlKeyword.Name), name);
			}, maxDepth);
		}

		protected List<HtmlElement> GetElementsByAttribute (string name, int maxDepth = -1) {
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
		protected List<HtmlElement> GetElementsByAttribute (string name, string value, int maxDepth = -1) {
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

		protected HtmlElement QuerySelector (string path) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			using (HtmlSelector selector = new HtmlSelector (this)) {
				return selector.QuerySelector (path);
			}
		}

		protected List<HtmlElement> QuerySelectorAll (string path) {
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

		void Serialize (StringBuilder stringBuilder, bool writeSelf, bool compress, int indentLevel = 0, bool unescape = false) {
			if (stringBuilder is null) {
				throw new ArgumentNullException (nameof (stringBuilder));
			}
			bool needWriteEndTag = false;
			bool needUnescape = false;
			if (writeSelf) {
				switch (this) {
					case HtmlText _:
						stringBuilder.Append (unescape ? HtmlApi.CancelUnescape (NodeValue) : NodeValue);
						break;
					case HtmlComment _:
						stringBuilder.Append ($"<!--{NodeValue}-->");
						break;
					case HtmlDocumentType documentType:
						stringBuilder.Append ($"<!doctype");
						WriteAttribute (stringBuilder, documentType.Attributes);
						stringBuilder.Append ('>');
						break;
					case HtmlElement element:
						stringBuilder.Append ($"<{element.LocalName}");
						WriteAttribute (stringBuilder, element.Attributes);
						needWriteEndTag = !HtmlApi.IsSingleTag (element.LocalName);
						stringBuilder.Append (needWriteEndTag ? ">" : "/>");
						if (needWriteEndTag && !HtmlApi.IsContentTag (element.LocalName)) {
							needUnescape = true;
						}
						break;
					default:
						throw new NotImplementedException (ToString ());
				}
				indentLevel++;
			}
			for (int i = 0; i < ChildNodes.Count; i++) {
				if (writeSelf || i > 0) {
					NewLineIndent (stringBuilder, compress, indentLevel);
				}
				ChildNodes[i].Serialize (stringBuilder, true, compress, indentLevel, needUnescape);
			}
			if (writeSelf) {
				indentLevel--;
				if (needWriteEndTag) {
					if (ChildNodes.Count > 0) {
						NewLineIndent (stringBuilder, compress, indentLevel);
					}
					stringBuilder.Append ($"</{((HtmlElement)this).LocalName}>");
				}
			}
		}

		void NewLineIndent (StringBuilder stringBuilder, bool compress, int indentLevel) {
			if (stringBuilder is null) {
				throw new ArgumentNullException (nameof (stringBuilder));
			}
			if (compress) {
				return;
			}
			stringBuilder.AppendLine ();
			for (int i = 0; i < indentLevel; i++) {
				stringBuilder.Append ('\t');
			}
		}

		void WriteAttribute (StringBuilder stringBuilder, List<HtmlAttribute> attributes) {
			if (stringBuilder is null) {
				throw new ArgumentNullException (nameof (stringBuilder));
			}
			if (attributes is null) {
				throw new ArgumentNullException (nameof (attributes));
			}
			foreach (HtmlAttribute attribute in attributes) {
				stringBuilder.Append ($" {attribute.Name}");
				if (attribute.Values != null) {
					stringBuilder.Append ($"=\"{HtmlApi.CancelUnescapeAttributeValue (attribute.Value)}\"");
				}
			}
		}

	}

}