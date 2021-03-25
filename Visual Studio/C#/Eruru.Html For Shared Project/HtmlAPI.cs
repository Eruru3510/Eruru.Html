using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Eruru.Html {

	public delegate TResult HtmlFunc<in T, out TResult> (T arg);

	static class HtmlApi {

		static readonly BindingFlags BindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		static readonly Dictionary<string, char> Unescapes = new Dictionary<string, char> {
			{ "quot", '"' },
			{ "amp", '&' },
			{ "lt", '<' },
			{ "gt", '>' },
			{ "nbsp", ' ' }
		};

		static readonly string[] SingleTagNames = { "meta", "link", "input", "img", "base", "hr", "br", "param" };
		static readonly string[] ContentTagNames = { "script", "style" };

		internal static bool IsSingleTag (string tagName) {
			if (tagName is null) {
				throw new ArgumentNullException (nameof (tagName));
			}
			return Array.Exists (SingleTagNames, singleTagName => Equals (singleTagName, tagName));
		}

		internal static bool IsContentTag (string tagName) {
			if (tagName is null) {
				throw new ArgumentNullException (nameof (tagName));
			}
			return Array.Exists (ContentTagNames, contentTagName => Equals (contentTagName, tagName));
		}

		internal static bool HasFlag (Enum a, Enum b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return (a.GetHashCode () & b.GetHashCode ()) != 0;
		}

		internal static void SetExceptionMessage (object instance, string message) {
			if (instance is null) {
				throw new ArgumentNullException (nameof (instance));
			}
			if (message is null) {
				throw new ArgumentNullException (nameof (message));
			}
			typeof (Exception).GetField ("_message", BindingFlags).SetValue (instance, message);
		}

		internal static bool Equals (string a, string b) {
			return string.Equals (a, b, StringComparison.OrdinalIgnoreCase);
		}

		internal static string[] Split (string text) {
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

		internal static bool Contains (string[] a, HtmlAttribute b) {
			for (int i = 0; i < a.Length; i++) {
				if (!b.Values.Contains (a[i])) {
					return false;
				}
			}
			return true;
		}

		internal static bool IsNullOrWhiteSpace (string text) {
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

		public static string Unescape (string text) {
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}
			StringBuilder stringBuilder = new StringBuilder ();
			for (int i = 0; i < text.Length; i++) {
				switch (text[i]) {
					case ' ':
						EatSpace (text, ref i, out _);
						stringBuilder.Append (' ');
						break;
					case '&':
						string value = GetValue (text, ref i);
						if (Unescapes.TryGetValue (value, out char symbol)) {
							stringBuilder.Append (symbol);
							continue;
						}
						stringBuilder.Append ($"&{value};");
						break;
					default:
						stringBuilder.Append (text[i]);
						break;
				}
			}
			return stringBuilder.ToString ();
		}

		public static string UnescapeAttributeValue (string text) {//todo 未完善
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}
			StringBuilder stringBuilder = new StringBuilder ();
			for (int i = 0; i < text.Length; i++) {
				if (text[i] == '&') {
					string value = GetValue (text, ref i);
					if (value == "quot") {
						stringBuilder.Append (Unescapes[value]);
						continue;
					}
					stringBuilder.Append ($"&{value};");
					continue;
				}
				stringBuilder.Append (text[i]);
			}
			return stringBuilder.ToString ();
		}

		public static string CancelUnescape (string text) {
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}
			StringBuilder stringBuilder = new StringBuilder ();
			for (int i = 0; i < text.Length; i++) {
				if (text[i] == ' ') {
					EatSpace (text, ref i, out int count);
					stringBuilder.Append (' ');
					for (int n = 0; n < count; n++) {
						stringBuilder.Append ("&nbsp;");
					}
					continue;
				}
				if (TryGetUnescapeKey (text[i], out string key)) {
					stringBuilder.Append ($"&{key};");
					continue;
				}
				stringBuilder.Append (text[i]);
			}
			return stringBuilder.ToString ();
		}

		public static string CancelUnescapeAttributeValue (string text) {
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}
			StringBuilder stringBuilder = new StringBuilder ();
			for (int i = 0; i < text.Length; i++) {
				switch (text[i]) {
					case ' ': {
						EatSpace (text, ref i, out int count);
						stringBuilder.Append (' ');
						for (int n = 0; n < count; n++) {
							stringBuilder.Append ("&nbsp;");
						}
						break;
					}
					case '"':
						stringBuilder.Append ("&quot;");
						break;
					default:
						stringBuilder.Append (text[i]);
						break;
				}
			}
			return stringBuilder.ToString ();
		}

		static string GetValue (string text, ref int i) {
			StringBuilder stringBuilder = new StringBuilder ();
			for (i++; i < text.Length; i++) {
				if (text[i] == ';') {
					break;
				}
				stringBuilder.Append (text[i]);
			}
			return stringBuilder.ToString ();
		}

		static bool TryGetUnescapeKey (char value, out string key) {
			foreach (KeyValuePair<string, char> unescape in Unescapes) {
				if (unescape.Value == value) {
					key = unescape.Key;
					return true;
				}
			}
			key = null;
			return false;
		}

		static void EatSpace (string text, ref int i, out int count) {
			count = 0;
			for (i++; i < text.Length; i++) {
				if (text[i] == ' ') {
					count++;
					continue;
				}
				break;
			}
			i--;
		}

	}

}