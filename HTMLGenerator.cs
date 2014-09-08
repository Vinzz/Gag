/*
 * Created vincent.tollu@gmail.com
 * IDE: SharpDevelop
 * Distributed under the GPL
 */

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic; 

namespace Gag
{
	public abstract class HTMLGenerator
    {
    	static void DirCopy(string src, string dest) 
		{ 
    		Directory.CreateDirectory(dest);
    		
    		foreach( string fi in Directory.GetFiles(src))
    		        File.Copy(fi,dest + @"\" + Path.GetFileName(fi));
    		
		}
    	
        public static void CreateIndex(ref GagData gData, DirectoryInfo wDir)
        {
            try
            {
                System.IO.Stream sr = System.IO.File.OpenWrite(wDir.FullName + @"\index.html");
                System.IO.StreamWriter myIndex = new System.IO.StreamWriter(sr,System.Text.Encoding.UTF8);

                gData.htmltemplate = wDir.FullName + @"\index.html";
                
                myIndex.WriteLine(@"<!DOCTYPE html PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"">");
                myIndex.WriteLine("<html>");
                myIndex.WriteLine("<head>");
                myIndex.WriteLine("<title>" + wDir.Name + "</title>");
                myIndex.WriteLine(@"<meta content=""text/html; charset=UTF-8"" http-equiv=""content-type"">");
                myIndex.WriteLine(@"<META NAME=""author"" CONTENT=""Vincent Tollu"">");
                myIndex.WriteLine(@"<META NAME=""generator"" CONTENT=""Gag - A simple GAllery Generator" + StaticTools.GetVersion() + @""">" );
                
                myIndex.WriteLine(@"<style type=""text/css"">");
                myIndex.WriteLine(@"a img {border: none;}");
                myIndex.WriteLine(@"#novideo {");
    			myIndex.WriteLine(@"border: solid 1px blue;");
    			myIndex.WriteLine(@"color: White;");
   				myIndex.WriteLine(@"height:" + gData.MiniPicHeight + "px;");
   				myIndex.WriteLine(@"width:" + gData.MiniPicWidth + "px;");
				myIndex.WriteLine(@"}");
                myIndex.WriteLine(@"</style>");

                //LightBox integration
                if(gData.LightBox == true)
                {
                	myIndex.WriteLine(@"<script type=""text/javascript"" src=""js/prototype.js"" ></script>");

                	myIndex.WriteLine(@"<script type=""text/javascript"" src=""js/scriptaculous.js?load=effects"" ></script>");
                	myIndex.WriteLine(@"<script type=""text/javascript"" src=""js/lightbox_s.js"" ></script>");
                	myIndex.WriteLine(@"<script type=""text/javascript"" src=""js/swfobject.js"" ></script>");
	
                	myIndex.WriteLine(@"<link rel=""stylesheet"" href=""css/lightbox.css"" type=""text/css"" media=""screen"" >");
                }
                
                myIndex.WriteLine("</head>");

                myIndex.WriteLine(@"<body style=""background-color:Black; color: White;"">");

                myIndex.WriteLine(@"<big style=""font-weight: bold;""><big><big><big>" + wDir.Name + @"</big></big></big></big>");
                myIndex.WriteLine(@"<div style=""text-align: center;"">");
                myIndex.WriteLine(@"<span class=""pics"">");
                myIndex.WriteLine("</span>"); 
                myIndex.WriteLine("</div>");
                myIndex.WriteLine("</body>");
                myIndex.WriteLine("</html>");
                myIndex.Close();
                
                
            }
            catch (System.Exception ex)
            {
               StaticTools.ShowMessage(ex.Message);
            }
        }
        
        public static void FillIndex(GagData gData, DirectoryInfo wDir)
        {
        	try
        	{	  			
	        	bool bLB = (gData.LightBox == true);
	  			
	        	//Custom template
	        	if(gData.htmltemplate != wDir.FullName + @"\index.html")
	        		File.Copy(gData.htmltemplate, wDir.FullName + @"\index.html");
	        	
	        	System.IO.Stream sr = System.IO.File.OpenRead(wDir.FullName + @"\index.html");
	            System.IO.StreamReader myIndex = new System.IO.StreamReader(sr);
				
	            StringBuilder myBuilder = new StringBuilder("");
	            string currentString = null;
	           	
	            #region LightBox copy
	            if(bLB == true)
	            {
	            	if(Directory.Exists(StaticTools.GetExecutingDir() + @"\lightBox"))
	        	   {
		            	DirCopy(StaticTools.GetExecutingDir() + @"\lightBox\css", wDir.FullName + @"\css");
		            	DirCopy(StaticTools.GetExecutingDir() + @"\lightBox\images", wDir.FullName + @"\images");
		            	DirCopy(StaticTools.GetExecutingDir() + @"\lightBox\js", wDir.FullName + @"\js");
	        	   }
					else
						StaticTools.ShowMessage(@"Can't find lightBox folder");
	            }
	            #endregion
	            
	            #region images
	            while((currentString = myIndex.ReadLine()) != null)
	            {
	            	myBuilder.AppendLine(currentString);
	            	if(currentString.Contains("span") && currentString.Contains("pic"))
	            	{
	            		myBuilder.AppendLine(@"<BR/>");
	            		myBuilder.AppendLine(@"<big style=""font-weight: italic;"">" + StaticTools.AppStringRManager.GetString("AdPrefix") +
	                                  @"<a href=""http://yocto.projects.free.fr/Programs/Gag/Gag.html"">GAG</a>" +
	                                  StaticTools.AppStringRManager.GetString("AdSuffix") + @"</big>");
	            		
						long i = 0;
						bool flag = false;
						
						FileInfo[] fi = wDir.GetFiles();

						// Sorts the FileInfo[] array
						Array.Sort(fi, 
						           delegate(FileInfo f1, FileInfo f2){
    						return f1.CreationTime.CompareTo(f2.CreationTime);});

		                foreach (FileInfo fileInfo in fi)
		                {
		                    if ((i % 4) == 0) myBuilder.AppendLine("<br/><br/>");
		                    #region Images with Lightbox stuff
		                    if (Regex.IsMatch(fileInfo.Extension, @".+(jpg)$|.+(jpeg)", RegexOptions.IgnoreCase))
		                    {
		                    	myBuilder.Append(@"<a href=""" + fileInfo.Name + @"""");
		                    	
		                    	if(bLB == true)
		                    	{
		                    		myBuilder.Append(@" rel=""lightbox[Gag]"" ");
		                    		if(flag == false)
		                    		{
										myBuilder.Append(@" startslideshow= ""false"" forever=""false"" slideDuration=""5""");	
										flag = true;
		                    		}
		                    	}
		                    	
		                    	 myBuilder.Append(@" title=""" + fileInfo.CreationTime.ToLongDateString() + @""" >" +
		                                             @"<img src=""" + "Mini" + @"/" + fileInfo.Name + @""" alt=""" + fileInfo.Name + @""" title=""" + fileInfo.CreationTime.ToLongDateString() + @""" ></a> &nbsp;" + "\n");
		                    }
		                    #endregion
		                    
		                    #region videos
		                    if (Regex.IsMatch(fileInfo.Extension, @".+(ogv)", RegexOptions.IgnoreCase) || Regex.IsMatch(fileInfo.Extension, @".+(avi)", RegexOptions.IgnoreCase))
		                    {
		                    	myBuilder.AppendLine("<br/><br/>");
		                   
								myBuilder.AppendLine(@"<video src=""" + fileInfo.Name + @""" controls='true'>");
								
								myBuilder.AppendLine(@"<div id='novideo'>");
								myBuilder.AppendLine(StaticTools.AppStringRManager.GetString("NoVideoTagSupport"));
							    myBuilder.AppendLine(@"</div>");
								myBuilder.AppendLine(@"</video>");									
		                    	myBuilder.AppendLine("<br/><br/>");
		                    }
		                    #endregion
		                    ++i;
		                }
		                
		                //Add a link to the Zip archive
		                if(gData.ZipArchive == true)
		                {
		                	myBuilder.AppendLine(@"<br/><br/>");
	            			myBuilder.AppendLine(@"<a href=""GenuinePictures.zip"">" + StaticTools.AppStringRManager.GetString("ZipArchive") + "</a>");
		                }
		                
		                if(bLB == true)
	           			{
		                	myBuilder.AppendLine(@"<br/><br/>");
	            			myBuilder.AppendLine(StaticTools.AppStringRManager.GetString("LightBox") + @"<a href=""http://www.huddletogether.com/projects/lightbox2/"">" + "LightBox 2"  + "</a>");
		                }
		             }
	           		}
	            #endregion
	            
	            myIndex.Close();
	            File.Delete(wDir.FullName + @"\index.html");
	            
	            sr = System.IO.File.OpenWrite(wDir.FullName + @"\index.html");
	            System.IO.StreamWriter myWriter = new System.IO.StreamWriter(sr,System.Text.Encoding.UTF8);
				
	            myWriter.Write(myBuilder);            
	            myWriter.Close();
        	}
	        catch (Exception ex)
	        {
	        	 StaticTools.ShowMessage(ex.Message);
	        }
        }
    }
}
