﻿using GSM_Designer.AppNavigationService;
using GSM_Designer.ViewModel;
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace GSM_Designer.Pages
{
    /// <summary>
    /// Interaction logic for PatterName.xaml
    /// </summary>
    public partial class PatternNameWindow : CustomWindow
    {
        private FileCroppingVM fileCroppingVM;
        private bool isDialog = false;

        public PatternNameWindow()
        {
            InitializeComponent();
        }

        protected override void Navigate(object payload, bool isBackNav = false)
        {
            if (payload != null)
            {
                fileCroppingVM = payload as FileCroppingVM;
            }
            else
            {
                Intialize();
            }
            this.DataContext = fileCroppingVM;
            var file = InfoViewModel.Instance.Files[0].FilePath;
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(file, UriKind.RelativeOrAbsolute);
            bitmapImage.EndInit();
            fileCroppingVM.DPIX = bitmapImage.DpiX;
            fileCroppingVM.DPIY = bitmapImage.DpiY;
            base.Navigate(payload, isBackNav);
        }

        private void Intialize()
        {
            fileCroppingVM = FileCroppingVM.Instance;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            NextButton.IsEnabled = (sender as TextBox)?.Text.Length > 0;
        }

        private void NextButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (fileCroppingVM.Width != 0 || fileCroppingVM.Height != 0 || fileCroppingVM.CroppedWidth != 0 || fileCroppingVM.CroppedHeight != 0)
            {
                if (!isDialog)
                {
                    NavigationParam navParam = new NavigationParam()
                    {
                        PageType = PageType.ImageCropping,
                        WindowType = WindowsType.WindowPage,
                        RemoveOnAway = false
                    };
                    CustomNavigationService.GetNavigationService().Navigate(this, navParam);
                    return;
                }
                fileCroppingVM.ApplySize(true);
                this.Close();

            }
        }

        private void SizeControl_PreviewTextChanged(object sender, TextChangedEventArgs e)
        {
            var text = (sender as TextBox).Text;
            NextButton.IsEnabled = !string.IsNullOrWhiteSpace(PatternName.Text) && !string.IsNullOrWhiteSpace(text) && text != "0";
            if (!string.IsNullOrWhiteSpace(text))
            {
                switch (e.Source.ToString())
                {
                    case "Width":
                        fileCroppingVM.Width = double.Parse(text);
                        break;
                    case "Height":
                        fileCroppingVM.Height = double.Parse(text);
                        break;
                }
            }
        }
    }
}
