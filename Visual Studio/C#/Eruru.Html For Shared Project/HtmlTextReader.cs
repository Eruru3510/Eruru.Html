using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Eruru.TextTokenizer;

namespace Eruru.Html {

	public class HtmlTextReader : IDisposable, IEnumerator<HtmlTag>, IEnumerable<HtmlTag> {

		protected readonly TextTokenizer<HtmlTokenType> TextTokenizer = new TextTokenizer<HtmlTokenType> (
			HtmlTokenType.End,
			HtmlTokenType.String,
			HtmlTokenType.String,
			HtmlTokenType.String,
			HtmlTokenType.String
		) {
			{ HtmlKeyword.LeftAngleBracket, HtmlTokenType.LeftAngleBracket },
			{ HtmlKeyword.RightAngleBracket, HtmlTokenType.RightAngleBracket },
			{ HtmlKeyword.ExclamationMark, HtmlTokenType.ExclamationMark },
			{ HtmlKeyword.Slash, HtmlTokenType.Slash },
			{ HtmlKeyword.EqualSign, HtmlTokenType.EqualSign },
			{ HtmlKeyword.MinusSign, HtmlTokenType.MinusSign }
		};

		readonly string[] SingleTags = { "meta", "link", "input", "img", "base", "hr", "br", "param" };

		HtmlTag _Current;
		bool NeedMoveNext = true;

		public HtmlTextReader (TextReader textReader) {
			if (textReader is null) {
				throw new ArgumentNullException (nameof (textReader));
			}
			TextTokenizer.TextReader = textReader;
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
							break;
						}
						switch (TextTokenizer.Current.Type) {
							case HtmlTokenType.EqualSign:
								TextTokenizer.MoveNext ();
								switch (TextTokenizer.Current.Type) {
									case HtmlTokenType.String:
										attribute.Values = new List<string> ();
										if (HtmlApi.Equals (attribute.Name, HtmlKeyword.Class)) {
											attribute.Values.AddRange (HtmlApi.Split (TextTokenizer.Current));
										} else {
											attribute.Values.Add (TextTokenizer.Current);
										}
										TextTokenizer.MoveNext ();
										continue;
								}
								throw new HtmlTextReaderException<HtmlTokenType> (TextTokenizer, "属性值");
							case HtmlTokenType.String:
								continue;
							default:
								throw new HtmlTextReaderException<HtmlTokenType> (TextTokenizer, HtmlKeyword.EqualSign, HtmlKeyword.Slash, HtmlKeyword.RightAngleBracket);
						}
					default:
						throw new HtmlTextReaderException<HtmlTokenType> (TextTokenizer, "属性名", HtmlKeyword.Slash, HtmlKeyword.RightAngleBracket);
				}
			}
			return attributes;
		}

		bool IsEnd (ref bool isSingle) {
			switch (TextTokenizer.Current.Type) {
				case HtmlTokenType.Slash:
					CheckRightAngleBracket ();
					isSingle = true;
					return true;
				case HtmlTokenType.RightAngleBracket:
					isSingle = false;
					return true;
			}
			return false;
		}

		void CheckRightAngleBracket () {
			TextTokenizer.MoveNext ();
			if (TextTokenizer.Current.Type == HtmlTokenType.RightAngleBracket) {
				return;
			}
			throw new HtmlTextReaderException<HtmlTokenType> (TextTokenizer, HtmlKeyword.RightAngleBracket);
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
			TextTokenizer.SkipWhiteSpace ();
			HtmlTag tag = new HtmlTag {
				Index = TextTokenizer.Index
			};
			if (TextTokenizer.Peek () == -1) {
				tag.Type = HtmlTagType.End;
				Current = tag;
				return false;
			}
			switch (TextTokenizer.Character) {
				case HtmlKeyword.LeftAngleBracket:
					TextTokenizer.Read ();
					TextTokenizer.MoveNext ();
					switch (TextTokenizer.Current.Type) {
						case HtmlTokenType.String: {
							tag.Type = HtmlTagType.Start;
							tag.Name = TextTokenizer.Current;
							bool isSingle = false;
							tag.Attributes = GetAttributes (ref isSingle);
							if (isSingle == false) {
								isSingle = Array.IndexOf (SingleTags, tag.Name) != -1;
							}
							if (isSingle) {
								tag.Type = HtmlTagType.Single;
							}
							Current = tag;
							return true;
						}
						case HtmlTokenType.ExclamationMark: {
							TextTokenizer.MoveNext ();
							switch (TextTokenizer.Current.Type) {
								case HtmlTokenType.String:
									tag.Type = HtmlTagType.Define;
									tag.Name = TextTokenizer.Current;
									bool isSingle = false;
									tag.Attributes = GetAttributes (ref isSingle);
									break;
								case HtmlTokenType.MinusSign:
									TextTokenizer.MoveNext ();
									if (TextTokenizer.Current.Type != HtmlTokenType.MinusSign) {
										throw new HtmlTextReaderException<HtmlTokenType> (TextTokenizer, HtmlKeyword.MinusSign);
									}
									tag.Type = HtmlTagType.Comment;
									TextTokenizer.SkipWhiteSpace ();
									tag.Content = TextTokenizer.ReadTo (HtmlKeyword.CommentTail).TrimEnd ();
									TextTokenizer.Read ();
									break;
								default:
									throw new HtmlTextReaderException<HtmlTokenType> (TextTokenizer, "标签名", HtmlKeyword.MinusSign);
							}
							Current = tag;
							return true;
						}
						case HtmlTokenType.Slash:
							tag.Type = HtmlTagType.End;
							tag.Name = GetName ();
							CheckRightAngleBracket ();
							Current = tag;
							return true;
						default:
							throw new HtmlTextReaderException<HtmlTokenType> (TextTokenizer, "标签名", HtmlKeyword.ExclamationMark, HtmlKeyword.Slash);
					}
				default:
					tag.Type = HtmlTagType.Text;
					tag.Content = TextTokenizer.ReadTo ("<", true);
					Current = tag;
					return true;
			}
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