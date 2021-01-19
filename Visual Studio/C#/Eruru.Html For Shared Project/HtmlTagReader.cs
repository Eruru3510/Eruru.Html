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

		public bool ReadNode (out HtmlNode node) {
			return ReadNode (out node, null);
		}

		bool ReadNode (out HtmlNode node, string endElementLocalName = null) {
			if (MoveNext ()) {
				switch (Current.Type) {
					case HtmlTagType.Define:
						node = new HtmlDocumentType (Current.Attributes);
						return true;
					case HtmlTagType.Single:
						node = new HtmlElement (Current.Name, Current.Attributes);
						return true;
					case HtmlTagType.Start: {
						HtmlElement element = new HtmlElement (Current.Name, Current.Attributes);
						node = element;
						if (Array.Exists (ContentTags, tagName => HtmlApi.Equals (tagName, element.LocalName))) {
							TextTokenizer.SkipWhiteSpace ();
							string text = TextTokenizer.ReadTo ($"</{element.LocalName}>").TrimEnd ();
							if (text.Length > 0) {
								node.ChildNodes.Add (new HtmlText (text));
							}
							return true;
						}
						Tags.Add (element.LocalName);
						while (ReadNode (out HtmlNode childNode, element.LocalName)) {
							if (childNode is null) {
								continue;
							}
							node.ChildNodes.Add (childNode);
						}
						return true;
					}
					case HtmlTagType.End:
						if (endElementLocalName != null && !HtmlApi.Equals (Current.Name, endElementLocalName)) {
							if (Tags.Contains (Current.Name)) {
								Buffer.Push (Current);
							} else {
								node = null;
								return true;
							}
						}
						if (Tags.Count > 0) {
							Tags.RemoveAt (Tags.Count - 1);
						}
						break;
					case HtmlTagType.Text:
						node = new HtmlText (Current.Content);
						return true;
					case HtmlTagType.Comment:
						node = new HtmlComment (Current.Content);
						return true;
					default:
						throw new NotImplementedException (Current.Type.ToString ());
				}
			}
			node = null;
			return false;
		}

	}

}