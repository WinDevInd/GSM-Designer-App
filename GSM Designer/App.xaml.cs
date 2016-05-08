using GSM_Designer.AppNavigationService;
using System.Windows;

namespace GSM_Designer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            CustomNavigationService.GetNavigationService().Navigate(PageType.SelectFile, null);
        }
    }
}
