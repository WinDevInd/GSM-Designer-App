using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesignShow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Load();
            this.DataContext = this;
            this.Height = 50 * 8;
            this.Width = 50 * 9;
        }

        private ObservableCollection<String> _COl;

        public ObservableCollection<String> Col
        {
            get { return _COl; }
            set { _COl = value; }
        }

        public void Load()
        {
            Col = new ObservableCollection<string>();
            for (int i = 0; i < 72; i++)
            {
                Col.Add(i.ToString());
            }
        }

        private void grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            grid.IntendedHeight = e.NewSize.Height / 8;
            grid.IntendedWidth = e.NewSize.Width / 9;
        }
    }
}
