using System;
using System.Text;
using Eruru.TextTokenizer;

namespace Eruru.Html {

	public class HtmlTextReaderException<T> : Exception where T : Enum {

		public HtmlTextReaderException (string message, TextTokenizer<T> textTokenizer) {
			if (HtmlApi.IsNullOrWhiteSpace (message)) {
				throw new ArgumentException ($"“{nameof (message)}”不能为 Null 或空白", nameof (message));
			}
			StringBuilder stringBuilder = new StringBuilder ();
			stringBuilder.AppendLine (message);
			stringBuilder.AppendLine (
				$"类型：{textTokenizer.Current.Type} " +
				$"位置：{textTokenizer.Current.Index} " +
				$"长度：{textTokenizer.Current.Length} " +
				$"值：{textTokenizer.Current.Value}"
			);
			stringBuilder.AppendLine (new string (textTokenizer.Buffer.ToArray ()));
			HtmlApi.SetExceptionMessage (this, stringBuilder.ToString ());
		}
		public HtmlTextReaderException (TextTokenizer<T> textTokenizer, params object[] values) : this (
			$"期望是{string.Join ("或", Array.ConvertAll (values, value => value.ToString ()))}",
			textTokenizer
		) {
			if (values is null) {
				throw new ArgumentNullException (nameof (values));
			}
		}

	}

}