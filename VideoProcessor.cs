/*
 * Created vincent.tollu@gmail.com
 * IDE: SharpDevelop
 * Distributed under the GPL
 * Date: 19/02/2009
 */

using System;
using System.IO;
using System.Diagnostics;
using NReco.VideoConverter;

namespace Gag
{
    /// <summary>
    /// Description of VideoProcessor.
    /// </summary>
    public abstract class VideoProcessor
    {
        private static string currentVideo = string.Empty;
        private static int step = 0;

        public static event GagPicturesMessageHandler SendMessageHandler; 

        private static void ChangeGUIStatusMessage(string stMessage)
        {
            if (SendMessageHandler != null)
                SendMessageHandler(stMessage);
        }

        public static void Convert2Html5Formats(string stVidPath, string outPath, int x, int y)
        {
            var ffMpeg = new NReco.VideoConverter.FFMpegConverter();

            try
            {
                ffMpeg.ConvertProgress += ffMpeg_ConvertProgress;

                ConvertSettings settings = new ConvertSettings();
                settings.SetVideoFrameSize(x, y);

                string videoPath = Path.Combine(outPath, Path.GetFileNameWithoutExtension(stVidPath));

                string webMVideoPath = videoPath + ".webm";
                currentVideo = Path.GetFileName(webMVideoPath);
                step = 1;
                ffMpeg.ConvertMedia(stVidPath, Path.GetExtension(stVidPath).TrimStart('.'), webMVideoPath, Format.webm, settings);

                string ogvVideoPath = videoPath + ".ogv";
                currentVideo = Path.GetFileName(ogvVideoPath);
                step = 2;
                ffMpeg.ConvertMedia(stVidPath, Path.GetExtension(stVidPath).TrimStart('.'), ogvVideoPath, Format.ogg, settings);

                string mp4VideoPath = videoPath + ".mp4";
                currentVideo = Path.GetFileName(mp4VideoPath);
                step = 3;
                ffMpeg.ConvertMedia(stVidPath, Path.GetExtension(stVidPath).TrimStart('.'), mp4VideoPath, Format.mp4, settings);
            }
            finally 
            {
                ffMpeg.Abort();
                ffMpeg = null;
            }
        }

        static void ffMpeg_ConvertProgress(object sender, ConvertProgressEventArgs e)
        {
            double conversion = (e.Processed.TotalSeconds / e.TotalDuration.TotalSeconds) * 100;
            ChangeGUIStatusMessage(string.Format(Properties.Resources.ProcessVideo, currentVideo, (int)conversion, step, (int) (e.TotalDuration.TotalSeconds - e.Processed.TotalSeconds)));
        }
    }
}
