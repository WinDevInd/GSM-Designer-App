using GSM_Designer.AppNavigationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for Splash.xaml
    /// </summary>
    public partial class Splash : CustomWindow
    {
        public Splash()
        {
            InitializeComponent();
            this.Loaded += Splash_Loaded;
            VersionText.Text = Assembly.GetEntryAssembly().GetName().Version.ToString();
        }

        private async void Splash_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(3000);
            CustomNavigationService.GetNavigationService().Navigate(this, new NavigationParam() { PageType = PageType.SelectFile, WindowType = WindowsType.WindowPage });
        }
    }
}
