/*
 * Created vincent.tollu@gmail.com
 * IDE: SharpDevelop
 * Distributed under the GPL
 */

using System;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Resources;
using System.Reflection;
using Gag.Properties;

namespace Gag
{
    /// <summary>
    /// Precious init datas
    /// Read/Write of the strings is dynamic (wow ;o)
    /// In fact, my oh-so-clever reflection scheme for dumping my object did exist.
    /// It's called serialization, and it's darn handy. 
    /// We'll now use a serializer instead of the reflection
    /// </summary>
    public class GagData
    {
        public string InputPath ="";
		public string OutputPath ="";
		
        public string ftpServer="";
        public string ftpUser="";
        
        private string _ftpPassword="";
        
        //Crypted password, used for I/O
        public string ftpPassword
        {
        	set{
				_ftpPassword = value;
        	}
        	get{
        		return _ftpPassword;
        	}
        }
        //clear password, used trhough the program
        [XmlIgnore]
        public string clearftpPassword
         {
         	get{
         		if(_ftpPassword == "") return "";
         		else
        			return CypherMgr.Decrypt(_ftpPassword,"h-]@!|7C");
        	}
         	set{ 
				_ftpPassword = CypherMgr.Encrypt(value,"h-]@!|7C");
        	}
         }
        
		public string httpServer="";
        public string MailFrom="";
        public string MailServer="";
        public string MailTo="";
        public string MailSubject="";
        public string MailText="";
        
        public string ftproot="";
        
        public string htUser="";
        public string htPassword="";
        
        public int PicWidth= 1700;
        public int PicHeight= 1275;
        public int MiniPicWidth= 450;
        public int MiniPicHeight= 338;
        
        public string htmltemplate="";
        
        public bool LightBox=false;
        public bool ZipArchive=false;
    }
    /// <summary>
    /// Manages the init xml files
    /// </summary>
    public abstract class InitFiles
    {
    	public static bool IsHtAccessOK(GagData gData)
    	{
    		return ( (gData.htUser != "") &&
    		         (gData.htPassword != ""));
    	}
    	public static bool IsFTPOK(GagData gData)
    	{
    		return ( (gData.ftpServer != "") &&
    		         (gData.ftpUser != "") &&
    		         (gData.clearftpPassword != "") );
    	}
    	
    	public static bool IsMailOK(GagData gData)
    	{
    		return ( (gData.MailFrom != "") &&
    		         (gData.MailServer != "") &&
    		         (gData.MailSubject != "") &&
    		         (gData.MailTo != "") );
    	}
    	
        public static bool ComsumeInitFile(ref GagData gData)
        {
            try
            {
                System.Reflection.Assembly currentAssembly = System.Reflection.Assembly.GetAssembly(typeof(Program));
                FileInfo assemblyFileInfo = new FileInfo(currentAssembly.Location);

                return ComsumeInitFile(assemblyFileInfo.DirectoryName + @"\GagInit.xml", ref gData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ComsumeInitFile Error");
                return false;
            }
        }
        public static bool ComsumeInitFile(string stInitFilePath, ref GagData gData)
        {
            try
            {
                if (File.Exists(stInitFilePath))
                {
                	StreamReader sr = new StreamReader(stInitFilePath, true);
					XmlSerializer s = new XmlSerializer(typeof(GagData)); 
					
					gData = (GagData) (s.Deserialize(sr));
					
                    sr.Close();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ComsumeInitFile Error");
                return false;
            }
        }
        public static void CreateInitFile(GagData gData)
        {
            try
            {
            	CreateInitFile(StaticTools.GetExecutingDir() + @"\GagInit.xml", gData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "CreateInitFile Error");
            }
        }
        public static void CreateInitFile(string stInitFilePath, GagData gData)
        {
            try
            {				
                XmlSerializer s = new XmlSerializer(typeof(GagData)); 
 				System.IO.TextWriter xw = new System.IO.StreamWriter(stInitFilePath, false, System.Text.Encoding.UTF8);
 				s.Serialize(xw,gData);
 				xw.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "CreateInitFile Error");
            }
        }
        public static bool AreInputsOK(GagData gData)
        {
        	try
        	{   	
        		if(!CheckMail(gData.MailFrom))
        		{
                    StaticTools.ShowMessage(Resources.BadMail + ": " + gData.MailFrom);
        			return false;
        		}
        		
        		if(!CheckMail(gData.MailTo))
        		{
        			StaticTools.ShowMessage(Resources.BadMail + ": " + gData.MailTo);
        			return false;
        		}
        		
        		if(!CheckURL(gData.ftpServer))
        		{
        			StaticTools.ShowMessage(Resources.BadURL + ": " + gData.ftpServer);
        			return false;
        		}
        		
        		if(!CheckHtmlTemplate(gData.htmltemplate))
        		{
        			StaticTools.ShowMessage(Resources.BadTemplate);
        			return false;
        		}
        		
        		if(!CheckURL(gData.httpServer))
        		{
       			StaticTools.ShowMessage(Resources.BadURL + ": " + gData.httpServer);
        			return false;
        		}
        		
        		//Check if the output path is empty
        		if( (gData.OutputPath != "") && Directory.Exists(gData.OutputPath) )
        		{
        			DirectoryInfo dir = new DirectoryInfo(gData.OutputPath);
        			
        			if( (dir.GetFiles().Length > 0) || (dir.GetDirectories().Length > 0) )
        			{
        				if(StaticTools.ShowDialog(Resources.FullOutDir + ": " + gData.OutputPath) == DialogResult.Cancel)
        					return false;        				
        			}
        		}
        			
        		return true;
        	}
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Check Inputs Error");
                return false;
            }
        }
        
        private static bool CheckMail(string st)
        {
        	if(st == "") return true;
        	
        	string[] adresses = st.Split(',');
        	
        	for(int i=0;i<adresses.Length;++i)
        		if(!Regex.IsMatch(adresses[i],@"^[0-9a-z][-_.0-9\+a-z]*@[0-9a-z][a-z0-9_-]*\.([a-z0-9_-]+\.)*[a-z]{2,}$"))
        			return false;
        	
        	return true;
        }
        
        private static bool CheckHtmlTemplate(string st)
         {
         	return true;
        	
//         	System.IO.Stream sr = System.IO.File.OpenRead(st);
//            System.IO.StreamReader myIndex = new System.IO.StreamReader(sr);
//			
//            if(Regex.IsMatch(myIndex.ReadToEnd(),"<.*span.*class.*pics.*>", RegexOptions.IgnoreCase|RegexOptions.IgnorePatternWhitespace))
//            	return true;
//            else
//            	return false;
         }
        
         private static bool CheckURL(string st)
         {
         	return true;
        	
//        	try
//			{
//				System.Net.IPHostEntry he = System.Net.Dns.GetHostEntry(st);
//				return true;
//			}
//			catch(System.Exception)
//			{
//				return false;
//			}
         }
    }
}
