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

        public async Task UpdateOutputLayout(string patternName)
        {
            AlternateText.Visibility = Visibility.Visible;
            OutputImageView.Visibility = Visibility.Collapsed;
            this.Activate();
            //// do stuffs here
            MakeLayout();
            AlternateText.Visibility = Visibility.Collapsed;
            OutputImageView.Visibility = Visibility.Visible;

        }


        protected override void OnClosed(EventArgs e)
        {
            this.Close();
            //base.OnClosed(e);
        }

        private void MakeLayout()
        {
            var innerMarginHorizontal = 50;// Math.Min(30, (filecroppingVM.DPIX * 0.2));
            var innerMarginVertical = Math.Min(40, (filecroppingVM.DPIY * 0.2));
            double horizontalPadding = innerMarginHorizontal / 2;
            double verticalPadding = innerMarginVertical / 2;

            DrawingVisual drawingVisual = new DrawingVisual();

            double horizontalIncrement = 0;
            double verticalIncrement = 0;

            var canvasWidth = (filecroppingVM.Width * filecroppingVM.DPIX) + (innerMarginHorizontal * (filecroppingVM.DPIX / ImageHelper.dpiConst));
            var canvasHeight = ((filecroppingVM.Height + (filecroppingVM.CroppedHeight * 2))
                //// 3 margins - top 0.5, bottom 0.5, 1 between image
                * filecroppingVM.DPIY) + ((innerMarginVertical * (filecroppingVM.DPIY / ImageHelper.dpiConst) * 3.5));

            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(new SolidColorBrush(Colors.White), null, new Rect(0, 0, canvasWidth, canvasHeight));
                Typeface patternTypeFace = new Typeface(new FontFamily("Segoe UI)").ToString());
                var culture = CultureInfo.GetCultureInfo("en-In");
                for (int i = 0; i < 5; i++)
                {
                    var imageSource = new BitmapImage();
                    imageSource.BeginInit();
                    imageSource.UriSource = new Uri(FileCroppingVM.PathPrefix + "collage" + i + filecroppingVM.SelectedFormat.Extension, UriKind.RelativeOrAbsolute);
                    imageSource.EndInit();
                    horizontalIncrement = imageSource.Width;
                    verticalIncrement = imageSource.Height;
                    double nextYLocation = 0;
                    double nextXLocation = 0;
                    var xPoint = nextXLocation + imageSource.Width + horizontalPadding;
                    var yPoint = nextYLocation + verticalIncrement + verticalPadding + 10;
                    switch (i)
                    {
                        case 0:
                            FormattedText fA = new FormattedText(filecroppingVM.PatternName + " - A", culture, FlowDirection.LeftToRight, patternTypeFace, 24, Brushes.Black);
                            xPoint -= fA.Width;
                            drawingContext.DrawText(fA, new Point(xPoint, yPoint));
                            nextYLocation += imageSource.Height + innerMarginVertical;
                            break;
                        case 1:
                            FormattedText fB = new FormattedText(filecroppingVM.PatternName + " - B", culture, FlowDirection.LeftToRight, patternTypeFace, 24, Brushes.Black);
                            xPoint -= fB.Width;
                            drawingContext.DrawText(fB, new Point(xPoint, yPoint));
                            nextXLocation += imageSource.Width + innerMarginHorizontal;
                            break;
                        case 2:
                            FormattedText fc = new FormattedText(filecroppingVM.PatternName + " - C", culture, FlowDirection.LeftToRight, patternTypeFace, 24, Brushes.Black);
                            xPoint -= fc.Width;
                            drawingContext.DrawText(fc, new Point(xPoint, yPoint));
                            nextXLocation -= imageSource.Width + innerMarginHorizontal; //// reset so next frame goes below 2nd image
                            nextYLocation += imageSource.Height + innerMarginVertical;
                            break;
                        case 3:
                            FormattedText fD = new FormattedText(filecroppingVM.PatternName + " - D", culture, FlowDirection.LeftToRight, patternTypeFace, 24, Brushes.Black);
                            xPoint -= fD.Width;
                            drawingContext.DrawText(fD, new Point(xPoint, yPoint));
                            nextXLocation += imageSource.Width + innerMarginHorizontal;
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
            using (Stream stream = File.Create(FileCroppingVM.PathPrefix + "output" + filecroppingVM.SelectedFormat.Extension))
            {
                bitmapEncoder.Save(stream);
                bitmapEncoder = null;
                renderTargetBitmap = null;
            }
            using (FileStream stream = File.OpenRead(FileCroppingVM.PathPrefix + "output" + filecroppingVM.SelectedFormat.Extension))
            {
                var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                var decodedImage = decoder.Frames[0];
                decoder = null;
                this.OutputImageView.Source = decodedImage;
            }

        }

    }
}
