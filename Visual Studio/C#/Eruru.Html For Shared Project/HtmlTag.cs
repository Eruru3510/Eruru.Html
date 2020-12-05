using System.Collections.Generic;

namespace Eruru.Html {

	public struct HtmlTag {

		public HtmlTagType Type;
		public string Name;
		public int Index;
		public string Content;
		public List<HtmlAttribute> Attributes;

	}

}