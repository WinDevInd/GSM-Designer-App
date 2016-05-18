using GSM_Designer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace GSM_Designer.Controls
{
    /// <summary>
    /// Interaction logic for SizeControl.xaml
    /// </summary>
    public partial class SizeControl : UserControl
    {
        public SizeControl()
        {
            InitializeComponent();
            DataObject.AddPastingHandler(WidthBox, OnPaste);
            DataObject.AddPastingHandler(HeightBox, OnPaste);
        }

        public event EventHandler<TextChangedEventArgs> TextChanged;

        public double CanvasWidth
        {
            get { return (double)GetValue(CanvasWidthProperty); }
            set { SetValue(CanvasWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanvasWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanvasWidthProperty =
            DependencyProperty.Register("CanvasWidth", typeof(double), typeof(SizeControl), new PropertyMetadata(16.5));


        public double CanvasHeight
        {
            get { return (double)GetValue(CanvasHeightProperty); }
            set { SetValue(CanvasHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanvasHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanvasHeightProperty =
            DependencyProperty.Register("CanvasHeight", typeof(double), typeof(SizeControl), new PropertyMetadata(9.45));


        private void TextBox_TextChanged(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex(@"^-*[0-9,\.]+$");
            if (!regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
            else
            {

            }
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            var isText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true);
            if (!isText) return;

            var text = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;
            text = TextHelper.TextToPositiveDouble(text);
            if (!string.IsNullOrWhiteSpace(text))
            {
                (sender as TextBox).Text = text.ToString();
            }
            e.CancelCommand();
        }

        private void TextBox_TextChanged_Route(object sender, TextChangedEventArgs e)
        {
            var tag = (sender as TextBox).Tag;
            if (this.TextChanged != null)
            {
                e.Source = tag;
                this.TextChanged(sender, e);
            }
        }
    }
}
