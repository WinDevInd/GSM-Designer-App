using GSM_Designer.Utils;
using GSM_Designer.ViewModel;
using ImageUtil;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private const string DesignText = "Please wait! Final desing is under processing...";
        private const string SavingText = "Saving Image";
        private const string outputFileName = "output";
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
            AlternateText.Text = DesignText;
            ProgressBar.Visibility = OpacityRect.Visibility = AlternateText.Visibility = Visibility.Visible;
            OutputImageView.Visibility = Visibility.Collapsed;
            this.Activate();
            SaveButton.IsEnabled = false;
            await App.TaskQueue.ExecuteTaskAsync(() =>
            {
                FileCroppingVM.Instance.CombinePattern(outputFileName);
            }, new TPL.TaskParams(TPL.Priority.High));
            using (FileStream stream = File.OpenRead(FileCroppingVM.PathPrefix + outputFileName))
            {
                var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                var decodedImage = decoder.Frames[0];
                decoder = null;
                this.OutputImageView.Source = decodedImage;
            }
            SaveButton.IsEnabled = true;
            ProgressBar.Visibility = OpacityRect.Visibility = AlternateText.Visibility = Visibility.Collapsed;
            OutputImageView.Visibility = Visibility.Visible;
            this.Activate();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar.Visibility = AlternateText.Visibility = OpacityRect.Visibility = Visibility.Visible;
            SaveButton.IsEnabled = false;
            AlternateText.Text = SavingText;
            await App.TaskQueue.ExecuteTaskAsync(() =>
            {
                SaveFile();
            }, new TPL.TaskParams(TPL.Priority.High));
            ProgressBar.Visibility = OpacityRect.Visibility = AlternateText.Visibility = Visibility.Collapsed;
            SaveButton.IsEnabled = true;
        }

        private async Task SaveFile()
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.FileName = "output.tif";
            saveFileDialog.Filter = ImageHelper.TIFFileFilter + "|" + ImageHelper.JPEGFileFileter;
            var extensions = saveFileDialog.Filter.Split('|');
            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                var extension = extensions[(saveFileDialog.FilterIndex * 2) - 1];
                extension = extension.Replace("*", "");
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(FileCroppingVM.PathPrefix + "output", UriKind.RelativeOrAbsolute);
                bitmapImage.EndInit();
                var bitmapEncoder = ImageHelper.GetEncoder(extension);
                bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapImage));
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
