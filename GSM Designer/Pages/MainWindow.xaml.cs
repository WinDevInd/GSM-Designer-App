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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using GSM_Designer.ViewModel;
using GSM_Designer.Utils;
using GSM_Designer.Model;
using GSM_Designer.Pages;

namespace GSM_Designer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SelectFileWindow : CustomWindow
    {
        private double defaultWidth = 16.5 * 96;
        private Point _StartPoint;
        private DragAdorner _adorner;
        private AdornerLayer _layer;
        private bool _dragIsOutOfScope = false;

        public SelectFileWindow()
        {
            InitializeComponent();
            this.DataContext = InfoViewModel.Instance;
        }

        public string[] OpenFile()
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.Multiselect = true;
            //fileDialog.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif|TIFF Files (*.tiff)|*.tiff";
            fileDialog.Filter = "Image Files |*.jpeg;*.png;*.jpg;*.gif,;*.tif";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = fileDialog.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string[] filename = fileDialog.FileNames;
                return filename;
            }
            return null;
        }

        private void SelectFile_BtnClick(object sender, RoutedEventArgs e)
        {
            var files = OpenFile();
            InfoViewModel.Instance.LoadFiles(files);
        }


        #region ListView ReOrder
        private void ListView_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("myFormat") || sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void ListView_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("myFormat"))
            {
                ImageFileInfo name = e.Data.GetData("myFormat") as ImageFileInfo;
                ListViewItem listViewItem = UIHelper.FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);

                if (listViewItem != null)
                {
                    var nameToReplace = FileListView.ItemContainerGenerator.ItemFromContainer(listViewItem);
                    int index = FileListView.Items.IndexOf(nameToReplace);

                    if (index >= 0)
                    {
                        InfoViewModel.Instance.Files.Remove(name);
                        InfoViewModel.Instance.Files.Insert(index, name);
                    }
                }
                else
                {
                    InfoViewModel.Instance.Files.Remove(name);
                    InfoViewModel.Instance.Files.Add(name);
                }

            }
        }

        private void ListViewPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point position = e.GetPosition(null);

                if (Math.Abs(position.X - _StartPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - _StartPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    BeginDrag(e);
                }
            }

        }

        private void ListViewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _StartPoint = e.GetPosition(null);
        }

        private void BeginDrag(MouseEventArgs e)
        {
            ListView listView = this.FileListView;
            ListViewItem listViewItem =
                UIHelper.FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);

            if (listViewItem == null)
                return;

            // get the data for the ListViewItem
            var name = listView.ItemContainerGenerator.ItemFromContainer(listViewItem);

            //setup the drag adorner.
            InitialiseAdorner(listViewItem, listView);

            //add handles to update the adorner.
            listView.PreviewDragOver += ListViewDragOver;
            listView.DragLeave += ListViewDragLeave;
            listView.DragEnter += ListView_DragEnter;

            DataObject data = new DataObject("myFormat", name);
            DragDropEffects de = DragDrop.DoDragDrop(listView, data, DragDropEffects.Move);

            //cleanup 
            listView.PreviewDragOver -= ListViewDragOver;
            listView.DragLeave -= ListViewDragLeave;
            listView.DragEnter -= ListView_DragEnter;

            if (_adorner != null)
            {
                AdornerLayer.GetAdornerLayer(listView).Remove(_adorner);
                _adorner = null;
            }
        }

        private void InitialiseAdorner(ListViewItem listViewItem, ListView listView)
        {
            VisualBrush brush = new VisualBrush(listViewItem);
            _adorner = new DragAdorner((UIElement)listViewItem, listViewItem.RenderSize, brush);
            _adorner.Opacity = 0.5;
            _layer = AdornerLayer.GetAdornerLayer(listView as Visual);
            _layer.Add(_adorner);
        }

        void ListViewQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (this._dragIsOutOfScope)
            {
                e.Action = DragAction.Cancel;
                e.Handled = true;
            }
        }

        void ListViewDragLeave(object sender, DragEventArgs e)
        {
            if (e.OriginalSource == FileListView)
            {
                Point p = e.GetPosition(FileListView);
                Rect r = VisualTreeHelper.GetContentBounds(FileListView);
                if (!r.Contains(p))
                {
                    this._dragIsOutOfScope = true;
                    e.Handled = true;
                }
            }
        }

        void ListViewDragOver(object sender, DragEventArgs args)
        {
            if (_adorner != null)
            {
                _adorner.OffsetLeft = args.GetPosition(FileListView).X;
                _adorner.OffsetTop = args.GetPosition(FileListView).Y - _StartPoint.Y;
            }
        }

        #endregion

        private void AddMoreFileButton_Click(object sender, RoutedEventArgs e)
        {
            var files = OpenFile();
            InfoViewModel.Instance.LoadFiles(files, false);

        }

        private void RemoveItem_ButtonClick(object sender, RoutedEventArgs e)
        {
            ImageFileInfo data = ((sender as Button)?.DataContext) as ImageFileInfo;
            if (data != null)
            {
                InfoViewModel.Instance.Files.Remove(data);
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            CustomNavigationService.GetNavigationService().Navigate(PageType.PatternName, this);
        }
        //public void SetImage(Image image)
        //{

        //    DrawingVisual drawingVisual = new DrawingVisual();
        //    using (DrawingContext drawingContext = drawingVisual.RenderOpen())
        //    {
        //        for (int x = 0; x <= image.ActualWidth - ((System.Windows.Media.Imaging.BitmapFrame)image.Source).PixelWidth; x += (int)image.Width)
        //        {
        //            drawingContext.DrawImage(image.Source, new Rect(0, 0, x, image.Height));
        //        }
        //    }

        //    RenderTargetBitmap bmp = new RenderTargetBitmap((int)image.Source.Width, (int)image.Height, 600, 600, PixelFormats.Pbgra32);
        //    bmp.Render(drawingVisual);
        //    Test.Source = bmp;
        //}
        //}

    }
}
