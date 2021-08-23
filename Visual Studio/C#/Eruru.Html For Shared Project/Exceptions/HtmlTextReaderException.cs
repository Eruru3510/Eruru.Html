using Eruru.LexicalAnalyzer;
using System;
using System.Text;

namespace Eruru.Html {

	public class HtmlTextReaderException<T> : Exception where T : Enum {

		public HtmlTextReaderException (string message, LexicalAnalyzer<T> textTokenizer) {
			if (message is null) {
				throw new ArgumentNullException (nameof (message));
			}
			if (textTokenizer is null) {
				throw new ArgumentNullException (nameof (textTokenizer));
			}
			StringBuilder stringBuilder = new StringBuilder ();
			stringBuilder.AppendLine (message);
			stringBuilder.AppendLine (
				$"类型：{textTokenizer.Current.Type} " +
				$"值：{textTokenizer.Current.Value}" +
				$"索引：{textTokenizer.Current.StartIndex} " +
				$"长度：{textTokenizer.Current.Length} " +
				$"行：{textTokenizer.Current.Line} " +
				$"行索引：{textTokenizer.Current.LineStartIndex} "
			);
			HtmlApi.SetExceptionMessage (this, stringBuilder.ToString ());
		}
		public HtmlTextReaderException (LexicalAnalyzer<T> textTokenizer, params object[] values) : this (
			$"期望是{string.Join ("或", Array.ConvertAll (values, value => value.ToString ()))}",
			textTokenizer
		) {
			if (textTokenizer is null) {
				throw new ArgumentNullException (nameof (textTokenizer));
			}
			if (values is null) {
				throw new ArgumentNullException (nameof (values));
			}
		}

	}

}