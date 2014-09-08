/*
 * Created vincent.tollu@gmail.com
 * IDE: SharpDevelop
 * Distributed under the GPL
 * Date: 29/05/2007
 */

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;
using System.IO;
using System.Collections.Generic;
using System;

namespace Gag
{
	/// <summary>
    /// Uses Sharpziplib so as to create a non flat zip archive
    /// </summary>
    public abstract class ZipManager
    {
    	public static event GagPicturesPerformStepHandler PerformStepHandler; 
    	
    	private static void PerformGUIStep()
    	{
    		if(PerformStepHandler != null)
    			PerformStepHandler();
    	}
    	
        /// <summary>
        /// Main method
        /// </summary>
        /// <param name="stZipPath">path of the archive wanted</param>
        /// <param name="stDirToZip">path of the directory we want to create, without ending backslash</param>
        public static void CreateZip(string stZipPath, Stack<FileInfo> stackFiles)
        {
            try
            {
				//Sanitize inputs
                stZipPath = Path.GetFullPath(stZipPath);

                ZipOutputStream zipOutput = null;

                if (File.Exists(stZipPath))
                    File.Delete(stZipPath);

                Crc32 crc = new Crc32();
                zipOutput = new ZipOutputStream(File.Create(stZipPath));
                zipOutput.SetLevel(6); // 0 - store only to 9 - means best compression

                foreach (FileInfo fi in stackFiles)
                {
                    PerformGUIStep();
                    FileStream fs = File.OpenRead(fi.FullName);

                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);

                    ZipEntry entry = new ZipEntry(fi.Name);

                    entry.DateTime = DateTime.Now;

                    // set Size and the crc, because the information
                    // about the size and crc should be stored in the header
                    // if it is not set it is automatically written in the footer.
                    // (in this case size == crc == -1 in the header)
                    // Some ZIP programs have problems with zip files that don't store
                    // the size and crc in the header.
                    entry.Size = fs.Length;
                    fs.Close();

                    crc.Reset();
                    crc.Update(buffer);

                    entry.Crc = crc.Value;

                    zipOutput.PutNextEntry(entry);

                    zipOutput.Write(buffer, 0, buffer.Length);
           }
                zipOutput.Finish();
                zipOutput.Close();
                zipOutput = null;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
