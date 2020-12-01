using System.Collections.Generic;
using System.Text;

namespace Eruru.Html {

	public struct HtmlTag {

		public HtmlTagType Type;
		public string Name;
		public int Index;
		public string Content;
		public List<HtmlAttribute> Attributes;

	}

}