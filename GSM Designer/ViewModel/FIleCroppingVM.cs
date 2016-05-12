using GSM_Designer.Model;
using GSM_Designer.Pages;
using GSM_Designer.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GSM_Designer.ViewModel
{
    public class ExtendedImageFileInfo : ImageFileInfo
    {
        public ExtendedImageFileInfo()
        {


        }
        public ExtendedImageFileInfo(ImageFileInfo info)
        {
            this.FilePath = info.FilePath;
            this.Name = info.Name;
        }

        public int Index { get; set; }
        public object DecodedImage { get; set; }
    }

    public class FileCroppingVM : BaseViewModel
    {
        private iImageWidgetController controller;

        public void RegisterController(iImageWidgetController controller)
        {
            this.controller = controller;
        }

        private string _PatternName;
        public string PatternName
        {
            get { return _PatternName; }
            set { SetFieldAndNotify(ref _PatternName, value); }
        }

        private double _Width = 9;
        public double Width
        {
            get { return _Width; }
            set { SetFieldAndNotify(ref _Width, value); }
        }

        private double _Height = 6.5;
        public double Height
        {
            get { return _Height; }
            set { SetFieldAndNotify(ref _Height, value); }
        }

        private double _CroppedHeight = 4.5;
        public double CroppingHeight
        {
            get { return _CroppedHeight; }
            set { SetFieldAndNotify(ref _CroppedHeight, value); }
        }

        private double _CroppedWidth = 5.0;
        public double CroppedWidth
        {
            get { return _CroppedWidth; }
            set { SetFieldAndNotify(ref _CroppedWidth, value); }
        }

        private ObservableCollection<ExtendedImageFileInfo> _Images;
        public ObservableCollection<ExtendedImageFileInfo> Images
        {
            get { return _Images; }
            set { SetFieldAndNotify(ref _Images, value); }
        }

        public void LoadFiles(ICollection<ImageFileInfo> items)
        {
            var i = 0;
            if (Images == null)
            {
                Images = new ObservableCollection<ExtendedImageFileInfo>();
            }
            foreach (var item in items)
            {
                var data = new ExtendedImageFileInfo(item) { Index = i };
                this.Images.Add(data);
                i++;
            }
            ApplySize();
        }

        public async void ApplySize(bool reset = false)
        {
            if (Images == null || !Images.Any())
                return;
            for (int i = 0; i < Images.Count; i++)
            {
                bool process = i == 0 ? true : false;
                if (File.Exists(Images[i]?.FilePath))
                {
                    ProcessImage(Images[i].FilePath, Width, Height, i, process);
                }
            }
        }

        private async Task ProcessImage(string file, double requiredWidth, double requiredHeight, int index, bool process = false)
        {
            App.TaskQueue.ExecuteTaskAsync(() =>
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(file, UriKind.RelativeOrAbsolute);
                bitmapImage.EndInit();
                var width = bitmapImage.DpiX * requiredWidth;
                var height = bitmapImage.DpiY * requiredHeight;
                var fileName = index + ".jpg";
                var resizedImage = ImageHelper.SaveResizedBitmapImage(bitmapImage, new System.Windows.Size(width, height), fileName);
                bitmapImage = null;
                resizedImage = null;
                BitmapDecoder decoder = null;
                if (process)
                {
                    using (FileStream stream = File.OpenRead(fileName))
                    {
                        decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                        var decodedImage = decoder.Frames[0];
                        if (this.controller != null)
                        {
                            controller.SetSource(decodedImage, index);
                        }
                        decoder = null;
                    }
                }
                GC.Collect();
            }, new TPL.TaskParams(TPL.Priority.Medium));
        }

    }
}
