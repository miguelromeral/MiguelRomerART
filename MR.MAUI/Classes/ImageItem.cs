using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MR.MAUI.Classes
{
    internal class ImageItem : BindableObject
    {
        public ImageSource ImageSource { get; set; }
        public string UrlThumbnail { get; set; }
        public string Id { get; set; }
        public string IdInCollection { get { return $"drawings/{Id}"; } }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }
    }
}
