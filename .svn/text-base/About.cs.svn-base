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
	/// Description of About.
	/// </summary>
	public partial class About : Form
	{
		public About()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			this.Text += StaticTools.GetVersion();
		}
		
		void LnkLblWebSiteLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("http://yocto.projects.free.fr/Programs/Gag/Gag.html");
			this.Hide();
		}
	}
}
