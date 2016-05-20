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
    public partial class ImageOutput : CustomWindow
    {
        public bool IsClosed { get; set; } = true;
        private FileCroppingVM filecroppingVM;
        public ImageOutput() : base(true)
        {
            InitializeComponent();
            filecroppingVM = FileCroppingVM.Instance;
            this.Title = filecroppingVM.PatternName;
            this.KeyDown += ImageOutput_KeyDown;
            var width = System.Windows.SystemParameters.PrimaryScreenWidth;
            var height = System.Windows.SystemParameters.PrimaryScreenHeight;

            this.Height = Math.Min(height - 200, 800);
            this.Width = Height * 0.6;
        }


        private void ImageOutput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Hide();
                e.Handled = true;
            }
        }

        protected override void Navigate(object payload, bool isBacknav = false)
        {
            IsClosed = false;
            base.Navigate(payload, isBacknav);
        }

        protected override void NavigateAway()
        {
            IsClosed = true;
            base.NavigateAway();
        }
        public async Task UpdateOutputLayout()
        {
            AlternateText.Visibility = Visibility.Visible;
            OutputImageView.Visibility = Visibility.Collapsed;
            this.Activate();
            SaveButton.IsEnabled = false;
            await App.TaskQueue.ExecuteTaskAsync(() =>
            {
                FileCroppingVM.Instance.CombinePattern();
            }, new TPL.TaskParams(TPL.Priority.High));
            using (FileStream stream = File.OpenRead(FileCroppingVM.PathPrefix + "output" + ImageHelper.TIFFIMAGEEXTENSION))
            {
                var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                var decodedImage = decoder.Frames[0];
                decoder = null;
                this.OutputImageView.Source = decodedImage;
            }
            SaveButton.IsEnabled = true;
            AlternateText.Visibility = Visibility.Collapsed;
            OutputImageView.Visibility = Visibility.Visible;
            this.Activate();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFile();
        }

        private void SaveFile()
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.FileName = "output.tif";
            saveFileDialog.Filter = ImageHelper.TIFFileFilter + "|" + ImageHelper.JPEGFileFileter;
            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                var extension = filecroppingVM.ImageFormatList[saveFileDialog.FilterIndex - 1].Extension;
                var bitmapEncoder = ImageHelper.GetEncoder();
                bitmapEncoder.Frames.Add(BitmapFrame.Create(this.OutputImageView.Source as BitmapSource));
                var saveFileName = fileName.EndsWith(extension) ? fileName :
                    fileName + extension;
                using (Stream stream = File.Create(saveFileName))
                {
                    bitmapEncoder.Save(stream);
                    bitmapEncoder = null;
                }
            }
        }
    }
}
