using System;

namespace Eruru.Html {

	public enum HtmlTagType {

		Unknown = 1 << 0,
		Text = 1 << 1,
		Start = 1 << 2,
		End = 1 << 3,
		Single = 1 << 4,
		Define = 1 << 5,
		Comment = 1 << 6,
		CanHasAttribute = Start | Single | Define

	}

}