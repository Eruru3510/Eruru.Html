using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Eruru.Html {

	public class HtmlElement {

		public HtmlElementType Type { get; set; }
		public string Name { get; set; }
		public string Content { get; set; }
		public List<HtmlAttribute> Attributes { get; set; }
		public List<HtmlElement> Elements { get; set; }
		public string InnerHtml {

			get => Serialize (false).ToString ();

		}
		public string OuterHtml {

			get => Serialize (true).ToString ();

		}
		public HtmlElement this[int index] {

			get => Elements[index];

		}

		public HtmlElement () {
			Type = HtmlElementType.Root;
			Elements = new List<HtmlElement> ();
		}
		public HtmlElement (string name, HtmlElementType type, List<HtmlAttribute> attributes) {
			Name = name ?? throw new ArgumentNullException (nameof (name));
			Type = type;
			Attributes = attributes ?? throw new ArgumentNullException (nameof (attributes));
			if (type == HtmlElementType.Double) {
				Elements = new List<HtmlElement> ();
			}
		}
		public HtmlElement (HtmlElementType type, string content) {
			if (content is null) {
				throw new ArgumentNullException (nameof (content));
			}
			Type = type;
			Content = content;
		}

		public HtmlAttribute GetAttribute (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			if (Attributes is null) {
				return null;
			}
			foreach (HtmlAttribute attribute in Attributes) {
				if (HtmlAPI.Equals (attribute.Name, name)) {
					return attribute;
				}
			}
			return null;
		}

		public string GetAttributeValue (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			List<string> values = GetAttribute (name)?.Values;
			if (values is null || values.Count == 0) {
				return string.Empty;
			}
			if (values.Count == 1) {
				return values[0];
			}
			return HtmlAPI.Equals (name, HtmlKeyword.Class) ? string.Join (" ", values.ToArray ()) : values[0];
		}

		public HtmlElement GetElementById (string id) {
			if (id is null) {
				throw new ArgumentNullException (nameof (id));
			}
			HtmlElement element = null;
			ForEachElement (current => {
				if (HtmlAPI.Equals (current.GetAttributeValue (nameof (id)), id)) {
					element = current;
					return false;
				}
				return true;
			});
			return element;
		}

		public HtmlElement[] GetElementsByTagName (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			List<HtmlElement> elements = new List<HtmlElement> ();
			ForEachElement (element => {
				if (HtmlAPI.Equals (element.Name, name)) {
					elements.Add (element);
				}
				return true;
			});
			return elements.ToArray ();
		}

		public HtmlElement[] GetElementsByClassName (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			List<HtmlElement> elements = new List<HtmlElement> ();
			string[] classes = HtmlAPI.Split (name);
			ForEachElement (element => {
				HtmlAttribute attribute = element.GetAttribute (HtmlKeyword.Class);
				if (attribute is null) {
					return true;
				}
				for (int i = 0; i < classes.Length; i++) {
					if (!attribute.Values.Contains (classes[i])) {
						return true;
					}
				}
				elements.Add (element);
				return true;
			});
			return elements.ToArray ();
		}

		public HtmlElement[] GetElementsByName (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			List<HtmlElement> elements = new List<HtmlElement> ();
			ForEachElement (current => {
				if (HtmlAPI.Equals (current.GetAttributeValue (nameof (name)), name)) {
					elements.Add (current);
				}
				return true;
			});
			return elements.ToArray ();
		}

		bool ForEachElement (HtmlFunc<HtmlElement, bool> func) {
			if (func is null) {
				throw new ArgumentNullException (nameof (func));
			}
			foreach (HtmlElement element in Elements) {
				if (HtmlAPI.HasFlag (element.Type, HtmlElementType.Tag)) {
					if (!func (element)) {
						return false;
					}
				}
				if (element.Elements != null) {
					if (!element.ForEachElement (func)) {
						return false;
					}
				}
			}
			return true;
		}

		StringBuilder Serialize (bool writeSelf, StringBuilder stringBuilder = null, int indent = 0) {
			if (stringBuilder is null) {
				stringBuilder = new StringBuilder ();
			}
			if (writeSelf && Type != HtmlElementType.Root) {
				switch (Type) {
					case HtmlElementType.Text:
						stringBuilder.Append (Content);
						return stringBuilder;
					case HtmlElementType.Comment:
						stringBuilder.Append (HtmlKeyword.LeftAngleBracket);
						stringBuilder.Append (HtmlKeyword.ExclamationMark);
						stringBuilder.Append (HtmlKeyword.MinusSign);
						stringBuilder.Append (HtmlKeyword.MinusSign);
						stringBuilder.Append (Content);
						stringBuilder.Append (HtmlKeyword.MinusSign);
						stringBuilder.Append (HtmlKeyword.MinusSign);
						stringBuilder.Append (HtmlKeyword.RightAngleBracket);
						return stringBuilder;
				}
				stringBuilder.Append (HtmlKeyword.LeftAngleBracket);
				if (Type == HtmlElementType.Define) {
					stringBuilder.Append (HtmlKeyword.ExclamationMark);
				}
				stringBuilder.Append (Name);
				if (Attributes?.Count > 0) {
					foreach (HtmlAttribute attribute in Attributes) {
						stringBuilder.Append (HtmlKeyword.Space);
						stringBuilder.Append (attribute.Name);
						if (attribute.Values != null) {
							stringBuilder.Append (HtmlKeyword.EqualSign);
							stringBuilder.Append (HtmlKeyword.DoubleQuot);
							stringBuilder.Append (string.Join (" ", attribute.Values.ToArray ()));
							stringBuilder.Append (HtmlKeyword.DoubleQuot);
						}
					}
				}
				if (Type == HtmlElementType.Single) {
					stringBuilder.Append (HtmlKeyword.Slash);
				}
				stringBuilder.Append (HtmlKeyword.RightAngleBracket);
			}
			if (Elements != null) {
				if (Type != HtmlElementType.Root) {
					indent++;
				}
				for (int i = 0; i < Elements.Count; i++) {
					if (i > 0 || Type != HtmlElementType.Root) {
						NewLineIndent (stringBuilder, indent);
					}
					Elements[i].Serialize (true, stringBuilder, indent);
				}
				indent--;
			}
			if (writeSelf && Type == HtmlElementType.Double) {
				if (Elements.Count > 0) {
					NewLineIndent (stringBuilder, indent);
				}
				stringBuilder.Append (HtmlKeyword.LeftAngleBracket);
				stringBuilder.Append (HtmlKeyword.Slash);
				stringBuilder.Append (Name);
				stringBuilder.Append (HtmlKeyword.RightAngleBracket);
			}
			return stringBuilder;
		}

		void NewLineIndent (StringBuilder stringBuilder, int indent) {
			if (stringBuilder is null) {
				throw new ArgumentNullException (nameof (stringBuilder));
			}
			stringBuilder.AppendLine ();
			for (int i = 0; i < indent; i++) {
				stringBuilder.Append ('\t');
			}
		}

	}

}