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

namespace GSM_Designer
{
    /// <summary>
    /// Interaction logic for ImageCrop.xaml
    /// </summary>
    public partial class ImageCrop : CustomWindow, iImageWidgetController
    {
        private FileCroppingVM fileCroppingVM;
        public ImageCrop()
        {
            fileCroppingVM = new FileCroppingVM();
            InitializeComponent();
            this.DataContext = fileCroppingVM;
        }


        public override void Navigated(object payload)
        {
            InitializeVM();
        }

        private void InitializeVM()
        {
            var images = InfoViewModel.Instance.Files;
            fileCroppingVM.RegisterController(this);
            fileCroppingVM.LoadFiles(images);
        }

        public void SetSource(object BitmapImage, int containerIndex)
        {

            if (containerIndex == 0)
            {
                this.Dispatcher.InvokeAsync(new Action(() =>
                {
                    this.ImageContainer.Source = BitmapImage as ImageSource;
                }));
            }
        }

        private void SizeButton_Click(object sender, RoutedEventArgs e)
        {
            CanvasSizeDialog c = new CanvasSizeDialog();
            c.DataContext = this.DataContext;
            c.ShowDialog();
        }

        public void Reset()
        {
            this.ImageContainer.Source = null;
            this.BCrop.Source = null;
            this.CCrop.Source = null;
            this.DCrop.Source = null;
            this.ECrop.Source = null;
        }
    }
}
