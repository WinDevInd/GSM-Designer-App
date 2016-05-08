﻿using GSM_Designer.Pages;
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
        PatternName
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
            }
            return null;
        }

        private static CustomNavigationService _NavigationService;

        private CustomNavigationService() { }

        static CustomNavigationService() { _NavigationService = new CustomNavigationService(); }

        public static CustomNavigationService GetNavigationService() { return _NavigationService; }

        Dictionary<PageType, CustomWindow> windowsCache = new Dictionary<PageType, CustomWindow>();
        Stack<PageType> NavigationStack = new Stack<PageType>();

        public void Navigate(PageType pageType, CustomWindow currentWindow, object navigationPayload = null)
        {
            NaviagetToPage(false, pageType, currentWindow, navigationPayload);
        }

        public void GoBack(CustomWindow currentWindow, object navigationPayload = null)
        {
            NavigationStack.Pop();
            if (NavigationStack.Any())
            {
                var pageType = NavigationStack.Peek();
                if (!NavigationStack.Contains(pageType))
                {
                    windowsCache.Remove(pageType);
                }
                NaviagetToPage(true, pageType, currentWindow, navigationPayload);
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
    }
}
