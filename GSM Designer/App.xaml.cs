using GSM_Designer.AppNavigationService;
using System.Windows;
using System.Windows.Threading;
using TPL;

namespace GSM_Designer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static TPL.TPL TaskQueue;
        static Dispatcher UIDispatcher;
        public App()
        {
            TaskQueue = new TPL.TPL(3);
            UIDispatcher = this.Dispatcher;
            CustomNavigationService.GetNavigationService().Navigate(PageType.SelectFile, null);
        }
    }
}
