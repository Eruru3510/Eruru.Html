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
				StringBuilder stringBuilder = new StringBuilder ();
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

		public HtmlNode (HtmlNodeType nodeType, string nodeName, string nodeValue, HtmlElement parentElement) {
			NodeType = nodeType;
			NodeName = nodeName ?? throw new ArgumentNullException (nameof (nodeName));
			NodeValue = nodeValue;
			ParentElement = parentElement;
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

	}

}