using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Eruru.Html {

	public delegate TResult HtmlFunc<in T, out TResult> (T arg);

	static class HtmlApi {

		static readonly BindingFlags BindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		static readonly KeyValuePair<char, char>[] Escapes = new KeyValuePair<char, char>[] {
			new KeyValuePair<char, char> ('\\', '\\'),
			new KeyValuePair<char, char> ('\'', '\''),
			new KeyValuePair<char, char> ('"', '"'),
			new KeyValuePair<char, char> ('r', '\r'),
			new KeyValuePair<char, char> ('n', '\n'),
			new KeyValuePair<char, char> ('t', '\t')
		};

		public static bool HasFlag (Enum a, Enum b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return (a.GetHashCode () & b.GetHashCode ()) != 0;
		}

		public static void SetExceptionMessage (object instance, string message) {
			if (instance is null) {
				throw new ArgumentNullException (nameof (instance));
			}
			if (IsNullOrWhiteSpace (message)) {
				throw new ArgumentException ($"“{nameof (message)}”不能为 Null 或空白", nameof (message));
			}
			typeof (Exception).GetField ("_message", BindingFlags).SetValue (instance, message);
		}

		public static string Unescape (string text) {
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}
			StringBuilder stringBuilder = new StringBuilder ();
			for (int i = 0; i < text.Length; i++) {
				int index = Array.FindIndex (Escapes, escape => escape.Value == text[i]);
				if (index > -1) {
					stringBuilder.Append (HtmlKeyword.Backslash);
					stringBuilder.Append (Escapes[index].Key);
					continue;
				}
				stringBuilder.Append (text[i]);
			}
			return stringBuilder.ToString ();
		}

		public static bool Equals (string a, string b) {
			return string.Equals (a, b, StringComparison.CurrentCultureIgnoreCase);
		}

		public static HtmlElementType TagTypeToElementType (HtmlTagType type) {
			switch (type) {
				case HtmlTagType.Text:
					return HtmlElementType.Text;
				case HtmlTagType.Comment:
					return HtmlElementType.Comment;
				case HtmlTagType.Define:
					return HtmlElementType.Define;
				case HtmlTagType.Single:
					return HtmlElementType.Single;
				case HtmlTagType.Start:
				case HtmlTagType.End:
					return HtmlElementType.Double;
				default:
					throw new HtmlNotSupportException (type);
			}
		}

		public static string[] Split (string text) {
			List<string> values = new List<string> ();
			StringBuilder stringBuilder = new StringBuilder ();
			for (int i = 0; i < text.Length; i++) {
				if (char.IsWhiteSpace (text[i])) {
					if (stringBuilder.Length > 0) {
						values.Add (stringBuilder.ToString ());
						stringBuilder.Remove (0, stringBuilder.Length);
					}
					continue;
				}
				stringBuilder.Append (text[i]);
			}
			if (stringBuilder.Length > 0) {
				values.Add (stringBuilder.ToString ());
			}
			return values.ToArray ();
		}

		public static bool Contains (string[] a, HtmlAttribute b) {
			for (int i = 0; i < a.Length; i++) {
				if (!b.Values.Contains (a[i])) {
					return false;
				}
			}
			return true;
		}

		public static bool IsNullOrWhiteSpace (string text) {
			if (text is null) {
				return true;
			}
			if (text.Length == 0) {
				return true;
			}
			foreach (char character in text) {
				if (!char.IsWhiteSpace (character)) {
					return false;
				}
			}
			return true;
		}

	}

}