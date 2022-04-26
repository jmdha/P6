using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ExperimentSuite.Helpers
{
    internal static class ImageHelper
    {
        public static ImageSource ByteToImage(byte[] imageData)
        {
            ImageSource imgSrc = ByteToBmpImage(imageData) as ImageSource;

            return imgSrc;
        }

        public static BitmapImage ByteToBmpImage(byte[] imageData)
        {
            BitmapImage biImg = new BitmapImage();
            MemoryStream ms = new MemoryStream(imageData);
            biImg.BeginInit();
            biImg.StreamSource = ms;
            biImg.EndInit();

            return biImg;
        }
    }
}
