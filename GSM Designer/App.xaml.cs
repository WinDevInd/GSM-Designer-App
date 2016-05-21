using GSM_Designer.AppNavigationService;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
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
        public static BitmapImage Icon;
        public App()
        {
            TaskQueue = new TPL.TPL(3);
            UIDispatcher = this.Dispatcher;
            try
            {
                Icon = new BitmapImage(new Uri("pack://application:,,,/Resource/logo.ico", UriKind.RelativeOrAbsolute));
            }
            catch { }
            NavigationParam navParam = new NavigationParam()
            {
                PageType = PageType.Splash,
                WindowType = WindowsType.DialogPage,
                RemoveOnAway = true
            };
            CustomNavigationService.GetNavigationService().Navigate(null, navParam);
        }
    }
}
