using GSM_Designer.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GSM_Designer.AppNavigationService
{
    public enum PageType
    {
        SelectFile,
        PatternName,
        ImageCropping
    }

    public class CustomNavigationService
    {
        private Type GetPage(PageType type)
        {
            switch (type)
            {
                case PageType.PatternName:
                    return typeof(PatternNameWindow);
                case PageType.SelectFile:
                    return typeof(SelectFileWindow);
                case PageType.ImageCropping:
                    return typeof(ImageCrop);
            }
            return null;
        }

        private static CustomNavigationService _NavigationService;

        private CustomNavigationService() { }

        static CustomNavigationService() { _NavigationService = new CustomNavigationService(); }

        public static CustomNavigationService GetNavigationService() { return _NavigationService; }

        Dictionary<PageType, CustomWindow> windowsCache = new Dictionary<PageType, CustomWindow>();
        Stack<PageType> NavigationStack = new Stack<PageType>();

        public void Navigate(PageType pageType, CustomWindow currentWindow, object navigationPayload = null, bool autoRemove = false)
        {
            if (!autoRemove)
                NaviagetToPage(false, pageType, currentWindow, navigationPayload);
            else
                NaviagetToPage(pageType, currentWindow, navigationPayload); /// no history maintain
        }

        public void GoBack(CustomWindow currentWindow, object navigationPayload = null)
        {
            var pageType = NavigationStack.Pop();
            if (NavigationStack.Any())
            {

                if (!NavigationStack.Contains(pageType))
                {
                    windowsCache.Remove(pageType);
                }
                var topItemOnStack = NavigationStack.Peek();
                NaviagetToPage(true, topItemOnStack, currentWindow, navigationPayload);
            }
            else
            {
                Application.Current.ShutdownMode = ShutdownMode.OnLastWindowClose;
                Application.Current.Shutdown(0);
            }
        }

        private void NaviagetToPage(bool isbacknav, PageType pageType, CustomWindow currentWindow, object navigationPayload = null)
        {
            var type = GetPage(pageType);

            if (!windowsCache.ContainsKey(pageType))
            {
                var newWindow = (CustomWindow)Activator.CreateInstance(type);
                windowsCache[pageType] = newWindow;
                NavigationStack.Push(pageType);
            }
            var window = windowsCache[pageType];
            currentWindow?.Hide();
            (window as iCustomNavigationService)?.Navigate(null);
        }

        private void NaviagetToPage(PageType pageType, CustomWindow currentWindow, object navigationPyload)
        {
            var type = GetPage(pageType);
            var newWindow = (CustomWindow)Activator.CreateInstance(type);
            currentWindow.Hide();
            newWindow.Show();

        }
    }
}
