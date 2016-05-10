using GSM_Designer.Model;
using GSM_Designer.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private int _Width;
        public int Width
        {
            get { return _Width; }
            set { SetFieldAndNotify(ref _Width, value); }
        }

        private int _Height;
        public int Height
        {
            get { return _Height; }
            set { SetFieldAndNotify(ref _Height, value); }
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
            foreach (var item in items)
            {
                var data = new ExtendedImageFileInfo(item) { Index = i };
                this.Images.Add(new ExtendedImageFileInfo(data));

                i++;
            }
        }

        #region Way1
        private string PrimaryImagePath;
        private BitmapImage _PrimaryContainer;
        public BitmapImage PrimaryImage
        {
            get { return _PrimaryContainer; }
            set { _PrimaryContainer = value; }
        }

        private string BImagePath;
        private BitmapImage _BImage;
        public BitmapImage BImage
        {
            get { return _BImage; }
            set { SetFieldAndNotify(ref _BImage, value); }
        }

        private string CImagePath;
        private BitmapImage _CImage;
        public BitmapImage CImage
        {
            get { return _CImage; }
            set { SetFieldAndNotify(ref _CImage, value); }
        }

        private string DImagePath;
        private BitmapImage _DImage;
        public BitmapImage DImage
        {
            get { return _DImage; }
            set { SetFieldAndNotify(ref _DImage, value); }
        }

        private string EImagePath;
        private BitmapImage _EImage;
        public BitmapImage EImage
        {
            get { return _EImage; }
            set { SetFieldAndNotify(ref _EImage, value); }
        }
        #endregion

    }
}
