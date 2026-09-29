using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Security.Cryptography;

namespace college_events_desktop.Services
{
    public static class ImageService
    {
        public static ImageBrush SelectImage()
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog()
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*",
                Title = "Выберите изображение"
            };
            ofd.ShowDialog();

            if (ofd.FileName != null)
            {    
                return LoadImage(ofd.FileName);   
            }
            throw new Exception("Пользователь отменил выбор изображения.");
        }

        public static ImageBrush LoadImage(string filePath)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(filePath);
            bitmap.EndInit();
            return new ImageBrush(bitmap);
        }
    }
}
