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
        private static double DefaultWidth = 18.5;
        private static double DefaultHeight = 16.5;
        private double CurrentWidth = DefaultWidth;
        private double currentHeight = DefaultHeight;
        private static FileCroppingVM _Instance;
        private iImageWidgetController controller;

        private FileCroppingVM()
        {

        }
        static FileCroppingVM()
        {
            _Instance = new FileCroppingVM();
        }

        public static FileCroppingVM Instance
        {
            get
            {
                return _Instance;
            }
        }

        private bool _IsLoading;

        public bool IsLoading
        {
            get { return _IsLoading; }
            set
            {
                if (value != _IsLoading)
                {
                    _IsLoading = value;
                    OnPropertyChanged();
                    if (this.controller != null)
                    {
                        controller.UpdateUI(value);
                    }
                }
            }
        }

        public void RegisterController(iImageWidgetController controller)
        {
            this.controller = controller;
        }

        private string _PatternName = "GSM PTRN 1";
        public string PatternName
        {
            get { return _PatternName; }
            set { SetFieldAndNotify(ref _PatternName, value); }
        }

        private double _Width = 18.5;
        public double Width
        {
            get { return _Width; }
            set
            {
                if (_Width != value)
                {
                    _Width = value;
                    CroppedWidth = (value - 0.5) / 2;
                }
            }
        }

        private double _Height = 16;
        public double Height
        {
            get { return _Height; }
            set
            {
                if (_Height != value)
                {
                    _Height = value;
                    CroppedHeight = _Height / 4;
                }
            }
        }

        public double DPIX { get; set; }
        public double DPIY { get; set; }

        private double _CroppedHeight = 4;
        public double CroppedHeight
        {
            get { return _CroppedHeight; }
            set { SetFieldAndNotify(ref _CroppedHeight, value); }
        }

        private double _CroppedWidth = 9;
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

        public async Task LoadFiles(ICollection<ImageFileInfo> items)
        {
            var i = 0;
            Images = new ObservableCollection<ExtendedImageFileInfo>();
            foreach (var item in items)
            {
                var data = new ExtendedImageFileInfo(item) { Index = i };
                this.Images.Add(data);
                i++;
            }
            ApplySize();
        }

        public async Task ApplySize(bool reset = false)
        {
            IsLoading = true;
            if (Images == null || !Images.Any())
                return;
            if (reset && controller != null)
            {
                controller.Reset();
            }
            for (int i = 0; i < Images.Count; i++)
            {
                bool callbackToView = i == 0 ? true : false;
                await App.TaskQueue.ExecuteTaskAsync(() =>
                {
                    if (File.Exists(Images[i]?.FilePath))
                    {
                        ProcessImage(Images[i].FilePath, Width, Height, i, callbackToView);
                    }
                }, new TPL.TaskParams(TPL.Priority.Medium));
            }
            IsLoading = false;
        }

        public async Task CropSelectedAreaInSecondaryImages(double x, double y)
        {
            IsLoading = true;
            for (int i = 1; i < Images.Count; i++)

                await App.TaskQueue.ExecuteTaskAsync(() =>
               {
                   CropImage(i, x, y);
               }, new TPL.TaskParams(TPL.Priority.Medium));
            IsLoading = false;
        }

        private async Task ProcessImage(string file, double requiredWidth, double requiredHeight, int index, bool process = false)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(file, UriKind.RelativeOrAbsolute);
            bitmapImage.EndInit();
            var width = bitmapImage.DpiX * requiredWidth;
            var height = bitmapImage.DpiY * requiredHeight;
            if (process)
            {
                DPIX = bitmapImage.DpiX;
                DPIY = bitmapImage.DpiY;
            }
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
            GC.Collect(0, GCCollectionMode.Optimized);
        }

        private async Task CropImage(int index, double x, double y)
        {
            var fileName = index + ".jpg";
            var croppedFileName = "cropped" + index + ".jpg";
            if (File.Exists(fileName))
            {
                var imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.UriSource = new Uri(fileName, UriKind.RelativeOrAbsolute);
                imageSource.EndInit();
                var imageToSave = ImageHelper.ProcessCroping(imageSource, new System.Windows.Size(CroppedWidth * DPIX, CroppedHeight * DPIY),
                    new System.Windows.Point(x, y));
                var bitmapEncoder = new JpegBitmapEncoder();
                bitmapEncoder.Frames.Add(BitmapFrame.Create(imageToSave));
                var finalImage = new BitmapImage();
                using (Stream stream = File.Create(croppedFileName))
                {
                    bitmapEncoder.Save(stream);
                    bitmapEncoder = null;
                    finalImage = null;
                    imageSource = null;
                }
                using (FileStream stream = File.OpenRead(croppedFileName))
                {
                    var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                    var decodedImage = decoder.Frames[0];
                    if (this.controller != null)
                    {
                        controller.SetSource(decodedImage, index);
                    }
                    decoder = null;
                }
            }
        }
    }
}
