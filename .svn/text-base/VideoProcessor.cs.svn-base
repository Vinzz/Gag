/*
 * Created vincent.tollu@gmail.com
 * IDE: SharpDevelop
 * Distributed under the GPL
 * Date: 19/02/2009
 */

using System;
using System.IO;
using System.Diagnostics;

namespace Gag
{
	/// <summary>
	/// Description of VideoProcessor.
	/// </summary>
	public abstract class VideoProcessor
	{
		public static void Convert2Theora(string stVidPath, string stOutPath, int x, int y)
		{
			try
			{
			Process proc = new System.Diagnostics.Process();
			proc.EnableRaisingEvents = false;
			proc.StartInfo.FileName = @"ffmpeg2theora.exe";
			//proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			
			proc.StartInfo.Arguments = "-o " + @"""" + stOutPath + @"\" + Path.GetFileNameWithoutExtension(stVidPath) + @".ogv"" --title=""0"" -c 1 --no-skeleton -x " + x  +" -y " + y  + @" """ + stVidPath +  @"""";
			
			proc.Start();
			
			proc.WaitForExit();
			}
			catch(Exception)
			{
				throw;
			}
		}
	}
}
