using Eruru.LexicalAnalyzer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Eruru.Html {

	public class HtmlTextReader : IDisposable, IEnumerator<HtmlTag>, IEnumerable<HtmlTag> {

		protected readonly LexicalAnalyzer<HtmlTokenType> TextTokenizer = new LexicalAnalyzer<HtmlTokenType> (
			HtmlTokenType.End,
			HtmlTokenType.String,
			HtmlTokenType.String,
			HtmlTokenType.String,
			HtmlTokenType.String,
			true
		);
		protected readonly Stack<HtmlTag> Buffer = new Stack<HtmlTag> ();

		HtmlTag _Current;
		bool NeedMoveNext = true;

		public HtmlTextReader (TextReader textReader) {
			if (textReader is null) {
				throw new ArgumentNullException (nameof (textReader));
			}
			TextTokenizer.TextReader = textReader;
			TextTokenizer.Characters.Add (HtmlKeyword.LeftAngleBracket, HtmlTokenType.LeftAngleBracket);
			TextTokenizer.Characters.Add (HtmlKeyword.RightAngleBracket, HtmlTokenType.RightAngleBracket);
			TextTokenizer.Characters.Add (HtmlKeyword.EqualSign, HtmlTokenType.EqualSign);
			TextTokenizer.AddSymbol (HtmlKeyword.DefineTag, HtmlTokenType.DefineTag);
			TextTokenizer.AddSymbol (HtmlKeyword.SingleTag, HtmlTokenType.SingleTag);
			TextTokenizer.AddSymbol (HtmlKeyword.EndTag, HtmlTokenType.EndTag);
			TextTokenizer.AddBlock (HtmlTokenType.BlockComment, HtmlKeyword.BlockCommentHead, HtmlKeyword.BlockCommentTail);
			TextTokenizer.AllowCharactersBreakKeyword = false;
			TextTokenizer.BreakKeywordCharacters.Add (HtmlKeyword.EqualSign);
			TextTokenizer.BreakKeywordCharacters.Add (HtmlKeyword.Slash);
			TextTokenizer.BreakKeywordCharacters.Add (HtmlKeyword.RightAngleBracket);
		}

		string GetName () {
			TextTokenizer.MoveNext ();
			if (TextTokenizer.Current.Type == HtmlTokenType.String) {
				return TextTokenizer.Current;
			}
			throw new HtmlTextReaderException<HtmlTokenType> (TextTokenizer, "标签名");
		}

		List<HtmlAttribute> GetAttributes (ref bool isSingle) {
			List<HtmlAttribute> attributes = new List<HtmlAttribute> ();
			TextTokenizer.MoveNext ();
			while (true) {
				if (IsEnd (ref isSingle)) {
					break;
				}
				switch (TextTokenizer.Current.Type) {
					case HtmlTokenType.String:
						HtmlAttribute attribute = new HtmlAttribute (TextTokenizer.Current);
						attributes.Add (attribute);
						TextTokenizer.MoveNext ();
						if (IsEnd (ref isSingle)) {
							return attributes;
						}
						switch (TextTokenizer.Current.Type) {
							case HtmlTokenType.EqualSign:
								TextTokenizer.MoveNext ();
								switch (TextTokenizer.Current.Type) {
									case HtmlTokenType.String:
										attribute.Values = new List<string> ();
										string value = HtmlApi.Unescape (TextTokenizer.Current.String);
										if (HtmlApi.Equals (attribute.Name, HtmlKeyword.Class)) {
											attribute.Values.AddRange (HtmlApi.Split (value));
										} else {
											attribute.Values.Add (value);
										}
										TextTokenizer.MoveNext ();
										continue;
								}
								throw new HtmlTextReaderException<HtmlTokenType> (TextTokenizer, "属性值");
							case HtmlTokenType.String:
								continue;
							default:
								throw new HtmlTextReaderException<HtmlTokenType> (TextTokenizer, HtmlKeyword.EqualSign, HtmlKeyword.SingleTag, HtmlKeyword.RightAngleBracket);
						}
					default:
						throw new HtmlTextReaderException<HtmlTokenType> (TextTokenizer, "属性名", HtmlKeyword.SingleTag, HtmlKeyword.RightAngleBracket);
				}
			}
			return attributes;
		}

		bool IsEnd (ref bool isSingle) {
			switch (TextTokenizer.Current.Type) {
				case HtmlTokenType.SingleTag:
					isSingle = true;
					return true;
				case HtmlTokenType.RightAngleBracket:
					isSingle = false;
					return true;
			}
			return false;
		}

		#region IDisposable

		public void Dispose () {
			TextTokenizer.Dispose ();
		}

		#endregion

		#region IEnumerator<HtmlTag>

		public HtmlTag Current {

			get {
				if (NeedMoveNext) {
					MoveNext ();
				}
				return _Current;
			}

			private set => _Current = value;

		}

		object IEnumerator.Current => Current;

		public bool MoveNext () {
			NeedMoveNext = false;
			if (Buffer.Count != 0) {
				Current = Buffer.Pop ();
				return true;
			}
			TextTokenizer.SkipIgnoreCharacters ();
			HtmlTag tag = new HtmlTag {
				Index = TextTokenizer.Index
			};
			if (TextTokenizer.Peek () == -1) {
				Current = tag;
				return false;
			}
			switch (TextTokenizer.Peek ()) {
				case HtmlKeyword.LeftAngleBracket:
					TextTokenizer.MoveNext ();
					switch (TextTokenizer.Current.Type) {
						case HtmlTokenType.LeftAngleBracket: {
							tag.Type = HtmlTagType.Start;
							tag.Name = GetName ();
							bool isSingle = false;
							tag.Attributes = GetAttributes (ref isSingle);
							if (!isSingle) {
								isSingle = HtmlApi.IsSingleTag (tag.Name);
							}
							if (isSingle) {
								tag.Type = HtmlTagType.Single;
							}
							break;
						}
						case HtmlTokenType.DefineTag: {
							tag.Type = HtmlTagType.Define;
							tag.Name = GetName ();
							bool isSingle = false;
							tag.Attributes = GetAttributes (ref isSingle);
							break;
						}
						case HtmlTokenType.BlockComment:
							tag.Type = HtmlTagType.Comment;
							tag.Content = TextTokenizer.Current.String.TrimEnd ();
							break;
						case HtmlTokenType.EndTag:
							tag.Type = HtmlTagType.End;
							tag.Name = GetName ();
							TextTokenizer.MoveNext ();
							if (TextTokenizer.Current.Type != HtmlTokenType.RightAngleBracket) {
								throw new HtmlTextReaderException<HtmlTokenType> (TextTokenizer, HtmlKeyword.RightAngleBracket);
							}
							break;
						default:
							throw new HtmlTextReaderException<HtmlTokenType> (TextTokenizer, "标签名", HtmlKeyword.EndTag, HtmlKeyword.DefineTag, HtmlKeyword.BlockCommentHead);
					}
					break;
				default:
					tag.Type = HtmlTagType.Text;
					tag.Content = TextTokenizer.ReadTo ("<", false).TrimEnd ();
					break;
			}
			Current = tag;
			return true;
		}

		public void Reset () {
			throw new Exception ($"{nameof (TextReader)}无法{nameof (Reset)}");
		}

		#endregion

		#region IEnumerable<HtmlTag>

		public IEnumerator<HtmlTag> GetEnumerator () {
			throw new NotImplementedException ();
		}

		IEnumerator IEnumerable.GetEnumerator () {
			throw new NotImplementedException ();
		}

		#endregion

	}

}