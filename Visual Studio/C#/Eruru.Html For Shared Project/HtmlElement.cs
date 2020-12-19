using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
				if (HtmlApi.Equals (attribute.Name, name)) {
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

		public string Serialize (bool compress = false, bool writeSelf = true) {
			return SerializeElement (writeSelf, compress).ToString ();
		}

		StringBuilder SerializeElement (bool writeSelf, bool compress = false, StringBuilder stringBuilder = null, int indent = default) {
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
						NewLineIndent ();
					}
					Elements[i].SerializeElement (true, compress, stringBuilder, indent);
				}
				if (writeSelf) {
					indent--;
				}
			}
			if (writeSelf && Type == HtmlElementType.Double) {
				if (Elements.Count > 0) {
					NewLineIndent ();
				}
				stringBuilder.Append (HtmlKeyword.LeftAngleBracket);
				stringBuilder.Append (HtmlKeyword.Slash);
				stringBuilder.Append (Name);
				stringBuilder.Append (HtmlKeyword.RightAngleBracket);
			}
			return stringBuilder;
			void NewLineIndent () {
				if (compress) {
					return;
				}
				stringBuilder.AppendLine ();
				for (int i = 0; i < indent; i++) {
					stringBuilder.Append ('\t');
				}
			}
		}

		StringBuilder SerializeTextNode (StringBuilder stringBuilder = null) {
			if (stringBuilder is null) {
				stringBuilder = new StringBuilder ();
			}
			ForEachTextNode (element => {
				stringBuilder.Append (element.Content);
				return true;
			});
			return stringBuilder;
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

		bool ForEach (HtmlFunc<HtmlElement, bool> func, int maxDepath = -1) {
			if (func is null) {
				throw new ArgumentNullException (nameof (func));
			}
			if (Elements is null) {
				return true;
			}
			foreach (HtmlElement element in Elements) {
				if (!func (element)) {
					return false;
				}
				if (maxDepath < 0 || maxDepath > 0) {
					if (!element.ForEach (func, maxDepath - 1)) {
						return false;
					}
				}
			}
			return true;
		}

		#region IHtmlElement

		public string InnerHtml {

			get => Serialize (false).ToString ();

		}
		public string InnerText {

			get => SerializeTextNode ().ToString ();

		}
		public HtmlElement this[int index] {

			get => Elements[index];

		}

		public HtmlElement GetElementById (string id, int maxDepath = -1) {
			if (id is null) {
				throw new ArgumentNullException (nameof (id));
			}
			return GetElement (element => {
				return HtmlApi.Equals (element.GetAttributeValue (HtmlKeyword.ID), id);
			}, maxDepath);
		}

		public HtmlElement GetElementByTagName (string name, int maxDepath = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return GetElement (element => {
				return HtmlApi.Equals (element.Name, name);
			}, maxDepath);
		}

		public HtmlElement GetElementByClassName (string name, int maxDepath = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			string[] classes = HtmlApi.Split (name);
			HtmlAttribute attribute;
			return GetElement (element => {
				attribute = element.GetAttribute (HtmlKeyword.Class);
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
				return HtmlApi.Equals (element.GetAttributeValue (HtmlKeyword.Name), name);
			}, maxDepath);
		}

		public HtmlElement GetElementByAttribute (string name, int maxDepath = -1) {
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
				return HtmlApi.Equals (element.GetAttributeValue (name), value);
			}, maxDepath);
		}

		public List<HtmlElement> GetElementsByTagName (string name, int maxDepath = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return GetElements (element => {
				return HtmlApi.Equals (element.Name, name);
			}, maxDepath);
		}

		public List<HtmlElement> GetElementsByClassName (string name, int maxDepath = -1) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			string[] classes = HtmlApi.Split (name);
			HtmlAttribute attribute;
			return GetElements (element => {
				attribute = element.GetAttribute (HtmlKeyword.Class);
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
				return HtmlApi.Equals (element.GetAttributeValue (HtmlKeyword.Name), name);
			}, maxDepath);
		}

		public List<HtmlElement> GetElementsByAttribute (string name, int maxDepath = -1) {
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
				return HtmlApi.Equals (element.GetAttributeValue (name), value);
			}, maxDepath);
		}

		public void ForEachNode (HtmlFunc<HtmlElement, bool> func, int maxDepath = -1) {
			if (func is null) {
				throw new ArgumentNullException (nameof (func));
			}
			ForEach (element => {
				if (!func (element)) {
					return false;
				}
				return true;
			}, maxDepath);
		}

		public void ForEachTextNode (HtmlFunc<HtmlElement, bool> func, int maxDepath = -1) {
			if (func is null) {
				throw new ArgumentNullException (nameof (func));
			}
			ForEach (element => {
				if (element.Type != HtmlElementType.Text) {
					return true;
				}
				if (!func (element)) {
					return false;
				}
				return true;
			}, maxDepath);
		}

		public void ForEachElement (HtmlFunc<HtmlElement, bool> func, int maxDepath = -1) {
			if (func is null) {
				throw new ArgumentNullException (nameof (func));
			}
			ForEach (element => {
				if (!HtmlApi.HasFlag (element.Type, HtmlElementType.Tag)) {
					return true;
				}
				if (!func (element)) {
					return false;
				}
				return true;
			}, maxDepath);
		}

		public HtmlElement QuerySelector (string path) {
			if (HtmlApi.IsNullOrWhiteSpace (path)) {
				throw new ArgumentException ($"“{nameof (path)}”不能为 Null 或空白", nameof (path));
			}
			List<HtmlElement> elements = QuerySelectorAll (path);
			return elements.Count == 0 ? null : elements[0];
		}

		public List<HtmlElement> QuerySelectorAll (string path) {
			if (HtmlApi.IsNullOrWhiteSpace (path)) {
				throw new ArgumentException ($"“{nameof (path)}”不能为 Null 或空白", nameof (path));
			}
			HtmlElement root = this;
			List<HtmlElement> elements = new List<HtmlElement> ();
			List<HtmlElement> targetElements = new List<HtmlElement> ();
			List<HtmlElement> tempElements = new List<HtmlElement> ();
			targetElements.Add (root);
			bool isChild = true;
			int depth = -1;
			using (HtmlSelectorReader reader = new HtmlSelectorReader (new StringReader (path))) {
				while (reader.MoveNext ()) {
					switch (reader.Current.Type) {
						case HtmlTokenType.Dot: {
							reader.MoveNext ();
							string name = reader.Current;
							if (isChild) {
								QuerySelectorAll (element => element.GetElementsByClassName (name, depth));
								break;
							}
							Filter (element => element.ClassList.Contains (name));
							break;
						}
						case HtmlTokenType.NumberSign: {
							reader.MoveNext ();
							string name = reader.Current;
							if (isChild) {
								QuerySelectorAll (element => element.GetElementsByClassName (name, depth));
								break;
							}
							Filter (element => HtmlApi.Equals (element.GetAttributeValue (HtmlKeyword.ID), name));
							break;
						}
						case HtmlTokenType.Comma: {
							elements.AddRange (targetElements);
							targetElements.Clear ();
							targetElements.Add (root);
							break;
						}
						case HtmlTokenType.String: {
							string name = reader.Current;
							QuerySelectorAll (element => element.GetElementsByTagName (name, depth));
							break;
						}
						case HtmlTokenType.LeftBracket: {
							reader.MoveNext ();
							string name = reader.Current;
							reader.MoveNext ();
							switch (reader.Current.Type) {
								case HtmlTokenType.EqualSign:
									reader.MoveNext ();
									string value = reader.Current;
									Filter (element => HtmlApi.Equals (element.GetAttributeValue (name), value));
									reader.MoveNext ();
									break;
								case HtmlTokenType.RightBracket:
									Filter (element => element.GetAttribute (name) != null);
									break;
							}
							break;
						}
					}
					depth = -1;
					switch (reader.Current.Type) {
						case HtmlTokenType.RightAngleBracket:
							depth = 0;
							break;
					}
					isChild = false;
					if (char.IsWhiteSpace ((char)reader.Peek ())) {
						isChild = true;
					}
				}
				elements.AddRange (targetElements);
				return elements;
			}
			void QuerySelectorAll (HtmlFunc<HtmlElement, List<HtmlElement>> func) {
				if (func is null) {
					throw new ArgumentNullException (nameof (func));
				}
				tempElements.Clear ();
				foreach (HtmlElement element in targetElements) {
					tempElements.AddRange (func (element));
				}
				targetElements.Clear ();
				targetElements.AddRange (tempElements);
			}
			void Filter (HtmlFunc<HtmlElement, bool> func) {
				if (func is null) {
					throw new ArgumentNullException (nameof (func));
				}
				for (int i = 0; i < targetElements.Count; i++) {
					if (!func (targetElements[i])) {
						targetElements.RemoveAt (i--);
					}
				}
			}
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