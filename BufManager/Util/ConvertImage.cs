using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BufManager.Util
{

    static class ConvertImage
    {
        const int threshold = 65000;
        public static string DataFromImage(System.Drawing.Image image)
        {

            int maxWidth = int.MaxValue;
            if (maxWidth <= 0)
            {
                maxWidth = 1;
            }

            if (maxWidth > image.Width)
            {
                maxWidth = image.Width;
            }

            // The quantizer runs super slow with big images so start at a small reasonable size
            int newWidth = 640; // starting point

            if (newWidth > maxWidth)
            {
                newWidth = maxWidth;
            }

            string data = "";

            // keep scaling down until we go below the threshold
            data = ScaleToWidth(image, newWidth);

            if (data.Length < threshold && newWidth < maxWidth)
            {
                // already below threshold at our starting point? try starting at 1600 px then

                newWidth = 1600;

                if (newWidth > maxWidth)
                {
                    newWidth = maxWidth;
                }

                data = ScaleToWidth(image, newWidth);
            }

            if (data.Length < threshold && newWidth < maxWidth)
            {
                // still below threshold at our starting point? start with the real size of the image i suppose, up to the hardlimit
                const int hardLimit = 3840;

                newWidth = image.Width;
                if (newWidth > hardLimit)
                {
                    newWidth = hardLimit;
                }

                if (newWidth > maxWidth)
                {
                    newWidth = maxWidth;
                }

                data = ScaleToWidth(image, newWidth);
            }

            while (data.Length >= threshold)
            {
                int nextWidth = newWidth;

                // scale down
                double diffRatio = Math.Sqrt(threshold / (double)data.Length);
                nextWidth = (int)(nextWidth * diffRatio);

                // align to 8 px
                nextWidth = (nextWidth / 8) * 8;

                if (nextWidth == newWidth)
                {
                    nextWidth -= 8;
                }

                if (nextWidth <= 0)
                {
                    throw new Exception("Something very troubling is going on resizing this image you gave me.");
                }

                newWidth = nextWidth;
                data = ScaleToWidth(image, newWidth);
            }

            // now we are below the threshold! this time let's just move up until we go over.
            while (newWidth < maxWidth)
            {
                newWidth += 16;
                string newData = ScaleToWidth(image, newWidth);
                if (newData.Length < threshold)
                {
                    data = newData;
                }
                else
                {
                    break;
                }
            }

            return data;
        }

        public static string ScaleToWidth(System.Drawing.Image image, int width)
        {
            var newWidth = width;
            var newHeight = (int)((image.Height * (width / (double)image.Width)) + 0.5);

            string data = "";

            if (newWidth == image.Width && newHeight == image.Height)
            {
                data = ImageToData(image);
            }
            else
            {
                using (var newImage = new System.Drawing.Bitmap(image, newWidth, newHeight))
                {
                    data = ImageToData(newImage);
                }
            }

            System.Diagnostics.Debug.Print("Resize ({0},{1}) -> ({2},{3}) -- len={4}", image.Width, image.Height, newWidth, newHeight, data.Length);

            return data;
        }
        static string ImageToData(System.Drawing.Image image)
        {
            using (var memStream = new System.IO.MemoryStream())
            {
                var quantizer = new ImageQuantization.OctreeQuantizer(255, 8);
                using (var quantized = quantizer.Quantize(image))
                {
                    quantized.Save(memStream, System.Drawing.Imaging.ImageFormat.Gif);
                }

                //image.Save(memStream, System.Drawing.Imaging.ImageFormat.Gif);

                return "base64:" + System.Convert.ToBase64String(memStream.ToArray());
            }
        }
    }
}
