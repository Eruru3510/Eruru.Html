using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Eruru.Html {

	public class HtmlTextReader : IDisposable, IEnumerator<HtmlTag>, IEnumerable<HtmlTag> {

		public int BufferLength { get; set; } = 500;

		protected readonly Queue<int> Buffer = new Queue<int> ();
		protected readonly TextReader TextReader;

		readonly string[] SingleTags = { "meta", "link", "input", "img", "base", "hr", "br", "param" };

		int Index;

		public HtmlTextReader (TextReader textReader) {
			if (textReader is null) {
				throw new ArgumentNullException (nameof (textReader));
			}
			TextReader = textReader;
		}

		protected int Read () {
			while (Buffer.Count >= BufferLength) {
				Buffer.Dequeue ();
			}
			Buffer.Enqueue (TextReader.Read ());
			Index++;
			return Buffer.Peek ();
		}

		protected string ReadContent (string end = "<") {
			StringBuilder stringBuilder = new StringBuilder ();
			while (TextReader.Peek () > -1) {
				char character = Peek ();
				stringBuilder.Append (character);
				if (Match (stringBuilder, character, end)) {
					break;
				}
				Read ();
			}
			return stringBuilder.ToString ().Trim ();
		}

		char Peek () {
			return (char)TextReader.Peek ();
		}

		char SkipWhiteSpace () {
			while (TextReader.Peek () > -1) {
				char character = Peek ();
				if (char.IsWhiteSpace (character)) {
					Read ();
					continue;
				}
				return character;
			}
			return Peek ();
		}

		string ReadString (char end) {
			Read ();
			int index = Index;
			StringBuilder stringBuilder = new StringBuilder ();
			while (TextReader.Peek () > -1) {
				char character = Peek ();
				if (character == HtmlKeyword.Backslash) {
					stringBuilder.Append (character);
					Read ();
					if (TextReader.Peek () > -1) {
						stringBuilder.Append (Peek ());
						Read ();
					}
					continue;
				}
				if (character == end) {
					Read ();
					return stringBuilder.ToString ();
				}
				stringBuilder.Append (character);
				Read ();
			}
			HtmlTag tag = new HtmlTag {
				Type = HtmlTagType.Text,
				Index = index,
				Content = stringBuilder.ToString ()
			};
			throw new HtmlTextReaderException ("字符串没有引号结束", Buffer, tag);
		}

		string ReadValue (ref bool isSingle) {
			StringBuilder stringBuilder = new StringBuilder ();
			while (TextReader.Peek () > -1) {
				char character = Peek ();
				if (char.IsWhiteSpace (character)) {
					break;
				}
				switch (character) {
					case HtmlKeyword.SingleQuot:
					case HtmlKeyword.DoubleQuot:
						return ReadString (character);
					case HtmlKeyword.Slash:
						Read ();
						character = Peek ();
						if (character == HtmlKeyword.RightAngleBracket) {
							isSingle = true;
							return stringBuilder.ToString ();
						}
						stringBuilder.Append (HtmlKeyword.Slash);
						break;
					case HtmlKeyword.EqualSign:
					case HtmlKeyword.LeftAngleBracket:
					case HtmlKeyword.RightAngleBracket:
						return stringBuilder.ToString ();
				}
				stringBuilder.Append (character);
				Read ();
			}
			return stringBuilder.ToString ();
		}

		List<HtmlAttribute> ReadAttributes (ref bool isSingle) {
			List<HtmlAttribute> attributes = new List<HtmlAttribute> ();
			while (TextReader.Peek () >= 0) {
				char character;
				SkipWhiteSpace ();
				HtmlAttribute attribute = new HtmlAttribute (ReadValue (ref isSingle));
				if (attribute.Name.Length == 0) {
					break;
				}
				attributes.Add (attribute);
				character = SkipWhiteSpace ();
				switch (character) {
					case HtmlKeyword.EqualSign:
						Read ();
						SkipWhiteSpace ();
						attribute.Values = new List<string> ();
						string value = ReadValue (ref isSingle);
						if (HtmlAPI.Equals (attribute.Name, HtmlKeyword.Class)) {
							attribute.Values.AddRange (HtmlAPI.Split (value));
							continue;
						}
						attribute.Values.Add (value);
						continue;
					case HtmlKeyword.RightAngleBracket:
						return attributes;
				}
			}
			return attributes;
		}

		bool Match (StringBuilder stringBuilder, char character, string end) {
			if (stringBuilder.Length < end.Length) {
				return false;
			}
			if (character != end[end.Length - 1]) {
				return false;
			}
			int start = stringBuilder.Length - end.Length;
			for (int i = 0; i < end.Length; i++) {
				if (stringBuilder[start + i] != end[i]) {
					return false;
				}
			}
			stringBuilder.Remove (start, end.Length);
			return true;
		}

		string Expectation (params object[] values) {
			if (values is null) {
				throw new ArgumentNullException (nameof (values));
			}
			return $"期望是{string.Join ("或", Array.ConvertAll (values, value => value.ToString ()))}";
		}

		#region IDisposable

		public void Dispose () {
			TextReader.Dispose ();
		}

		#endregion

		#region IEnumerable<HtmlTag>

		public IEnumerator<HtmlTag> GetEnumerator () {
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator () {
			return this;
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

		}

		object IEnumerator.Current {

			get => Current;

		}
		HtmlTag _Current;
		bool NeedMoveNext = true;

		public bool MoveNext () {
			NeedMoveNext = false;
			HtmlTag tag = new HtmlTag () {
				Type = HtmlTagType.Unknown,
				Index = Index
			};
			while (TextReader.Peek () > -1) {
				char character = SkipWhiteSpace ();
				switch (character) {
					case HtmlKeyword.LeftAngleBracket:
						Read ();
						character = Peek ();
						bool isSingle = false;
						switch (character) {
							case HtmlKeyword.Slash:
								tag.Type = HtmlTagType.End;
								Read ();
								tag.Name = ReadValue (ref isSingle);
								if (tag.Name.Length == 0) {
									throw new HtmlTextReaderException (Expectation ("标签名", HtmlKeyword.MinusSign), Buffer, tag);
								}
								break;
							case HtmlKeyword.ExclamationMark:
								Read ();
								character = Peek ();
								if (character == HtmlKeyword.MinusSign) {
									Read ();
									character = Peek ();
									if (character != HtmlKeyword.MinusSign) {
										throw new HtmlTextReaderException (Expectation (HtmlKeyword.CommentHead), Buffer, tag);
									}
									Read ();
									tag.Type = HtmlTagType.Comment;
									tag.Content = ReadContent (HtmlKeyword.CommentTail);
									Read ();
									_Current = tag;
									return true;
								}
								tag.Type = HtmlTagType.Define;
								tag.Name = ReadValue (ref isSingle);
								if (tag.Name.Length == 0) {
									throw new HtmlTextReaderException (Expectation ("标签名"), Buffer, tag);
								}
								break;
							default:
								tag.Type = HtmlTagType.Start;
								tag.Name = ReadValue (ref isSingle);
								if (tag.Name.Length == 0) {
									throw new HtmlTextReaderException (Expectation ("标签名", HtmlKeyword.ExclamationMark, HtmlKeyword.Slash), Buffer, tag);
								}
								break;
						}
						if (HtmlAPI.HasFlag (tag.Type, HtmlTagType.CanHasAttribute)) {
							tag.Attributes = ReadAttributes (ref isSingle);
						}
						if (isSingle || (tag.Type == HtmlTagType.Start && Array.Exists (SingleTags, tagName => HtmlAPI.Equals (tagName, tag.Name)))) {
							tag.Type = HtmlTagType.Single;
						}
						character = Peek ();
						switch (character) {
							case HtmlKeyword.Slash:
								Read ();
								character = Peek ();
								if (character == HtmlKeyword.RightAngleBracket) {
									tag.Type = HtmlTagType.Single;
									_Current = tag;
									Read ();
									return true;
								}
								break;
							case HtmlKeyword.RightAngleBracket:
								_Current = tag;
								Read ();
								return true;
						}
						if (isSingle) {
							throw new HtmlTextReaderException (Expectation (HtmlKeyword.RightAngleBracket), Buffer, tag);
						}
						throw new HtmlTextReaderException (Expectation ("属性", HtmlKeyword.Slash, HtmlKeyword.RightAngleBracket), Buffer, tag);
					default:
						tag.Type = HtmlTagType.Text;
						tag.Content = ReadContent ();
						_Current = tag;
						return true;
				}
			}
			_Current = tag;
			return false;
		}

		public void Reset () {
			throw new Exception ($"{nameof (TextReader)}无法{nameof (Reset)}");
		}

		#endregion

	}

}