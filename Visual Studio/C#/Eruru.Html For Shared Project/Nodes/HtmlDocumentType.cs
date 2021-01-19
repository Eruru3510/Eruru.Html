using System;
using System.Collections.Generic;

namespace Eruru.Html {

	class HtmlDocumentType : HtmlNode {

		public List<HtmlAttribute> Attributes { get; }

		public HtmlDocumentType (List<HtmlAttribute> attributes, HtmlElement parentElement) : base (HtmlNodeType.DocumentType, (attributes?.Count ?? 0) == 0 ? null : attributes[0].Value, null, parentElement) {
			Attributes = attributes ?? throw new ArgumentNullException (nameof (attributes));
		}

	}

}