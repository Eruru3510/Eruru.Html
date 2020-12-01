using System;
using System.Collections.Generic;

namespace Eruru.Html {

	public class HtmlAttribute {

		public string Name { get; set; }
		public List<string> Values { get; set; }

		public HtmlAttribute (string name) {
			Name = name ?? throw new ArgumentNullException (nameof (name));
		}

	}

}