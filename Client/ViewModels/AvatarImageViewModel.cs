using Client.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Point = System.Windows.Point;

namespace Client.ViewModels
{
    public class AvatarImageViewModel:ViewModel
    {
        private string imagePath = string.Empty;
        public int SideLength { get; set; } = 50;
        public Point EllipseCenter { get; set; } = new Point(25, 25);
        public int Radius { get; set; } = 25;
        public SolidColorBrush? BackgroundChatImage => (ImagePath == string.Empty)
            ? null : new SolidColorBrush(Colors.Gray);
        public string ImagePath
        {
            get => imagePath;
            set
            {
                imagePath = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BackgroundChatImage));
            }
        }
        public AvatarImageViewModel(string pathImage)
        {
            ImagePath = pathImage;
        }
        public AvatarImageViewModel()
        {
            ImagePath = string.Empty;
        }
    }
}
