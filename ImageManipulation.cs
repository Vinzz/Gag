/*
 * Created vincent.tollu@gmail.com
 * IDE: SharpDevelop
 * Distributed under the GPL
 */

using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;
using System;

namespace Gag
{
    public abstract class PicProcessor
    {
        public static Image ResizeToFixedSizeAndRotate(string stPhotoPath, int Width, int Height,
    	                                              ref DateTime picDate)
        {
            try
            {
            	Image imgPhoto = Image.FromFile(stPhotoPath); 
                //Check the orientation of the photo
                int myOrientation = 0;
                PropertyItem propItem = null;

                try
                {
                    propItem = imgPhoto.GetPropertyItem((int)0x0112 /*orientation flag*/);
                    myOrientation = BitConverter.ToUInt16(propItem.Value, 0);

                    picDate = new FileInfo(stPhotoPath).LastWriteTime;
                }
                catch (Exception)
                {
                    //No EXIF Data, no need to cry, though
                }

                ///1       2       3       4       5           6           7           8
                ///
                ///888888  888888      88  88      8888888888  88                  88  8888888888
                ///88          88      88  88      88  88      88  88          88  88      88  88
                ///8888      8888    8888  8888    88          8888888888  8888888888          88
                ///88          88      88  88
                ///88          88  888888  888888
                switch (myOrientation)
                {
                    case 1:
                        //Nothin to do
                        break;
                    case 2:
                        //Horizontal flip
                        imgPhoto.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        break;
                    case 3:
                        //Rotate 180
                        imgPhoto.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    case 4:
                        //Rotate 180 + Horizontal flip
                        imgPhoto.RotateFlip(RotateFlipType.Rotate180FlipX);
                        break;
                    case 5:
                        //Rotate 90 + Horizontal flip
                        imgPhoto.RotateFlip(RotateFlipType.Rotate90FlipX);
                        break;
                    case 6:
                        //Rotate 90
                        imgPhoto.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case 7:
                        //Rotate 90 Vertical flip
                        imgPhoto.RotateFlip(RotateFlipType.Rotate90FlipY);
                        break;
                    case 8:
                        //Rotate 270
                        imgPhoto.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                    default:
                        break;
                }

                FileInfo fi = new FileInfo(stPhotoPath);
                  
                int sourceWidth = imgPhoto.Width;
                int sourceHeight = imgPhoto.Height;

                //Consider vertical pics
                if (sourceWidth < sourceHeight)
                {
                    int buff = Width;

                    Width = Height;
                    Height = buff;
                }

                int sourceX = 0, sourceY = 0, destX = 0, destY = 0;
                float nPercent = 0, nPercentW = 0, nPercentH = 0;

                nPercentW = ((float)Width / (float)sourceWidth);
                nPercentH = ((float)Height / (float)sourceHeight);
                if (nPercentH < nPercentW)
                {
                    nPercent = nPercentH;
                    destX = System.Convert.ToInt16((Width -
                                  (sourceWidth * nPercent)) / 2);
                }
                else
                {
                    nPercent = nPercentW;
                    destY = System.Convert.ToInt16((Height -
                                  (sourceHeight * nPercent)) / 2);
                }

                int destWidth = (int)(sourceWidth * nPercent);
                int destHeight = (int)(sourceHeight * nPercent);

                Bitmap bmPhoto = new Bitmap(Width, Height,
                                  PixelFormat.Format24bppRgb);
                bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                                 imgPhoto.VerticalResolution);

                Graphics grPhoto = Graphics.FromImage(bmPhoto);
                grPhoto.Clear(Color.Black);
                grPhoto.InterpolationMode =
                        InterpolationMode.HighQualityBicubic;

                grPhoto.DrawImage(imgPhoto,
                    new Rectangle(destX, destY, destWidth, destHeight),
                    new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                    GraphicsUnit.Pixel);

                grPhoto.Dispose();
                return bmPhoto;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Image Manipulation error");
                return null;
            }
        }
    }
}
