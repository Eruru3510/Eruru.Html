using System;
using System.Collections.Generic;
using System.Text;

namespace Eruru.Html {

	public class HtmlTextReaderException : Exception {

		public HtmlTextReaderException (string message, Queue<int> buffer, HtmlTag tag) {
			if (message is null) {
				throw new ArgumentNullException (nameof (message));
			}
			if (buffer is null) {
				throw new ArgumentNullException (nameof (buffer));
			}
			StringBuilder stringBuilder = new StringBuilder ();
			stringBuilder.AppendLine (message);
			stringBuilder.AppendLine ($"类型：{tag.Type} 索引：{tag.Index} 名或值：{tag.Name ?? tag.Content?.ToString ()}");
			stringBuilder.AppendLine (new string (Array.ConvertAll (buffer.ToArray (), character => (char)character)));
			HtmlAPI.SetExceptionMessage (this, stringBuilder.ToString ());
		}

	}

}