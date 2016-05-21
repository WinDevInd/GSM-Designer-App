using GSM_Designer.AppNavigationService;
using GSM_Designer.Utils;
using GSM_Designer.ViewModel;
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace GSM_Designer.Pages
{
    /// <summary>
    /// Interaction logic for PatterName.xaml
    /// </summary>
    public partial class PatternInfoWindow : CustomWindow
    {
        private FileCroppingVM fileCroppingVM;
        private bool isDialog = false;

        public PatternInfoWindow()
        {
            InitializeComponent();
        }

        protected override void Navigate(object payload, bool isBackNav = false)
        {
            if (payload != null)
            {
                isDialog = true;
                fileCroppingVM = payload as FileCroppingVM;
            }
            else
            {
                isDialog = false;
                fileCroppingVM = FileCroppingVM.Instance;
            }
            this.DataContext = fileCroppingVM;
            base.Navigate(payload, isBackNav);
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
                this.CloseWindow(false);
            }
        }

        private void SizeControl_PreviewTextChanged(object sender, TextChangedEventArgs e)
        {
            var text = (sender as TextBox).Text;
            var data = TextHelper.TextToPositiveDouble(text);
            NextButton.IsEnabled = !string.IsNullOrWhiteSpace(PatternName.Text) && !string.IsNullOrWhiteSpace(data) && (data != "0" || data != "0.");
            if (!string.IsNullOrWhiteSpace(data.Trim()))
            {
                
                if (!string.IsNullOrWhiteSpace(data))
                {
                    switch (e.Source.ToString())
                    {
                        case "Width":
                            fileCroppingVM.Width = double.Parse(data);
                            break;
                        case "Height":
                            fileCroppingVM.Height = double.Parse(data);
                            break;
                    }
                }
            }
        }
    }
}
