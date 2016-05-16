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

    public class ImageFormat
    {
        public string Format { get; set; }
        public string Extension { get; set; }
    }

    public class FileCroppingVM : BaseViewModel
    {
        private static double DefaultWidth = 18.5;
        private static double DefaultHeight = 16.5;
        private static string defaultPatternName = "GSM DESIGN 1";
        private double currentWidth = 0;
        private double currentHeight = 0;
        private string currentFormat = "";
        private static FileCroppingVM _Instance;
        public static string PathPrefix = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\GSM Temp\\";
        private iImageWidgetController controller;

        private FileCroppingVM()
        {
            Directory.CreateDirectory(PathPrefix);
            this.Width = DefaultWidth;
            this.Height = DefaultHeight;
            this.PatternName = defaultPatternName;
            var imageFormatList = new List<ImageFormat>();
            imageFormatList.Add(new ImageFormat() { Extension = ImageHelper.JPEGIMAGEEXTENSION, Format = ImageHelper.JPEGIMAGEFORMAT });
            imageFormatList.Add(new ImageFormat() { Extension = ImageHelper.TIFFIMAGEEXTENSION, Format = ImageHelper.TIFFIAMGEFORMAT });
            ImageFormatList = imageFormatList;
            SelectedFormat = ImageFormatList[0];
        }

        public static FileCroppingVM Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new FileCroppingVM();
                }
                return _Instance;
            }
        }

        private List<ImageFormat> _ImageFormatList;
        public List<ImageFormat> ImageFormatList
        {
            get
            {
                return _ImageFormatList;
            }
            set
            {
                SetFieldAndNotify(ref _ImageFormatList, value);
            }
        }

        private ImageFormat _SelectedFormat;
        public ImageFormat SelectedFormat
        {
            get
            {
                return _SelectedFormat;
            }
            set
            {
                SetFieldAndNotify(ref _SelectedFormat, value);
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

        private string _PatternName;
        public string PatternName
        {
            get { return _PatternName; }
            set { SetFieldAndNotify(ref _PatternName, value); }
        }

        private double _Width;
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

        private double _Height;
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

        private double _CroppedHeight;
        public double CroppedHeight
        {
            get { return _CroppedHeight; }
            set { SetFieldAndNotify(ref _CroppedHeight, value); }
        }

        private double _CroppedWidth;
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
            //// if no change in new and old size do not process
            if (currentWidth == Width && currentHeight == Height && currentFormat == SelectedFormat.Format)
                return;

            currentHeight = Height;
            currentWidth = Width;
            currentFormat = SelectedFormat.Extension;
            IsLoading = true;
            if (Images == null || !Images.Any())
                return;
            if (reset && controller != null)
            {
                controller.Reset();
            }

            for (int index = 0; index < Images.Count; index++)
            {
                bool callbackToView = index == 0 ? true : false;
                var image = await App.TaskQueue.ExecuteTaskAsync(() =>
                {
                    if (File.Exists(Images[index]?.FilePath))
                    {
                        var processedImage = ProcessImage(Images[index].FilePath, Width, Height, index, callbackToView);
                        return processedImage;
                    }
                    return null;
                }, new TPL.TaskParams(TPL.Priority.Medium));
                if (this.controller != null)
                {
                    controller.SetSource(image, index);
                }
            }
            IsLoading = false;
            GC.Collect();
        }

        public async Task CropSelectedAreaInSecondaryImages(double x, double y)
        {
            IsLoading = true;
            for (int index = 1; index < Images.Count; index++)
            {
                var image = await App.TaskQueue.ExecuteTaskAsync(() =>
                {
                    var srcFile = PathPrefix + index + SelectedFormat.Extension;
                    var croppedFile = PathPrefix + "collage" + index + SelectedFormat.Extension;
                    var croppedImage = CropImage(srcFile, croppedFile, x, y);
                    return croppedImage;
                }, new TPL.TaskParams(TPL.Priority.Medium));

                if (controller != null)
                {
                    controller.SetSource(image, index);
                }
            }
            IsLoading = false;
            GC.Collect();
        }

        public void Dispose()
        {
            try
            {
                App.TaskQueue.ExecuteTaskAsync(() =>
                {
                    foreach (var file in Directory.GetFiles(PathPrefix))
                    {
                        File.Delete(file);
                    }
                }, new TPL.TaskParams(TPL.Priority.Medium));
            }
            catch
            {

            }
        }

        private async Task<BitmapFrame> ProcessImage(string file, double requiredWidth, double requiredHeight, int index, bool isPrimary)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(file, UriKind.RelativeOrAbsolute);
            bitmapImage.EndInit();
            var width = bitmapImage.DpiX * requiredWidth;
            var height = bitmapImage.DpiY * requiredHeight;
            if (isPrimary)
            {
                DPIX = bitmapImage.DpiX;
                DPIY = bitmapImage.DpiY;
            }
            string extension = "";
            switch (SelectedFormat.Format)
            {
                case ImageHelper.TIFFIAMGEFORMAT:
                    extension = ImageHelper.TIFFIMAGEEXTENSION;
                    break;
                default:
                    extension = ImageHelper.JPEGIMAGEEXTENSION;
                    break;
            }
            var fileName = isPrimary ? PathPrefix + "collage" + index + extension : PathPrefix + index + extension;
            var resizedImage = ImageHelper.SaveResizedBitmapImage(bitmapImage, new System.Windows.Size(width, height), fileName, SelectedFormat.Format);
            bitmapImage = null;
            resizedImage = null;
            BitmapDecoder decoder = null;
            if (isPrimary)
            {
                using (FileStream stream = File.OpenRead(fileName))
                {
                    decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                    var decodedImage = decoder.Frames[0];
                    decoder = null;
                    return decodedImage;
                }
            }
            return null;
        }

        private async Task<BitmapFrame> CropImage(string sourceFileName, string destinationFileName, double x, double y)
        {
            if (File.Exists(sourceFileName))
            {
                var imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.UriSource = new Uri(sourceFileName, UriKind.RelativeOrAbsolute);
                imageSource.EndInit();
                var imageToSave = ImageHelper.ProcessCroping(imageSource, new System.Windows.Size(CroppedWidth * DPIX, CroppedHeight * DPIY),
                    new System.Windows.Point(x, y));

                var bitmapEncoder = ImageHelper.GetEncoder(SelectedFormat.Format);
                bitmapEncoder.Frames.Add(BitmapFrame.Create(imageToSave));
                var finalImage = new BitmapImage();
                using (Stream stream = File.Create(destinationFileName))
                {
                    bitmapEncoder.Save(stream);
                    bitmapEncoder = null;
                    finalImage = null;
                    imageSource = null;
                }
                using (FileStream stream = File.OpenRead(destinationFileName))
                {
                    var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                    var decodedImage = decoder.Frames[0];
                    decoder = null;
                    return decodedImage;
                }
            }
            return null;
        }
    }
}
