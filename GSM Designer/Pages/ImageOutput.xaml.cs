using GSM_Designer.Utils;
using GSM_Designer.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GSM_Designer.Pages
{
    /// <summary>
    /// Interaction logic for ImageLayput.xaml
    /// </summary>
    public partial class ImageOutput : CustomWindow, iLayoutUpdater
    {
        private FileCroppingVM filecroppingVM;
        public ImageOutput()
        {
            InitializeComponent();
            filecroppingVM = FileCroppingVM.Instance;
        }

        public void ShowWindow()
        {
            if (!this.IsActive)
                this.Show();
        }

        public void UpdateOutputLayout(string patternName)
        {
            AlternateText.Visibility = Visibility.Visible;
            this.BringIntoView();
            //// do stuffs here
            MakeLayout();
            AlternateText.Visibility = Visibility.Collapsed;

        }

        private void MakeLayout()
        {
            var paddingXInch = 0.25;
            var paddingYInch = 0.25;

            double horizontalPadding = (filecroppingVM.DPIX * paddingXInch) / 2;
            double verticalPadding = filecroppingVM.DPIY * paddingYInch;

            DrawingVisual drawingVisual = new DrawingVisual();

            double horizontalIncrement = 0;
            double verticalIncrement = 0;

            var canvasWidth = (filecroppingVM.Width + (paddingXInch * 3)) * filecroppingVM.DPIX;
            var canvasHeight = (filecroppingVM.Height + (filecroppingVM.CroppedHeight * 2) + (paddingYInch * 12)) * filecroppingVM.DPIY;

            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(new SolidColorBrush(Colors.White), null, new Rect(0, 0, canvasWidth, canvasHeight));
                Typeface patternTypeFace = new Typeface(new FontFamily("Segoe UI)").ToString());
                var culture = CultureInfo.GetCultureInfo("en-In");
                for (int i = 0; i < 5; i++)
                {
                    var imageSource = new BitmapImage();
                    imageSource.BeginInit();
                    imageSource.UriSource = new Uri(FileCroppingVM.PathPrefix + "collage" + i + ".jpg", UriKind.RelativeOrAbsolute);
                    imageSource.EndInit();
                    horizontalIncrement = imageSource.Width;
                    verticalIncrement = imageSource.Height;
                    double nextYLocation = 0;
                    double nextXLocation = 0;
                    var xPoint = nextXLocation + imageSource.Width + horizontalPadding;
                    var yPoint = nextYLocation + verticalIncrement + (verticalPadding + 10);
                    switch (i)
                    {
                        case 0:
                            FormattedText fA = new FormattedText(filecroppingVM.PatternName + " - A", culture, FlowDirection.LeftToRight, patternTypeFace, 24, Brushes.Black);
                            xPoint -= fA.Width;
                            drawingContext.DrawText(fA, new Point(xPoint, yPoint));
                            nextYLocation = imageSource.Height + (filecroppingVM.DPIY * paddingYInch);
                            break;
                        case 1:
                            FormattedText fB = new FormattedText(filecroppingVM.PatternName + " - B", culture, FlowDirection.LeftToRight, patternTypeFace, 24, Brushes.Black);
                            xPoint -= fB.Width;
                            drawingContext.DrawText(fB, new Point(xPoint, yPoint));
                            nextXLocation = imageSource.Width + (filecroppingVM.DPIX * paddingXInch / 1.5);
                            break;
                        case 2:
                            FormattedText fc = new FormattedText(filecroppingVM.PatternName + " - C", culture, FlowDirection.LeftToRight, patternTypeFace, 24, Brushes.Black);
                            xPoint -= fc.Width;
                            drawingContext.DrawText(fc, new Point(xPoint, yPoint));
                            nextXLocation -= imageSource.Width + (filecroppingVM.DPIX * paddingXInch / 1.5); //// reset so next frame goes below 2nd image
                            nextYLocation += imageSource.Height + (filecroppingVM.DPIY * paddingYInch);
                            break;
                        case 3:
                            FormattedText fD = new FormattedText(filecroppingVM.PatternName + " - D", culture, FlowDirection.LeftToRight, patternTypeFace, 24, Brushes.Black);
                            xPoint -= fD.Width;
                            drawingContext.DrawText(fD, new Point(xPoint, yPoint));
                            nextXLocation += imageSource.Width + (filecroppingVM.DPIX * paddingXInch / 1.5);
                            break;
                        case 4:
                            FormattedText fE = new FormattedText(filecroppingVM.PatternName + " - E", culture, FlowDirection.LeftToRight, patternTypeFace, 24, Brushes.Black);
                            xPoint -= fE.Width;
                            drawingContext.DrawText(fE, new Point(xPoint, yPoint));
                            break;
                    }
                    drawingContext.DrawImage(imageSource, new Rect(new Point(horizontalPadding, verticalPadding), new Size(horizontalIncrement, verticalIncrement)));
                    //// update the image pointer for next image to draw
                    verticalPadding += nextYLocation;
                    horizontalPadding += nextXLocation;
                }
            }
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)canvasWidth, (int)canvasHeight, filecroppingVM.DPIX, filecroppingVM.DPIY, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(drawingVisual);
            drawingVisual = null;

            var bitmapEncoder = new JpegBitmapEncoder();
            bitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            using (Stream stream = File.Create(FileCroppingVM.PathPrefix + "output.jpg"))
            {
                bitmapEncoder.Save(stream);
                bitmapEncoder = null;
                renderTargetBitmap = null;
            }
        }

    }
}
