using System;
using System.IO;

namespace Eruru.Html {

	public class HtmlTagReader : HtmlTextReader {

		readonly string[] ContentTags = { "script", "style" };

		public HtmlTagReader (TextReader textReader) : base (textReader) {
			if (textReader is null) {
				throw new ArgumentNullException (nameof (textReader));
			}
		}

		public bool ReadElement (out HtmlElement element, string name = null, string parentName = null) {
			if (MoveNext ()) {
				switch (Current.Type) {
					case HtmlTagType.Define:
					case HtmlTagType.Single:
						element = new HtmlElement (Current.Name, HtmlApi.TagTypeToElementType (Current.Type), Current.Attributes);
						return true;
					case HtmlTagType.Start:
						element = new HtmlElement (Current.Name, HtmlApi.TagTypeToElementType (Current.Type), Current.Attributes);
						if (Array.Exists (ContentTags, tagName => HtmlApi.Equals (tagName, Current.Name))) {
							TextTokenizer.SkipWhiteSpace ();
							string content = TextTokenizer.ReadTo ($"</{element.Name}>").TrimEnd ();
							if (content.Length > 0) {
								element.Elements.Add (new HtmlElement (HtmlElementType.Text, content));
							}
							TextTokenizer.Read ();
							return true;
						}
						while (ReadElement (out HtmlElement childElement, element.Name, name)) {
							if (childElement is null) {
								continue;
							}
							element.Elements.Add (childElement);
						}
						return true;
					case HtmlTagType.End:
						if (name != null && Current.Name != name) {
							if (parentName != null && HtmlApi.Equals (Current.Name, parentName)) {
								//Console.WriteLine ($"标签不配对，{name}没有结束标签");
								element = null;
								return false;
							}
							//Console.WriteLine ($"标签不配对，{Current.Name}没有开始标签");
							element = null;
							return true;
						}
						element = null;
						return false;
					case HtmlTagType.Text:
					case HtmlTagType.Comment:
						element = new HtmlElement (HtmlApi.TagTypeToElementType (Current.Type), Current.Content);
						return true;
				}
			}
			element = null;
			return false;
		}

	}

}