/*
 * Created vincent.tollu@gmail.com
 * IDE: SharpDevelop
 * Distributed under the GPL
 */
 
using System.IO;
using System.Windows.Forms;
using System.Net;
using System;


namespace Gag
{
    public abstract class FtpMgr
    {
    	public static event GagPicturesPerformStepHandler PerformStepHandler; 
    	public static event GagPicturesMessageHandler SendMessageHandler; 
    	
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
    	
    	public static void PushFTPDir(DirectoryInfo dir, string prevPath, GagData gData)
        {
    		int ntry=0;
	        string stFTPPath = FtpMgr.MakeDir(prevPath + dir.Name, gData, ref ntry);
					
            //Stubborn mode
            if(stFTPPath == "")
            	stFTPPath = FtpMgr.MakeDir(prevPath + dir.Name, gData, ref ntry);
	                
            foreach(FileInfo fileInfo in dir.GetFiles())
        	{
    		    ntry=0;
	        	//Stubborn mode
	        	if(FtpMgr.PushFile(fileInfo.FullName, stFTPPath, gData, ref ntry) == false)
	        		FtpMgr.PushFile(fileInfo.FullName, stFTPPath, gData, ref ntry);
	        	
	        	PerformGUIStep();
        	}
            foreach (DirectoryInfo diNext in dir.GetDirectories())
        		PushFTPDir(diNext, prevPath + dir.Name + "/", gData);
        }
    	
        public static bool PushFile(String filename, String FTPDirName, GagData gData, ref int nTry)
        {
        	nTry++;
            FileInfo fileInf = new FileInfo(filename);
            FtpWebRequest reqFTP;

            // Create FtpWebRequest object from the Uri provided
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(FTPDirName + "/" + fileInf.Name));

            // Provide the WebPermission Credintials
            reqFTP.Credentials = new NetworkCredential(gData.ftpUser, gData.clearftpPassword);

            // By default KeepAlive is true, where the control connection is not closed
            // after a command is executed.
            reqFTP.KeepAlive = false;

            // Specify the command to be executed.
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;

            // Specify the data transfer type.
            reqFTP.UseBinary = true;

            // Notify the server about the size of the uploaded file
            reqFTP.ContentLength = fileInf.Length;

            // The buffer size is set to 2kb
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;

            // Opens a file stream (System.IO.FileStream) to read the file to be uploaded
            FileStream fs = fileInf.OpenRead();
			
            #region progress tracker
            long NumSteps = (long) ((fs.Length / buffLength) / 100);
            
            int percent = 0;

            string stUploadLabel = "Upload :" + Path.GetFileName(filename) + " (" + fs.Length / (1024) + " ko) ";
			
            ChangeGUIStatusMessage(stUploadLabel + percent + "%");
            
            #endregion
            
            try
            {
                // Stream to which the file to be upload is written
                Stream strm = reqFTP.GetRequestStream();

                // Read from the file stream 2kb at a time
                contentLen = fs.Read(buff, 0, buffLength);
				int i=0;
				
                // Till Stream content ends
                while (contentLen != 0)
                {
                    // Write Content from the file stream to the FTP Upload Stream
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                    
                    #region progress tracker
                    if(NumSteps > 0)
                    {
	                    if((i % NumSteps) ==0)
	                              ++percent;
	                	++i;
	                	
	                	//Math 101 :o)
	                	if(percent > 100 ) percent = 100;
	                	
	                	ChangeGUIStatusMessage(stUploadLabel + percent + "%");
                    }
                    #endregion
                }

                // Close the file stream and the Request Stream
                strm.Close();
                fs.Close();
                return true;
            }
            catch (Exception ex)
            {
            	if(nTry > 1)
            		MessageBox.Show(ex.Message, "FTP Upload Error. File " + filename + " won't be uploaded");
            	
                return false;
            }
        }

        public static string MakeDir(string dirName, GagData gData, ref int nTry)
        {
            FtpWebRequest reqFTP;
            try
            {
            	nTry++;
                // dirName = name of the directory to create.
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + gData.ftpServer + "/" + dirName));
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(gData.ftpUser, gData.clearftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                ftpStream.Close();
                response.Close();

                return "ftp://" + gData.ftpServer + "/" + dirName;
            }
            catch (Exception ex)
            {
                if(nTry > 1)
            		MessageBox.Show(ex.Message, "FTP CreateDir Error. Directory " + dirName + " won't be created");
                
                return "";
            }
        }
    }
}
