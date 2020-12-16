using System;
using System.Windows.Forms;
using Eruru.Html;

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
				Html html = Html.Parse (TextBox_Input.Text);
				TextBox_Output.Text = html.InnerHtml;
			} catch (Exception exception) {
				TextBox_Output.Text = exception.ToString ();
			}
		}

	}

}