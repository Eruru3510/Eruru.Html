using System;

namespace Eruru.Html {

	public class HtmlNotSupportException : Exception {

		public HtmlNotSupportException (object value) {
			HtmlApi.SetExceptionMessage (this, $"不支持{value}");
		}

	}

}