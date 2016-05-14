using GSM_Designer.AppNavigationService;
using GSM_Designer.ViewModel;
using System.Windows.Controls;

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
            Intialize();
        }

        public PatternNameWindow(object dataContext) : base(true)
        {
            Intialize();
            isDialog = true;
        }

        private void Intialize()
        {
            InitializeComponent();
            fileCroppingVM = FileCroppingVM.Instance;
            this.DataContext = fileCroppingVM;
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
                    CustomNavigationService.GetNavigationService().Navigate(PageType.ImageCropping, this);
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
