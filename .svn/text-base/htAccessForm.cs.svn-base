/*
 * Created vincent.tollu@gmail.com
 * IDE: SharpDevelop
 * Distributed under the GPL
 */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Gag
{
	/// <summary>
	/// Description of htAccessForm.
	/// </summary>
	public partial class htAccessForm : Form
	{
		public string htLogin
        {
            get { return this.txtHTLogin.Text; }
            set { this.txtHTLogin.Text = value; }
        }
		
		public string htPassword
        {
            get { return this.txtHTPassword.Text; }
            set { this.txtHTPassword.Text = value; }
        }
		
		public htAccessForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
		}
		
		void BtnOKClick(object sender, EventArgs e)
		{
			this.Hide();
		}
		
		void HtAccessFormFormClosing(object sender, FormClosingEventArgs e)
		{
			this.txtHTLogin.Text = "";
			this.txtHTPassword.Text = "";
		}
	}
}
