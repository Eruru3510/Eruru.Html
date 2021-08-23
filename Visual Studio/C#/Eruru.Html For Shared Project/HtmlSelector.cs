using Eruru.LexicalAnalyzer;
using System;
using System.Collections.Generic;
using System.IO;

namespace Eruru.Html {

	public class HtmlSelector : LexicalAnalyzer<HtmlTokenType> {

		readonly HtmlNode RootNode;

		HtmlElement Root;

		public HtmlSelector (HtmlNode root) : base (
			HtmlTokenType.End,
			HtmlTokenType.String,
			HtmlTokenType.Integer,
			HtmlTokenType.Decimal,
			HtmlTokenType.String
		) {
			RootNode = root ?? throw new ArgumentNullException (nameof (root));
			Characters.Add (HtmlKeyword.EqualSign, HtmlTokenType.EqualSign);
			Characters.Add (HtmlKeyword.Comma, HtmlTokenType.Comma);
			Characters.Add (HtmlKeyword.NumberSign, HtmlTokenType.NumberSign);
			Characters.Add (HtmlKeyword.Dot, HtmlTokenType.Dot);
			Characters.Add (HtmlKeyword.LeftBracket, HtmlTokenType.LeftBracket);
			Characters.Add (HtmlKeyword.RightBracket, HtmlTokenType.RightBracket);
			Characters.Add (HtmlKeyword.RightAngleBracket, HtmlTokenType.RightAngleBracket);
		}

		public HtmlElement QuerySelector (string path) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			List<HtmlElement> elements = QuerySelectorAll (path);
			return elements.Count == 0 ? null : elements[0];
		}

		public List<HtmlElement> QuerySelectorAll (string path) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			TextReader = new StringReader (path);
			List<HtmlElement> elements = new List<HtmlElement> ();
			List<HtmlElement> targetElements = new List<HtmlElement> ();
			List<HtmlElement> tempElements = new List<HtmlElement> ();
			switch (RootNode) {
				case HtmlElement element:
					Root = element;
					break;
				case HtmlDocument document:
					Root = document.Root;
					break;
				default:
					throw new Exception ($"必须是元素或文档元素");
			}
			targetElements.Add (Root);
			bool isChild = true;
			int depth = -1;
			while (MoveNext ()) {
				switch (Current.Type) {
					case HtmlTokenType.Dot: {
						MoveNext ();
						string name = Current;
						if (isChild) {
							Query (targetElements, tempElements, element => element.GetElementsByClassName (name, depth));
							break;
						}
						Filter (targetElements, element => element.ClassList.Contains (name));
						break;
					}
					case HtmlTokenType.NumberSign: {
						MoveNext ();
						string id = Current;
						if (isChild) {
							Query (targetElements, tempElements, element => element.GetElementsByAttribute (HtmlKeyword.ID, id, depth));
							break;
						}
						Filter (targetElements, element => HtmlApi.Equals (element.GetAttribute (HtmlKeyword.ID), id));
						break;
					}
					case HtmlTokenType.Comma: {
						elements.AddRange (targetElements);
						targetElements.Clear ();
						targetElements.Add (Root);
						break;
					}
					case HtmlTokenType.String: {
						string name = Current;
						Query (targetElements, tempElements, element => element.GetElementsByTagName (name, depth));
						break;
					}
					case HtmlTokenType.LeftBracket: {
						MoveNext ();
						if (Current.Type == HtmlTokenType.Integer) {
							HtmlElement element = targetElements[Current.Int];
							targetElements.Clear ();
							targetElements.Add (element);
							MoveNext ();
							break;
						}
						string name = Current;
						MoveNext ();
						switch (Current.Type) {
							case HtmlTokenType.EqualSign:
								MoveNext ();
								string value = Current;
								Filter (targetElements, element => HtmlApi.Equals (element.GetAttribute (name), value));
								MoveNext ();
								break;
							case HtmlTokenType.RightBracket:
								Filter (targetElements, element => element.GetAttributeNode (name) != null);
								break;
						}
						break;
					}
				}
				depth = -1;
				switch (Current.Type) {
					case HtmlTokenType.RightAngleBracket:
						depth = 0;
						break;
				}
				isChild = false;
				if (char.IsWhiteSpace ((char)Peek ())) {
					isChild = true;
				}
			}
			elements.AddRange (targetElements);
			return elements;
		}

		void Query (List<HtmlElement> elements, List<HtmlElement> tempElements, HtmlFunc<HtmlElement, List<HtmlElement>> func) {
			if (elements is null) {
				throw new ArgumentNullException (nameof (elements));
			}
			if (tempElements is null) {
				throw new ArgumentNullException (nameof (tempElements));
			}
			if (func is null) {
				throw new ArgumentNullException (nameof (func));
			}
			tempElements.Clear ();
			foreach (HtmlElement element in elements) {
				tempElements.AddRange (func (element));
			}
			elements.Clear ();
			elements.AddRange (tempElements);
		}

		void Filter (List<HtmlElement> elements, HtmlFunc<HtmlElement, bool> func) {
			if (elements is null) {
				throw new ArgumentNullException (nameof (elements));
			}
			if (func is null) {
				throw new ArgumentNullException (nameof (func));
			}
			for (int i = 0; i < elements.Count; i++) {
				if (!func (elements[i])) {
					elements.RemoveAt (i--);
				}
			}
		}

	}

}