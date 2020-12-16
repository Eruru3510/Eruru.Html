using System;
using System.Collections.Generic;
using System.Text;

namespace Eruru.Html {

	public class HtmlAttribute {

		public string Name { get; set; }
		public string Value {

			get {
				if (Values is null || Values.Count == 0) {
					return string.Empty;
				}
				if (Values.Count == 1) {
					return Values[0];
				}
				StringBuilder stringBuilder = new StringBuilder ();
				for (int i = 0; i < Values.Count; i++) {
					if (i > 0) {
						stringBuilder.Append (' ');
					}
					stringBuilder.Append (Values[i]);
				}
				return stringBuilder.ToString ();
			}

		}
		public List<string> Values { get; set; }

		public HtmlAttribute (string name) {
			Name = name ?? throw new ArgumentNullException (nameof (name));
		}

	}

}