/*
 * Created vincent.tollu@gmail.com
 * IDE: SharpDevelop
 * Distributed under the GPL
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Resources;
using System.Reflection;

namespace Gag
{
	#region GUI events delegates
	public delegate void GagPicturesChangeprogressBarHandler(int iProgressMax); 
	public delegate void GagPicturesPerformStepHandler();
	public delegate void GagPicturesMessageHandler(string Message);
	public delegate void GagPicturesChangeCursorHandler(Cursor newCursor);
	public delegate void GagPicturesEndProcHandler();
	#endregion
	
    public class GagPictures
    {
    	#region events
    	public static event GagPicturesChangeprogressBarHandler ChangeprogressHandler; 
    	public static event GagPicturesPerformStepHandler PerformStepHandler; 
    	public static event GagPicturesMessageHandler SendMessageHandler; 
    	public static event GagPicturesChangeCursorHandler ChangecursorHandler; 
    	public static event GagPicturesEndProcHandler endProcHandler;
		
    	private static void endGUIProc()
    	{
    		if(endProcHandler != null)
    			endProcHandler();
    	}
    	
    	private static void ChangeGUICursor(Cursor newCursor)
    	{
    		if(ChangecursorHandler != null)
    			ChangecursorHandler(newCursor);
    	}
    	
    	private static void ChangeGUIStatusMessage(string stMessage)
    	{
    		if(SendMessageHandler != null)
    			SendMessageHandler(stMessage);
    	}
    	
    	private static void PerformGUIStep()
    	{
    		if(PerformStepHandler != null)
    			PerformStepHandler();
    	}
    	
    	private static void ChangeGUIProgressBar(int iProgressMax)
    	{
    		if(ChangeprogressHandler != null)
    			ChangeprogressHandler(iProgressMax);
    	}
    	#endregion
    	
    	private static GagData _GData;
        public static GagData GData
        {
            set { _GData = value; }
        }
        
        static public void MainGagProc()
        {
            try
            {
            	Stack<FileInfo> stOriginalImages = new Stack<FileInfo>();
            	
                //Access all jpg pictures
                DirectoryInfo currentDir = new DirectoryInfo(_GData.InputPath);
                DirectoryInfo wDir;
			
                if (Directory.Exists(_GData.OutputPath))
                    Directory.Delete(_GData.OutputPath, true);

                wDir = Directory.CreateDirectory(_GData.OutputPath);

                //Resize and copy these in a temp directory
                //Don't forget to rotate these according to EXIf infos
                ChangeGUIProgressBar(currentDir.GetFiles().Length * 2 + 2);
                PerformGUIStep();

                DateTime FileDate = new DateTime();
                
                foreach (FileInfo fileInfo in currentDir.GetFiles())
                {
                    if (Regex.IsMatch(fileInfo.Extension, @".+(jpg)$|.+(jpeg)", RegexOptions.IgnoreCase))
                    {
                    	ChangeGUIStatusMessage(StaticTools.AppStringRManager.GetString("ProgressBarStep1Title"));
                        Image Resized = PicProcessor.ResizeToFixedSizeAndRotate(
                                                fileInfo.FullName,
                                                _GData.PicWidth, _GData.PicHeight,  ref FileDate);

                    	string stSavePath = wDir.FullName + @"\" + FileDate.Year + "_"
                    	             					  + FileDate.Month.ToString("D2") + "_"
                                                          + FileDate.Day.ToString("D2") + "_"
                    	             					  + fileInfo.Name;
                        Resized.Save(stSavePath, ImageFormat.Jpeg);

                        FileInfo fi = new FileInfo(stSavePath);
                        fi.CreationTime = FileDate;
                        
                        Resized.Dispose();
                        Resized = null;
                        GC.Collect();
                     	PerformGUIStep();
                    }

                    if (Regex.IsMatch(fileInfo.Extension, @".+(mov)$|.+(mpg)|.+(avi)", RegexOptions.IgnoreCase))
                    {
                    	ChangeGUIStatusMessage(StaticTools.AppStringRManager.GetString("ProgressBarStep1VideoTitle").Replace("%v%",fileInfo.Name));
                    	VideoProcessor.Convert2Theora(fileInfo.FullName, wDir.FullName, _GData.MiniPicWidth * 3, _GData.MiniPicHeight * 3);
                    	PerformGUIStep();
                    }
                    
                    if(_GData.ZipArchive == true)
                        	stOriginalImages.Push(fileInfo);
                }
   
                //Create tmp\Mini
                Directory.CreateDirectory(wDir.FullName + @"\" + "Mini");

                //Populate tmp\Mini
                foreach (FileInfo fileInfo in wDir.GetFiles())
                {
            		if (Regex.IsMatch(fileInfo.Extension, @".+(jpg)$|.+(jpeg)", RegexOptions.IgnoreCase))
            		{
	                    Image Resized = PicProcessor.ResizeToFixedSizeAndRotate(
	                                                fileInfo.FullName,
	                                                _GData.MiniPicWidth, _GData.MiniPicHeight, ref FileDate);
	
	                    Resized.Save(wDir.FullName + @"\" + "Mini" + @"\" + fileInfo.Name, ImageFormat.Jpeg);
	
	                    FileInfo fi = new FileInfo(wDir.FullName + @"\" + "Mini" + @"\" + fileInfo.Name);
	                    fi.CreationTime = FileDate;
	                    
	                    Resized.Dispose();
	                    Resized = null;
	                    GC.Collect();
	                    PerformGUIStep();
            		}
                }
				
                 //Create the original pictures archives
                 if(_GData.ZipArchive == true)
                 {
                 	ChangeGUIStatusMessage(StaticTools.AppStringRManager.GetString("MakingZip"));
					ChangeGUIProgressBar(stOriginalImages.Count);
                 	ChangeGUICursor(Cursors.WaitCursor);
                 	
                 	ZipManager.CreateZip(wDir.FullName + @"\GenuinePictures.zip", stOriginalImages);
                 	ZipManager.PerformStepHandler += new GagPicturesPerformStepHandler(PerformGUIStep);
                 	
                 	ChangeGUICursor(Cursors.Default);
                 	
                 }
                //Create a template html page
                if((_GData.htmltemplate == "") || (_GData.htmltemplate == null))
                	HTMLGenerator.CreateIndex(ref _GData, wDir);
                
                //And fill it
                HTMLGenerator.FillIndex(_GData, wDir);

                string stNewGalleryURL = _GData.httpServer + @"/" +  _GData.ftproot + wDir.Name + @"/";

                //Create .htaccess and .htpasswd if necessary
                if(InitFiles.IsHtAccessOK(_GData))
                {
                	System.IO.Stream sr = System.IO.File.OpenWrite(wDir.FullName + @"\.htaccess");
                	System.IO.StreamWriter myFile = new System.IO.StreamWriter(sr);
                	
                	myFile.WriteLine("PerlSetVar AuthFile /" + wDir.Name + "/.htpasswd");
                	myFile.WriteLine(@"AuthName ""Restricted Area""");
                	myFile.WriteLine("AuthType Basic");
                	myFile.WriteLine("require valid-user");
                	
                	myFile.Close();
                	
                	sr = System.IO.File.OpenWrite(wDir.FullName + @"\.htpasswd");
                	myFile = new System.IO.StreamWriter(sr);
                	
                	myFile.WriteLine(_GData.htUser + ":" + _GData.htPassword);

                	myFile.Close();
                }

                //FTP
                if(InitFiles.IsFTPOK(_GData))
                {
	                ChangeGUIStatusMessage(StaticTools.AppStringRManager.GetString("ProgressBarStep2Title"));
	                ChangeGUIProgressBar(currentDir.GetFiles().Length * 2);
	                PerformGUIStep();
	                
	                //ftp manager event routing on our own events
	                FtpMgr.SendMessageHandler += new GagPicturesMessageHandler(ChangeGUIStatusMessage);
					FtpMgr.PerformStepHandler += new GagPicturesPerformStepHandler(PerformGUIStep);
	                
	                FtpMgr.PushFTPDir(wDir, _GData.ftproot, _GData);
	                
	                //Mail
	                if(InitFiles.IsMailOK(_GData))
	                {
		                //Mail results
		                //Update the mail body with the URL
		                if(_GData.MailText.Contains("%URL%"))
		                	_GData.MailText = _GData.MailText.Replace("%URL%", stNewGalleryURL.Replace(" ","%20"));
		                else
		                	_GData.MailText += "\n\t" + stNewGalleryURL;
		                
		                //Update the mail body with login/password if necessary
		                if(InitFiles.IsHtAccessOK(_GData))
		                {
		                	if(_GData.MailText.Contains("%LOGIN%"))
                                _GData.MailText = _GData.MailText.Replace("%LOGIN%", _GData.htUser);
		                	else
		                		_GData.MailText += "\n\n\t" + StaticTools.AppStringRManager.GetString("htLogin") + _GData.htUser;
		                	
		                	if(_GData.MailText.Contains("%PASSWORD%"))
		                		_GData.MailText = _GData.MailText.Replace("%PASSWORD%", _GData.htPassword);
		                	else
		                		_GData.MailText += "\n\t" + StaticTools.AppStringRManager.GetString("htPassword") + _GData.htPassword;
		                }
		                MailMan.SendMail(_GData);
	                }
	        
	                //Tidy our mess, if the directory was auto-chosen
    	            if(_GData.OutputPath == (Environment.GetEnvironmentVariable("TEMP") + @"\" + currentDir.Name))
        	        	wDir.Delete(true);
                }
        		else
        		{
        			System.Diagnostics.Process.Start(_GData.OutputPath);
        		}
        		
        		endGUIProc();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
    }
}
