using GSM_Designer.Pages;
using GSM_Designer.ViewModel;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for ImageCrop.xaml
    /// </summary>
    public partial class ImageCrop : CustomWindow, iImageWidgetController
    {
        private FileCroppingVM fileCroppingVM;
        private ImageOutput outputWindow;
        private bool canMoveCropBox = false;
        public ImageCrop()
        {
            fileCroppingVM = FileCroppingVM.Instance;
            InitializeComponent();
            this.DataContext = fileCroppingVM;
            outputWindow = new ImageOutput();
        }

        protected override void OnStateChanged(EventArgs obj)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                outputWindow.WindowState = WindowState.Normal;
            }
            else
            {
                outputWindow.WindowState = this.WindowState;
            }

            base.OnStateChanged(obj);
        }

        public override void Navigated(object payload)
        {
            InitializeVM();
            base.Navigated(payload);
        }

        public override void NavigateAway()
        {
            outputWindow.Close();
            outputWindow = null;
            fileCroppingVM = null;
            FileCroppingVM.Instance.Dispose();
            base.NavigateAway();

        }

        private void InitializeVM()
        {
            var images = InfoViewModel.Instance.Files;
            fileCroppingVM.RegisterController(this);
            fileCroppingVM.LoadFiles(images);
        }

        public void SetSource(object bitmapImage, int containerIndex)
        {
            this.ProgressBar.Value = containerIndex + 1;
            var imageSource = bitmapImage as ImageSource;
            switch (containerIndex)
            {
                case 0:
                    this.ImageContainer.Source = imageSource;
                    var source = (bitmapImage as BitmapFrame);
                    var height = fileCroppingVM.Height * source.DpiX;
                    var width = fileCroppingVM.Width * source.DpiY;
                    this.ImageContainer.Height = height;
                    this.ImageContainer.Width = width;
                    CompletArea.Rect = new Rect(0, 0, width, height);
                    InteractionArea.Rect = new Rect(0, 0, fileCroppingVM.CroppedWidth * source.DpiX, fileCroppingVM.CroppedHeight * source.DpiY);
                    break;
                case 1:
                    this.BCrop.Source = imageSource;
                    break;
                case 2:
                    this.CCrop.Source = imageSource;
                    break;
                case 3:
                    this.DCrop.Source = imageSource;
                    break;
                case 4:
                    this.ECrop.Source = imageSource;
                    break;
            }
        }

        private void SizeButton_Click(object sender, RoutedEventArgs e)
        {
            PatternNameWindow c = new PatternNameWindow(this.DataContext);
            c.ShowDialog();
            this.ProgressBar.Maximum = 4;
        }

        private void path_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            canMoveCropBox = false;
        }

        private void path_MouseMove(object sender, MouseEventArgs e)
        {
            if (canMoveCropBox)
            {
                var pos = e.GetPosition(path);
                var locX = pos.X - InteractionArea.Rect.Width / 2;
                var locY = pos.Y - InteractionArea.Rect.Height / 2;
                var boundryX = InteractionArea.Rect.Width + pos.X;
                var boundryY = InteractionArea.Rect.Height + pos.Y;
                var x = pos.X;
                var y = pos.Y;
                if (boundryX > CompletArea.Rect.Width)
                {
                    x = CompletArea.Rect.Width - InteractionArea.Rect.Width;
                }
                else if (x < 0)
                {
                    x = 0;
                }
                if (boundryY > CompletArea.Rect.Height)
                {
                    y = CompletArea.Rect.Height - InteractionArea.Rect.Height;
                }
                else if (y < 0)
                {
                    y = 0;
                }
                InteractionArea.Rect = new Rect(x, y, InteractionArea.Rect.Width, InteractionArea.Rect.Height);
            }
        }

        private void path_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            canMoveCropBox = true;
        }

        private void CropButton_Click(object sender, RoutedEventArgs e)
        {
            fileCroppingVM.CropSelectedAreaInSecondaryImages(InteractionArea.Rect.X, InteractionArea.Rect.Y);
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            MakeLayout();
        }

        public void UpdateUI(bool isProcessing)
        {
            OpacityRect.Visibility = isProcessing ? Visibility.Visible : Visibility.Collapsed;
            ProgressBar.Visibility = isProcessing ? Visibility.Visible : Visibility.Collapsed;
            CropButton.IsEnabled = ApplyButton.IsEnabled = SizeButton.IsEnabled = !isProcessing;
            ProgressBar.Value = 0;
        }

        public void Reset()
        {
            this.ProgressBar.Value = 0;
            this.CompletArea.Rect = new Rect(0, 0, 0, 0);
            this.ImageContainer.Source = null;
            this.BCrop.Source = null;
            this.CCrop.Source = null;
            this.DCrop.Source = null;
            this.ECrop.Source = null;
        }

        private void MakeLayout()
        {
            iLayoutUpdater layoutUpdater = outputWindow as iLayoutUpdater;
            layoutUpdater.ShowWindow();
            layoutUpdater.UpdateOutputLayout("Jay.jpg");
        }

    }
}
