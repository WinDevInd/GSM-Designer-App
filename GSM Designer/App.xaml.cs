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
        public static TPL.TPL TaskQueue;
        public static Dispatcher UIDispatcher;
        public App()
        {
            TaskQueue = new TPL.TPL(3);
            UIDispatcher = this.Dispatcher;
            NavigationParam navParam = new NavigationParam()
            {
                PageType = PageType.SelectFile,
                WindowType = WindowsType.WindowPage,
                RemoveOnAway = false
            };
            CustomNavigationService.GetNavigationService().Navigate(null, navParam);
            //new Window1().Show();
        }
    }
}
