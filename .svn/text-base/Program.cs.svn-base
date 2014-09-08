/*
 * Created vincent.tollu@gmail.com
 * IDE: SharpDevelop
 * Distributed under the GPL
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using System.Globalization;
using System.Resources;

namespace Gag
{
	public abstract class StaticTools
    {
		static public ResourceManager AppStringRManager = new ResourceManager("Gag.StringResources", Assembly.GetExecutingAssembly() );
            	
		static public string GetExecutingDir()
		{
			System.Reflection.Assembly currentAssembly = System.Reflection.Assembly.GetAssembly(typeof(Program));
            FileInfo assemblyFileInfo = new FileInfo(currentAssembly.Location);
            
            return assemblyFileInfo.DirectoryName;
		}
		static public bool IsFrench()
		{
			if(Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "fr")
				return true;
			else
				return false;
		}
		
		static public string GetVersion()
		{
			Version ownversion = Assembly.GetExecutingAssembly().GetName().Version;
			return " v. " + ownversion.Major + "." + ownversion.Minor;
		}
		
		static public void ShowMessage(string stMessage)
		{
			MessageBox.Show(stMessage, "GAG -" + GetVersion());
		}
		
		static public DialogResult ShowDialog(string stMessage)
		{
			return MessageBox.Show(stMessage, "GAG -" + GetVersion(), MessageBoxButtons.OKCancel);
		}
	}
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            GagData gData = new GagData();
            MainForm myForm = new MainForm();
            if(InitFiles.ComsumeInitFile(ref gData) == true)
                myForm.Data = gData;

            myForm.ShowDialog();
        }
    }
    
}
