using System;
using System.Collections.Generic;
using System.IO;

namespace Eruru.Html {

	public class HtmlTagReader : HtmlTextReader {

		readonly string[] ContentTags = { "script", "style" };
		readonly List<string> Tags = new List<string> ();

		public HtmlTagReader (TextReader textReader) : base (textReader) {
			if (textReader is null) {
				throw new ArgumentNullException (nameof (textReader));
			}
		}

		public bool ReadElement (out HtmlElement element) {
			return ReadElement (out element, null);
		}

		bool ReadElement (out HtmlElement element, string endTagName = null) {
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
							return true;
						}
						Tags.Add (Current.Name);
						while (ReadElement (out HtmlElement childElement, element.Name)) {
							if (childElement is null) {
								continue;
							}
							element.Elements.Add (childElement);
						}
						return true;
					case HtmlTagType.End:
						if (endTagName != null && !HtmlApi.Equals (Current.Name, endTagName)) {
							if (Tags.Contains (Current.Name)) {
								Buffer.Push (Current);
							} else {
								element = null;
								return true;
							}
						}
						Tags.RemoveAt (Tags.Count - 1);
						break;
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