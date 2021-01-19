using System;
using System.Collections.Generic;

namespace Eruru.Html {

	class HtmlDocumentType : HtmlNode {

		public List<HtmlAttribute> Attributes { get; }

		public HtmlDocumentType (List<HtmlAttribute> attributes) : base (HtmlNodeType.DocumentType, "文档类型名（暂未实现）", null) {
			Attributes = attributes ?? throw new ArgumentNullException (nameof (attributes));
		}

	}

}