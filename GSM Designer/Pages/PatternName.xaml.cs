﻿using GSM_Designer.AppNavigationService;
using System.Windows.Controls;

namespace GSM_Designer.Pages
{
    /// <summary>
    /// Interaction logic for PatterName.xaml
    /// </summary>
    public partial class PatternNameWindow : CustomWindow
    {
        public PatternNameWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            NextButton.IsEnabled = (sender as TextBox)?.Text.Length > 0;
        }

        private void NextButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CustomNavigationService.GetNavigationService().Navigate(PageType.ImageCropping, this);
        }
    }
}
