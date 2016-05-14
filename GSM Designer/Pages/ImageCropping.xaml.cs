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
    /// Interaction logic for ImageCropping.xaml
    /// </summary>
    public partial class ImageCroppingWindow : CustomWindow, iImageWidgetController
    {
        FileCroppingVM vm;
        public ImageCroppingWindow()
        {
            InitializeComponent();
            FileCroppingVM vm = FileCroppingVM.Instance;
            vm.RegisterController(this);
            vm.LoadFiles(InfoViewModel.Instance.Files);
            this.DataContext = vm;
        }

        public void Reset()
        {
        }

        public void SetSource(object BitmapImage, int containerIndex)
        {
            this.Dispatcher.InvokeAsync(new Action(() =>
            {
                switch (containerIndex)
                {
                    case 0:
                        this.PrimaryImage.Source = BitmapImage as ImageSource;
                        break;
                    case 1:
                        this.BImage.Source = BitmapImage as ImageSource;
                        break;
                    case 2:
                        this.CImage.Source = BitmapImage as ImageSource;
                        break;
                    case 3:
                        this.DImage.Source = BitmapImage as ImageSource;
                        break;
                    case 4:
                        this.EImage.Source = BitmapImage as ImageSource;
                        break;
                }
            }));
        }

        public void UpdateUI(bool isProcessing)
        {
            throw new NotImplementedException();
        }

        protected override void OnClosed(EventArgs e)
        {
            this.PrimaryImage.Source = null;
            this.BImage.Source = null;
            this.CImage.Source = null;
            this.DImage.Source = null;
            this.EImage.Source = null;
            base.OnClosed(e);
            vm = null;
        }
    }
}
