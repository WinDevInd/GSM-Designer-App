using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GSM_Designer.Utils
{
    public class ImageHelper
    {
        public static double dpiConst = 96; //// defaultDpiConstant

        public static BitmapImage GetResizedBitmapImage(BitmapImage source, Size size)
        {
            try
            {
                if (size != null && source != null)
                {
                    var bitmapEncoder = new JpegBitmapEncoder();
                    bitmapEncoder.Frames.Add(BitmapFrame.Create(ProcessImageRepeeatXY(source, size)));
                    var finalImage = new BitmapImage();
                    using (var stream = new MemoryStream())
                    {
                        bitmapEncoder.Save(stream);
                        stream.Seek(0, SeekOrigin.Begin);
                        finalImage.BeginInit();
                        finalImage.CacheOption = BitmapCacheOption.OnLoad;
                        finalImage.StreamSource = stream;
                        finalImage.EndInit();
                    }
                    bitmapEncoder = null;
                    return finalImage;
                }
            }
            catch { }
            return null;
        }

        public static async Task<bool> SaveResizedBitmapImage(BitmapImage source, Size size, string desitnationPath = "file.jpg")
        {
            bool isTaskSuccess = false;
            try
            {
                if (!string.IsNullOrWhiteSpace(desitnationPath) && size != null && source != null)
                {
                    var bitmapEncoder = new JpegBitmapEncoder();
                    var targetBitmap = ProcessImageRepeeatXY(source, size);
                    bitmapEncoder.Frames.Add(BitmapFrame.Create(targetBitmap));
                    using (Stream stream = File.Create(desitnationPath))
                    {
                        bitmapEncoder.Save(stream);
                        bitmapEncoder = null;
                        targetBitmap = null;
                    }
                    isTaskSuccess = true;
                }
            }
            catch
            {
                isTaskSuccess = false;
            }
            return isTaskSuccess;
        }

        public static BitmapSource ProcessCroping(BitmapImage source, Size size, Point location)
        {
            int width = (int)size.Width;
            int height = (int)size.Height;
            int x = (int)location.X;
            int y = (int)location.Y;
            CroppedBitmap img = new CroppedBitmap(source, new Int32Rect(x, y, width, height));
            return img;
        }

        public static RenderTargetBitmap ProcessImageRepeeatXY(BitmapImage source, Size size)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            var dpiX = source.DpiX;
            var dpiY = source.DpiY;
            int destinationImageWidth = (int)(size.Width);
            int destiantionImageHeight = (int)(size.Height);
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                int XIncrment = (int)Math.Min(destinationImageWidth, source.Width);
                int YIncrement = (int)Math.Min(destiantionImageHeight, source.Height);
                for (int startPointY = 0; startPointY < destiantionImageHeight; startPointY += Math.Min(YIncrement, destiantionImageHeight - startPointY))
                {
                    for (int startPointX = 0; startPointX < destinationImageWidth; startPointX += Math.Min(XIncrment, destinationImageWidth - startPointX))
                    {
                        drawingContext.DrawImage(source, new Rect(startPointX, startPointY, XIncrment, YIncrement));
                    }
                }
            }
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(destinationImageWidth, destiantionImageHeight, dpiX, dpiY, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(drawingVisual);
            drawingVisual = null;
            return renderTargetBitmap;
        }
    }
}
