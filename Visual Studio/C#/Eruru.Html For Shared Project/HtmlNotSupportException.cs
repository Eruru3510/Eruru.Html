using System;

namespace Eruru.Html {

	public class HtmlNotSupportException : Exception {

		public HtmlNotSupportException (object value) {
			HtmlAPI.SetExceptionMessage (this, $"不支持{value}");
		}

	}

}