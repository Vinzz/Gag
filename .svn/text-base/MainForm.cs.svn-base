/*
 * Created vincent.tollu@gmail.com
 * IDE: SharpDevelop
 * Distributed under the GPL
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Resources;
using System.Reflection;
using System.IO;

namespace Gag
{
    public partial class MainForm : Form
    {
        private GagData m_gData;
		private htAccessForm myHTAFrom;
		
        public GagData Data
        {
            get { return m_gData; }
            set { m_gData = value; }
        }

        public MainForm()
        {
            InitializeComponent();
            myHTAFrom = new htAccessForm();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	 if((_workerThread != null) && _workerThread.IsAlive)
        		return;
        	            
            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog1.ShowDialog();
        }

        //
		// TODO: Different colors for optional fields?
		//
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            InitFiles.ComsumeInitFile(openFileDialog1.FileNames[0].ToString(), ref m_gData);
            FillControls();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
        	FillData();
            InitFiles.CreateInitFile(saveFileDialog1.FileNames[0].ToString(), m_gData);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        	if(m_gData == null) m_gData = new GagData();
            FillControls();
        }
		
        private void FillData()
        {
        	m_gData.InputPath = txtPicDir.Text;
            m_gData.httpServer = txtHttp.Text;
            m_gData.ftpServer = txtFTP.Text;
            m_gData.ftpUser = txtFTPUser.Text;
            m_gData.clearftpPassword = txtFTPPassword.Text;
            m_gData.MailServer = txtMailServer.Text;
            m_gData.MailFrom = txtMailFrom.Text;
            m_gData.MailTo = txtMailTo.Text;
            m_gData.MailSubject = txtMailSubject.Text;
            m_gData.MailText = txtMailBody.Text;
            m_gData.htUser = myHTAFrom.htLogin;
            m_gData.htPassword = myHTAFrom.htPassword;
            m_gData.htmltemplate = txtHtmlTemplate.Text;
            m_gData.ZipArchive = CheckZip.Checked;
            m_gData.LightBox = CheckLightBox.Checked;       
          	m_gData.OutputPath = txtOutDir.Text;
            
        }
        private void FillControls()
        {
            txtPicDir.Text = m_gData.InputPath;
            txtHttp.Text = m_gData.httpServer;
            txtFTP.Text = m_gData.ftpServer;
            txtFTPUser.Text = m_gData.ftpUser;
            txtFTPPassword.Text = m_gData.clearftpPassword;
            txtMailServer.Text = m_gData.MailServer;
            txtMailFrom.Text = m_gData.MailFrom;
            txtMailTo.Text = m_gData.MailTo;
            txtMailSubject.Text = m_gData.MailSubject;
            txtMailBody.Text = m_gData.MailText.Replace("\n","\r\n");
            txtOutDir.Text = m_gData.OutputPath;
            myHTAFrom.htLogin = m_gData.htUser;
            myHTAFrom.htPassword = m_gData.htPassword;
            txtHtmlTemplate.Text = m_gData.htmltemplate;
            CheckZip.Checked = m_gData.ZipArchive;
            CheckLightBox.Checked = m_gData.LightBox;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if((_workerThread != null) && _workerThread.IsAlive)
        		return;
                    	
        	saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog1.ShowDialog();
        }

        private void btnBrowseForPicDir_Click(object sender, EventArgs e)
        {
        	if((_workerThread != null) && _workerThread.IsAlive)
        		return;

            if (Directory.Exists(txtPicDir.Text))
                folderBrowserDialog1.SelectedPath = txtPicDir.Text;
            
             if(folderBrowserDialog1.ShowDialog() == DialogResult.OK)
             {
                txtPicDir.Text = folderBrowserDialog1.SelectedPath;
                
                if(txtOutDir.Text == "" || txtOutDir.Text.Contains(Environment.GetEnvironmentVariable("TEMP")))
	            {
	            	DirectoryInfo currentDir = new DirectoryInfo(txtPicDir.Text);
	                string st = Environment.GetEnvironmentVariable("TEMP") + @"\" + currentDir.Name;
	                
	                //FTP servers don't like unicode chars, let's avoid 
	                //some pretty common mistakes with french inputs
	                st = st.Replace('é','e');
	                st = st.Replace('è','e');
	                st = st.Replace('ê','e');
	                st = st.Replace('ù','u');
	                st = st.Replace('à','a');
	                st = st.Replace('ç','c');
	                st = st.Replace(" ","");
	                
	                txtOutDir.Text = st;
	            }
             }
        }

        private Thread _workerThread = null;
        private void btnOK_Click(object sender, EventArgs e)
        {
        	if((_workerThread != null) && _workerThread.IsAlive)
        		return;
        	
        	FillData();
            InitFiles.CreateInitFile(m_gData);
            
            //Check inputs
            if(InitFiles.AreInputsOK(m_gData))
            {
            	//Launch the main process
            	GagPictures.GData = m_gData;
            	GagPictures.SendMessageHandler += new GagPicturesMessageHandler(onGagMessage);
				GagPictures.PerformStepHandler += new GagPicturesPerformStepHandler(onGagPerformStep);
				GagPictures.ChangeprogressHandler += new GagPicturesChangeprogressBarHandler(onGagChangeProgress);
				GagPictures.ChangecursorHandler += new GagPicturesChangeCursorHandler(onGagChangeCursor);
				GagPictures.endProcHandler += new GagPicturesEndProcHandler(onGagEndProc);
				
            	_workerThread = new Thread(GagPictures.MainGagProc);

				_workerThread.Start();
            }
        }
        
        void onGagEndProc()
        {
        	if (this.InvokeRequired)
		    {
		        // Execute the same method, but this time on the GUI thread
		        this.BeginInvoke(new GagPicturesEndProcHandler(onGagEndProc));
		        // we return immedeately
		        return;
		    }
        	
        	this.Close();
        }
        
        void onGagChangeCursor(Cursor newCursor)
        {
        	if (this.InvokeRequired)
		    {
		        // Execute the same method, but this time on the GUI thread
		        this.BeginInvoke(new GagPicturesChangeCursorHandler(onGagChangeCursor), newCursor);
		        // we return immedeately
		        return;
		    }
        	this.Cursor = newCursor;
        }
        
		void onGagChangeProgress(int iProgressMax)
		{
			if (this.InvokeRequired)
		    {
		        // Execute the same method, but this time on the GUI thread
		        this.BeginInvoke(new GagPicturesChangeprogressBarHandler(onGagChangeProgress), iProgressMax);
		        // we return immedeately
		        return;
		    }
			
			this.toolStripProgressBar1.Visible = true;
			this.toolStripProgressBar1.Minimum = 1;
            this.toolStripProgressBar1.Maximum = iProgressMax;
            this.toolStripProgressBar1.Value = 1;
            this.toolStripProgressBar1.Step = 1;
		}
		void onGagPerformStep()
		{
			if (this.InvokeRequired)
		    {
		        // Execute the same method, but this time on the GUI thread
		        this.BeginInvoke(new GagPicturesPerformStepHandler(onGagPerformStep));
		        // we return immedeately
		        return;
		    }
			
			this.toolStripProgressBar1.PerformStep();
		}
	
        void onGagMessage(string msg)
        {
        	if (this.InvokeRequired)
		    {
		        // Execute the same method, but this time on the GUI thread
		        this.BeginInvoke(new GagPicturesMessageHandler(onGagMessage), msg);
		        // we return immedeately
		        return;
		    }
        	
        	this.toolStripStatusLabel.Text = msg;
        }
        
        void BrowseForOutDirClick(object sender, System.EventArgs e)
        {
        	if((_workerThread != null) && _workerThread.IsAlive)
        		return;
        	            
        	if(folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                txtOutDir.Text = folderBrowserDialog1.SelectedPath;
        }
        
        void BtnProtectClick(object sender, EventArgs e)
        {
        	if((_workerThread != null) && _workerThread.IsAlive)
        		return;
        	            
        	myHTAFrom.Show();
        }
        
        void BrowseForHtmlClick(object sender, EventArgs e)
        {
        	if((_workerThread != null) && _workerThread.IsAlive)
        		return;
        	            
        	openFileForHtmlTemplate.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileForHtmlTemplate.ShowDialog();
        }

        void openFileForHtmlTemplate_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
        	txtHtmlTemplate.Text = openFileForHtmlTemplate.FileNames[0].ToString();
        }
        
        void AboutToolStripMenuItem1Click(object sender, System.EventArgs e)
        {
        	About myAForm = new About();
        	myAForm.Show();
        }
        
        void HelpToolStripMenuItemClick(object sender, System.EventArgs e)
        {
        	if(StaticTools.IsFrench())
        		System.Diagnostics.Process.Start("http://yocto.projects.free.fr/Programs/Gag/GagHelpFr.html");
        	else
        		System.Diagnostics.Process.Start("http://yocto.projects.free.fr/Programs/Gag/GagHelp.html");
        }
        
        void CheckLightBoxCheckedChanged(object sender, System.EventArgs e)
        {
        	m_gData.LightBox = CheckLightBox.Checked;
        }
        
        void CheckZipCheckedChanged(object sender, System.EventArgs e)
        {
        	m_gData.ZipArchive = CheckZip.Checked;
        }
    }
}
