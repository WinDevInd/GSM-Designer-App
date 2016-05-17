using GSM_Designer.AppNavigationService;
using System;
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

        public bool IsDialog
        {
            get;
            private set;
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
        }

        public CustomWindow(bool isDialog)
        {
            Init();
            this.IsDialog = isDialog;
        }

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

        public void ShowWindow(object payload, bool isBacknav = false,bool isDilogPage = false)
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

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (!IsDialog)
            {
                GC.Collect();
                CustomNavigationService.GetNavigationService().GoBack(this);
            }
            NavigateAway();
            IsDialog = false;
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
