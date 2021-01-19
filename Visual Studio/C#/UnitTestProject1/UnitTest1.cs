using System.IO;
using Eruru.Html;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1 {

	[TestClass]
	public class UnitTest1 {

		const string AssetsPath = @"..\..\..\Assets";

		[TestMethod]
		public void TestMethod1 () {
			string output = HtmlDocument.Load ($@"{AssetsPath}\Input.html").InnerHtml;
			string result = File.ReadAllText ($@"{AssetsPath}\Result.html");
			Assert.AreEqual (result, output);
		}

	}

}