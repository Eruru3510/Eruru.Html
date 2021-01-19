using System;
using System.Collections.Generic;
using System.IO;
using Eruru.TextTokenizer;

namespace Eruru.Html {

	public class HtmlSelector : TextTokenizer<HtmlTokenType> {

		readonly HtmlElement Root;

		public HtmlSelector (HtmlElement root) : base (
			HtmlTokenType.End,
			HtmlTokenType.String,
			HtmlTokenType.Integer,
			HtmlTokenType.Decimal,
			HtmlTokenType.String
		) {
			Root = root ?? throw new ArgumentNullException (nameof (root));
			AddSymbol (HtmlKeyword.EqualSign, HtmlTokenType.EqualSign);
			AddSymbol (HtmlKeyword.Comma, HtmlTokenType.Comma);
			AddSymbol (HtmlKeyword.NumberSign, HtmlTokenType.NumberSign);
			AddSymbol (HtmlKeyword.Dot, HtmlTokenType.Dot);
			AddSymbol (HtmlKeyword.LeftBracket, HtmlTokenType.LeftBracket);
			AddSymbol (HtmlKeyword.RightBracket, HtmlTokenType.RightBracket);
			AddSymbol (HtmlKeyword.RightAngleBracket, HtmlTokenType.RightAngleBracket);
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
						string name = Current;
						if (isChild) {
							Query (targetElements, tempElements, element => element.GetElementsByClassName (name, depth));
							break;
						}
						Filter (targetElements, element => HtmlApi.Equals (element.GetAttribute (HtmlKeyword.ID), name));
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