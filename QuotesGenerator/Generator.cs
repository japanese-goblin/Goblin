using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace QuotesGenerator
{
    public static class Generator
    {
        public static async Task<byte[]> GenerateQuote(string quote, long id, string name, DateTime date, string imageUrl)
        {
            var dateString = date.ToShortDateString();
            var bitmap = new Bitmap(700, 394, PixelFormat.Format32bppArgb);
            var graphics = Graphics.FromImage(bitmap);
            graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, bitmap.Width, bitmap.Height));

            //graphics.SmoothingMode = SmoothingMode.AntiAlias;
            //graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            var rectF = new RectangleF(10, 10, bitmap.Width, 70);
            graphics.DrawString("Цитаты великих людей", new Font("Tahoma", 40), Brushes.White, rectF);

            var image = await GetImageFromUrl(imageUrl);
            image = new Bitmap(image, new Size(195, 212));
            graphics.DrawImage(image, new PointF(10, 100));
            
            var citRect = new RectangleF(220, 100, bitmap.Width - 220, 270);
            graphics.DrawString(quote, new Font("Tahoma", 20), Brushes.White, citRect);
            
            var nameRect = new RectangleF(400, bitmap.Height - 50, bitmap.Width - 410, 60);
            graphics.DrawString($"(c) {name}\nid{id}",
                new Font("Tahoma", 15), Brushes.White, nameRect, new StringFormat
                {
                    Alignment = StringAlignment.Far
                });

            var dateRect = new RectangleF(10, bitmap.Height - 25, 200, 60);
            graphics.DrawString(dateString,
                new Font("Tahoma", 15), Brushes.White, dateRect);

            graphics.Flush();
            var bytes = ToByteArray(bitmap);

            bitmap.Dispose();
            graphics.Dispose();
            image.Dispose();

            return bytes;
        }

        internal static async Task<Image> GetImageFromUrl(string url)
        {
            using (var client = new HttpClient())
            {
                var str = await client.GetStreamAsync(url);
                return Image.FromStream(str);
            }
        }

        internal static byte[] ToByteArray(Bitmap bitmap)
        {
            using(var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }
    }
}
