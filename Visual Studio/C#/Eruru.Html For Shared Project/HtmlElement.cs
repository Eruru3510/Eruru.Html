using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Eruru.Html {

	public class HtmlElement : IHtmlElement, IEnumerable<HtmlElement> {

		public HtmlElementType Type { get; set; }
		public string Name { get; set; }
		public string Content { get; set; }
		public List<HtmlAttribute> Attributes { get; set; }
		public List<HtmlElement> Elements { get; set; }
		public List<string> ClassList {

			get => GetAttribute (HtmlKeyword.Class)?.Values;

		}
		public string OuterHtml {

			get => Serialize (true).ToString ();

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
			return GetAttribute (name)?.Value ?? string.Empty;
		}

		StringBuilder Serialize (bool writeSelf, StringBuilder stringBuilder = null, int indent = 0) {
			if (stringBuilder is null) {
				stringBuilder = new StringBuilder ();
			}
			if (writeSelf) {
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
							stringBuilder.Append (attribute.Value);
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
				if (writeSelf) {
					indent++;
				}
				for (int i = 0; i < Elements.Count; i++) {
					if (i > 0 || writeSelf) {
						NewLineIndent (stringBuilder, indent);
					}
					Elements[i].Serialize (true, stringBuilder, indent);
				}
				if (writeSelf) {
					indent--;
				}
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

		HtmlElement GetElement (HtmlFunc<HtmlElement, bool> func) {
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
			});
			return targetElement;
		}

		HtmlElement[] GetElements (HtmlFunc<HtmlElement, bool> func) {
			if (func is null) {
				throw new ArgumentNullException (nameof (func));
			}
			List<HtmlElement> elements = new List<HtmlElement> ();
			ForEachElement (element => {
				if (func (element)) {
					elements.Add (element);
				}
				return true;
			});
			return elements.ToArray ();
		}

		#region IHtmlElement

		public string InnerHtml {

			get => Serialize (false).ToString ();

		}

		public HtmlElement this[int index] {

			get => Elements[index];

		}

		public HtmlElement GetElementById (string id) {
			if (id is null) {
				throw new ArgumentNullException (nameof (id));
			}
			return GetElement (element => {
				return HtmlAPI.Equals (element.GetAttributeValue (HtmlKeyword.ID), id);
			});
		}

		public HtmlElement GetElementByTagName (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return GetElement (element => {
				return HtmlAPI.Equals (element.Name, name);
			});
		}

		public HtmlElement GetElementByClassName (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			string[] classes = HtmlAPI.Split (name);
			HtmlAttribute attribute;
			return GetElement (element => {
				attribute = element.GetAttribute (HtmlKeyword.Class);
				if (attribute is null || !HtmlAPI.Contains (classes, attribute)) {
					return false;
				}
				return true;
			});
		}

		public HtmlElement GetElementByName (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return GetElement (element => {
				return HtmlAPI.Equals (element.GetAttributeValue (HtmlKeyword.Name), name);
			});
		}

		public HtmlElement GetElementByAttribute (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			HtmlAttribute attribute;
			return GetElement (element => {
				attribute = element.GetAttribute (name);
				if (attribute is null) {
					return false;
				}
				return true;
			});
		}
		public HtmlElement GetElementByAttribute (string name, string value) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			if (value is null) {
				throw new ArgumentNullException (nameof (value));
			}
			return GetElement (element => {
				return HtmlAPI.Equals (element.GetAttributeValue (name), value);
			});
		}

		public HtmlElement[] GetElementsByTagName (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return GetElements (element => {
				return HtmlAPI.Equals (element.Name, name);
			});
		}

		public HtmlElement[] GetElementsByClassName (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			string[] classes = HtmlAPI.Split (name);
			HtmlAttribute attribute;
			return GetElements (element => {
				attribute = element.GetAttribute (HtmlKeyword.Class);
				if (attribute is null || !HtmlAPI.Contains (classes, attribute)) {
					return false;
				}
				return true;
			});
		}

		public HtmlElement[] GetElementsByName (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return GetElements (element => {
				return HtmlAPI.Equals (element.GetAttributeValue (HtmlKeyword.Name), name);
			});
		}

		public HtmlElement[] GetElementsByAttribute (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			HtmlAttribute attribute;
			return GetElements (element => {
				attribute = element.GetAttribute (name);
				if (attribute is null) {
					return false;
				}
				return true;
			});
		}
		public HtmlElement[] GetElementsByAttribute (string name, string value) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			if (value is null) {
				throw new ArgumentNullException (nameof (value));
			}
			return GetElements (element => {
				return HtmlAPI.Equals (element.GetAttributeValue (name), value);
			});
		}

		public bool ForEachElement (HtmlFunc<HtmlElement, bool> func) {
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

		#endregion

		#region IEnumerable<HtmlElement>

		public IEnumerator<HtmlElement> GetEnumerator () {
			return Elements.GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator () {
			return Elements.GetEnumerator ();
		}

		#endregion

	}

}