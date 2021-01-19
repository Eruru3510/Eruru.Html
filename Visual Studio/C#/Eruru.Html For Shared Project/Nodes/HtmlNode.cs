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
		public string InnerText {

			get {
				StringBuilder stringBuilder = new StringBuilder ();
				ForEachText (text => {
					stringBuilder.Append (text.NodeValue);
					return true;
				});
				return stringBuilder.ToString ();
			}

		}
		public List<HtmlNode> ChildNodes { get; } = new List<HtmlNode> ();
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

		public HtmlNode (HtmlNodeType nodeType, string nodeName, string nodeValue) {
			NodeType = nodeType;
			NodeName = nodeName ?? throw new ArgumentNullException (nameof (nodeName));
			NodeValue = nodeValue;
		}

		public HtmlElement GetElementById (string id, int maxDepath = -1) {
			if (id is null) {
				throw new ArgumentNullException (nameof (id));
			}
			return GetElement (element => {
				return HtmlApi.Equals (element.GetAttribute (HtmlKeyword.ID), id);
			}, maxDepath);
		}

		public HtmlElement GetElementByTagName (string name, int maxDepath = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return GetElement (element => {
				return HtmlApi.Equals (element.LocalName, name);
			}, maxDepath);
		}

		public HtmlElement GetElementByClassName (string name, int maxDepath = -1) {
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
			}, maxDepath);
		}

		public HtmlElement GetElementByName (string name, int maxDepath = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return GetElement (element => {
				return HtmlApi.Equals (element.GetAttribute (HtmlKeyword.Name), name);
			}, maxDepath);
		}

		public HtmlElement GetElementByAttribute (string name, int maxDepath = -1) {
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
			}, maxDepath);
		}
		public HtmlElement GetElementByAttribute (string name, string value, int maxDepath = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			if (value is null) {
				throw new ArgumentNullException (nameof (value));
			}
			return GetElement (element => {
				return HtmlApi.Equals (element.GetAttribute (name), value);
			}, maxDepath);
		}

		public List<HtmlElement> GetElementsByTagName (string name, int maxDepath = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return GetElements (element => {
				return HtmlApi.Equals (element.LocalName, name);
			}, maxDepath);
		}

		public List<HtmlElement> GetElementsByClassName (string name, int maxDepath = -1) {
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
			}, maxDepath);
		}

		public List<HtmlElement> GetElementsByName (string name, int maxDepath = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return GetElements (element => {
				return HtmlApi.Equals (element.GetAttribute (HtmlKeyword.Name), name);
			}, maxDepath);
		}

		public List<HtmlElement> GetElementsByAttribute (string name, int maxDepath = -1) {
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
			}, maxDepath);
		}
		public List<HtmlElement> GetElementsByAttribute (string name, string value, int maxDepath = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			if (value is null) {
				throw new ArgumentNullException (nameof (value));
			}
			return GetElements (element => {
				return HtmlApi.Equals (element.GetAttribute (name), value);
			}, maxDepath);
		}

		public void ForEachNode (HtmlFunc<HtmlNode, bool> func, int maxDepath = -1) {
			if (func is null) {
				throw new ArgumentNullException (nameof (func));
			}
			ForEach (node => {
				if (!func (node)) {
					return false;
				}
				return true;
			}, maxDepath);
		}

		public void ForEachText (HtmlFunc<HtmlText, bool> func, int maxDepath = -1) {
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
			}, maxDepath);
		}

		public void ForEachElement (HtmlFunc<HtmlElement, bool> func, int maxDepath = -1) {
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
			}, maxDepath);
		}

		protected string Serialize (bool isOuter, bool compress) {
			StringBuilder stringBuilder = new StringBuilder ();
			Serialize (stringBuilder, isOuter, compress);
			return stringBuilder.ToString ();
		}

		void Serialize (StringBuilder stringBuilder, bool writeSelf, bool compress, int indentLevel = 0) {
			if (stringBuilder is null) {
				throw new ArgumentNullException (nameof (stringBuilder));
			}
			bool needWriteEndTag = false;
			if (writeSelf) {
				switch (this) {
					case HtmlText _:
						stringBuilder.Append (NodeValue);
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
				ChildNodes[i].Serialize (stringBuilder, true, compress, indentLevel);
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
					stringBuilder.Append ($"=\"{attribute.Value}\"");
				}
			}
		}

		HtmlElement GetElement (HtmlFunc<HtmlElement, bool> func, int maxDepath = -1) {
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
			}, maxDepath);
			return targetElement;
		}

		List<HtmlElement> GetElements (HtmlFunc<HtmlElement, bool> func, int maxDepath = -1) {
			if (func is null) {
				throw new ArgumentNullException (nameof (func));
			}
			List<HtmlElement> elements = new List<HtmlElement> ();
			ForEachElement (element => {
				if (func (element)) {
					elements.Add (element);
				}
				return true;
			}, maxDepath);
			return elements;
		}

		bool ForEach (HtmlFunc<HtmlNode, bool> func, int maxDepath = -1) {
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
				if (maxDepath < 0 || maxDepath > 0) {
					if (!node.ForEach (func, maxDepath - 1)) {
						return false;
					}
				}
			}
			return true;
		}

	}

}