﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GSM_Designer.Pages
{
    public class CustomWindow : Window, iCustomNavigationService
    {
        private object navigationPayLaod = null;
        public CustomWindow()
        {
            this.Effect = new System.Windows.Media.Effects.DropShadowEffect()
            {
                Color = new System.Windows.Media.Color() { A = 12, R = 00, G = 00, B = 00 },
                BlurRadius = 1,
                ShadowDepth = 2
            };
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public void ShowWindow(object payload, bool isBacknav = false)
        {
            Show();
            if (isBacknav)
            {
                NavigatedBack(payload);
            }
            else
            {
                Navigated(payload);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            CustomNavigationService.GetNavigationService().GoBack(this);
        }

        public virtual void Navigated(object payload)
        {
            //// Forward Navigation
        }

        public virtual void NavigateAway()
        {
            //// this windows is hidden now
        }

        protected virtual void NavigatedBack(object payload)
        {
            //// back navigation
        }

        public void Navigate(object payload, bool isBacknav = false)
        {
            Show();
            if (isBacknav)
            {
                NavigatedBack(payload);
            }
            else
            {
                Navigated(payload);
            }

        }
    }
}