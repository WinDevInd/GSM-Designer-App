using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GSM_Designer
{
    /// <summary>
    /// Interaction logic for CanvasSizeDialog.xaml
    /// </summary>
    public partial class CanvasSizeDialog : Window
    {
        public CanvasSizeDialog()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string command = Convert.ToString(btn?.CommandParameter);
            switch(command)
            {
                case "apply":
                    //// do something
                    break;
                case "ok":
                    this.Close();
                    break;
            }
        }
    }
}
