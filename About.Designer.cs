/*
 * Created by SharpDevelop.
 * User: vtollu
 * Date: 4/19/2007
 * Time: 2:02 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace Gag
{
	partial class About
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
			this.lnkLblWebSite = new System.Windows.Forms.LinkLabel();
			this.lblAbout = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lnkLblWebSite
			// 
			this.lnkLblWebSite.AccessibleDescription = null;
			this.lnkLblWebSite.AccessibleName = null;
			this.lnkLblWebSite.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lnkLblWebSite.Anchor")));
			this.lnkLblWebSite.AutoSize = ((bool)(resources.GetObject("lnkLblWebSite.AutoSize")));
			this.lnkLblWebSite.BackgroundImageLayout = ((System.Windows.Forms.ImageLayout)(resources.GetObject("lnkLblWebSite.BackgroundImageLayout")));
			this.lnkLblWebSite.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lnkLblWebSite.Dock")));
			this.lnkLblWebSite.Font = null;
			this.lnkLblWebSite.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lnkLblWebSite.ImageAlign")));
			this.lnkLblWebSite.ImageIndex = ((int)(resources.GetObject("lnkLblWebSite.ImageIndex")));
			this.lnkLblWebSite.ImageKey = resources.GetString("lnkLblWebSite.ImageKey");
			this.lnkLblWebSite.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lnkLblWebSite.ImeMode")));
			this.lnkLblWebSite.Location = ((System.Drawing.Point)(resources.GetObject("lnkLblWebSite.Location")));
			this.lnkLblWebSite.Name = "lnkLblWebSite";
			this.lnkLblWebSite.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lnkLblWebSite.RightToLeft")));
			this.lnkLblWebSite.Size = ((System.Drawing.Size)(resources.GetObject("lnkLblWebSite.Size")));
			this.lnkLblWebSite.TabIndex = ((int)(resources.GetObject("lnkLblWebSite.TabIndex")));
			this.lnkLblWebSite.TabStop = true;
			this.lnkLblWebSite.Text = resources.GetString("lnkLblWebSite.Text");
			this.lnkLblWebSite.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lnkLblWebSite.TextAlign")));
			this.lnkLblWebSite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LnkLblWebSiteLinkClicked);
			// 
			// lblAbout
			// 
			this.lblAbout.AccessibleDescription = null;
			this.lblAbout.AccessibleName = null;
			this.lblAbout.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblAbout.Anchor")));
			this.lblAbout.AutoSize = ((bool)(resources.GetObject("lblAbout.AutoSize")));
			this.lblAbout.BackgroundImageLayout = ((System.Windows.Forms.ImageLayout)(resources.GetObject("lblAbout.BackgroundImageLayout")));
			this.lblAbout.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblAbout.Dock")));
			this.lblAbout.Font = null;
			this.lblAbout.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblAbout.ImageAlign")));
			this.lblAbout.ImageIndex = ((int)(resources.GetObject("lblAbout.ImageIndex")));
			this.lblAbout.ImageKey = resources.GetString("lblAbout.ImageKey");
			this.lblAbout.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblAbout.ImeMode")));
			this.lblAbout.Location = ((System.Drawing.Point)(resources.GetObject("lblAbout.Location")));
			this.lblAbout.Name = "lblAbout";
			this.lblAbout.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblAbout.RightToLeft")));
			this.lblAbout.Size = ((System.Drawing.Size)(resources.GetObject("lblAbout.Size")));
			this.lblAbout.TabIndex = ((int)(resources.GetObject("lblAbout.TabIndex")));
			this.lblAbout.Text = resources.GetString("lblAbout.Text");
			this.lblAbout.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblAbout.TextAlign")));
			// 
			// label1
			// 
			this.label1.AccessibleDescription = null;
			this.label1.AccessibleName = null;
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label1.Anchor")));
			this.label1.AutoSize = ((bool)(resources.GetObject("label1.AutoSize")));
			this.label1.BackgroundImageLayout = ((System.Windows.Forms.ImageLayout)(resources.GetObject("label1.BackgroundImageLayout")));
			this.label1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label1.Dock")));
			this.label1.Font = null;
			this.label1.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label1.ImageAlign")));
			this.label1.ImageIndex = ((int)(resources.GetObject("label1.ImageIndex")));
			this.label1.ImageKey = resources.GetString("label1.ImageKey");
			this.label1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label1.ImeMode")));
			this.label1.Location = ((System.Drawing.Point)(resources.GetObject("label1.Location")));
			this.label1.Name = "label1";
			this.label1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label1.RightToLeft")));
			this.label1.Size = ((System.Drawing.Size)(resources.GetObject("label1.Size")));
			this.label1.TabIndex = ((int)(resources.GetObject("label1.TabIndex")));
			this.label1.Text = resources.GetString("label1.Text");
			this.label1.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label1.TextAlign")));
			// 
			// About
			// 
			this.AccessibleDescription = null;
			this.AccessibleName = null;
			this.AutoScaleDimensions = ((System.Drawing.SizeF)(resources.GetObject("$this.AutoScaleDimensions")));
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoSize = ((bool)(resources.GetObject("$this.AutoSize")));
			this.AutoSizeMode = ((System.Windows.Forms.AutoSizeMode)(resources.GetObject("$this.AutoSizeMode")));
			this.BackgroundImage = null;
			this.BackgroundImageLayout = ((System.Windows.Forms.ImageLayout)(resources.GetObject("$this.BackgroundImageLayout")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lnkLblWebSite);
			this.Controls.Add(this.lblAbout);
			this.Font = null;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximizeBox = false;
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimizeBox = false;
			this.Name = "About";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.RightToLeftLayout = ((bool)(resources.GetObject("$this.RightToLeftLayout")));
			this.ShowInTaskbar = false;
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblAbout;
		private System.Windows.Forms.LinkLabel lnkLblWebSite;
	}
}
