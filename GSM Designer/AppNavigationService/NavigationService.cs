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
        PatternName,
        ImageCropping,
        ImageOutput
    }

    public enum WindowsType
    {
        WindowPage,
        DialogPage,
        Dialog
    }

    public class NavigationParam
    {
        public PageType PageType { get; set; }
        public WindowsType WindowType { get; set; }
        public object NavigationPayload { get; set; }
        public bool RemoveOnAway { get; set; }
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
                case PageType.ImageOutput:
                    return typeof(ImageOutput);
            }
            return null;
        }

        private static CustomNavigationService _NavigationService;

        private CustomNavigationService() { }

        static CustomNavigationService() { _NavigationService = new CustomNavigationService(); }

        public static CustomNavigationService GetNavigationService() { return _NavigationService; }

        Dictionary<PageType, CustomWindow> windowsCache = new Dictionary<PageType, CustomWindow>();

        Stack<NavigationParam> NavigationStack = new Stack<NavigationParam>();


        public void GoBack(CustomWindow currentWindow, object navigationPayload = null)
        {
            var lastPage = NavigationStack.Pop();
            if (NavigationStack.Any())
            {

                if (!NavigationStack.Contains(lastPage))
                {
                    windowsCache.Remove(lastPage.PageType);
                }
                var topItemOnStack = NavigationStack.Peek();
                var navParam = topItemOnStack;
                currentWindow.Close();
                currentWindow = null;
                Navigate(currentWindow, navParam,true);
                //NaviagetToPage(true, topItemOnStack, currentWindow, navigationPayload);
            }
            else
            {
                Application.Current.ShutdownMode = ShutdownMode.OnLastWindowClose;
                Application.Current.Shutdown(0);
            }
        }

        public void Navigate(CustomWindow currentWindow, NavigationParam navigationParam, bool isbacknav = false)
        {
            switch (navigationParam.WindowType)
            {
                case WindowsType.WindowPage:
                case WindowsType.DialogPage:
                    NaviagetToPage(currentWindow, navigationParam,isbacknav);
                    break;
                case WindowsType.Dialog:
                    OpenDialog(navigationParam);
                    break;
            }

        }

        private void OpenDialog(NavigationParam navigationParam)
        {
            var pageType = navigationParam.PageType;
            var type = GetPage(pageType);
            var newWindow = (CustomWindow)Activator.CreateInstance(type);
            (newWindow as iCustomNavigationService).ShowDialog(navigationParam.NavigationPayload);
        }

        private void NaviagetToPageDialog(CustomWindow currentWindow, NavigationParam navigationParam)
        {
            if (navigationParam != null)
            {
                var pageType = navigationParam.PageType;
                var type = GetPage(pageType);
                var newWindow = (CustomWindow)Activator.CreateInstance(type);
                currentWindow.Hide();
                (newWindow as iCustomNavigationService).ShowWindow(navigationParam.NavigationPayload, false, true);
            }
        }

        private void NaviagetToPage(CustomWindow currentWindow, NavigationParam navigationParam,bool isbacknav = false)
        {
            var pageType = navigationParam.PageType;
            var type = GetPage(pageType);
            CustomWindow window = null;
            if (!windowsCache.ContainsKey(pageType))
            {
                window = (CustomWindow)Activator.CreateInstance(type);
                if (!navigationParam.RemoveOnAway)
                {
                    windowsCache[pageType] = window;
                    NavigationStack.Push(navigationParam);
                }
            }
            else
            {
                window = windowsCache[pageType];
            }
            if (isbacknav)
                currentWindow?.Close();
            else
                currentWindow?.Hide();
            (window as iCustomNavigationService)?.ShowWindow(navigationParam.NavigationPayload);
        }

    }
}
