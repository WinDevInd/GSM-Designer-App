using GSM_Designer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GSM_Designer.ViewModel
{
    public class InfoViewModel : BaseViewModel
    {
        private static int MinFileCount = 5;
        static InfoViewModel()
        {
            _Instance = new InfoViewModel();
        }

        private static InfoViewModel _Instance;
        public static InfoViewModel Instance
        {
            get
            {
                return _Instance;
            }
        }

        private InfoViewModel()
        {

        }

        private ObservableCollection<ImageFileInfo> _Files;
        public ObservableCollection<ImageFileInfo> Files
        {
            get { return _Files; }
            set { SetFieldAndNotify(ref _Files, value); }
        }


        public bool CanGoNextScreen
        {
            get { return Files?.Count == MinFileCount; }
        }

        public void LoadFiles(IEnumerable<string> items, bool shouldClearAll = true)
        {
            if (Files == null)
            {
                Files = new ObservableCollection<ImageFileInfo>();
                Files.CollectionChanged += Files_CollectionChanged;
            }
            else if (shouldClearAll)
            {
                Files.Clear();
            }
            foreach (var file in items)
            {
                Files.Add(new ImageFileInfo()
                {
                    FilePath = file,
                    Name = file.Substring(file.LastIndexOf("\\") + 1)
                });
            }
        }

        private void Files_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("CanGoNextScreen");
        }
    }

}
