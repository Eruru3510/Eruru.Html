using System;
using System.Windows.Forms;
using HtmlDocument = Eruru.Html.HtmlDocument;

namespace WindowsFormsApp1 {

	public partial class Form1 : Form {

		public Form1 () {
			InitializeComponent ();
		}

		private void Form1_Load (object sender, EventArgs e) {
			Parse ();
		}

		private void TextBox_Input_TextChanged (object sender, EventArgs e) {
			Parse ();
		}

		void Parse () {
			try {
				HtmlDocument htmlDocument = HtmlDocument.Parse (TextBox_Input.Text);
				TextBox_Output.Text = htmlDocument.InnerHtml;
			} catch (Exception exception) {
				TextBox_Output.Text = exception.ToString ();
			}
		}

	}

}