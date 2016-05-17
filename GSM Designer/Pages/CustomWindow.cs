﻿using GSM_Designer.AppNavigationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace GSM_Designer.Pages
{
    public class CustomWindow : Window, iCustomNavigationService
    {
        private object navigationPayLaod = null;

        public bool IsDialog
        {
            get;
            private set;
        }

        private bool CanExit
        {
            get;
            set;
        }

        private void Init()
        {
            this.Effect = new System.Windows.Media.Effects.DropShadowEffect()
            {
                Color = new System.Windows.Media.Color() { A = 12, R = 00, G = 00, B = 00 },
                BlurRadius = 1,
                ShadowDepth = 2
            };
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.KeyDown += CustomWindow_KeyDown;
        }

        private void CustomWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (!this.IsDialog)
                {
                    this.GoBack();
                    return;
                }
                this.CloseWindow();
            }
        }

        public CustomWindow(bool isDialog)
        {
            Init();
            this.IsDialog = isDialog;
        }

        public CustomWindow()
        {
            Init();
        }

        public void ShowWindow(object payload, bool isBacknav = false, bool isDilogPage = false)
        {
            IsDialog = isDilogPage;
            Show();
            if (isBacknav)
            {
                NavigatedBack(payload);
            }
            else
            {
                Navigate(payload);
            }
        }

        public void ShowDialog(object payload)
        {
            IsDialog = true;
            Navigate(payload);
            ShowDialog();
        }

        private void GoBack()
        {
            if (!IsDialog)
            {
                GC.Collect();
                CustomNavigationService.GetNavigationService().GoBack(this);
            }
            else
            {
                CloseWindow(false);
            }
            NavigateAway();
            IsDialog = false;
        }

        public void CloseWindow(bool exit = false)
        {
            CanExit = exit;
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {

            base.OnClosed(e);
            if (CanExit)
            {
                Application.Current.Shutdown(0);
            }
        }

        protected virtual void Navigated(object payload)
        {
            //// Forward Navigation
        }

        protected virtual void NavigateAway()
        {
            //// this windows is hidden now

        }

        protected virtual void NavigatedBack(object payload)
        {
            //// back navigation
        }

        protected virtual void Navigate(object payload, bool isBacknav = false)
        {
            //// navigate forward
        }


    }
}
