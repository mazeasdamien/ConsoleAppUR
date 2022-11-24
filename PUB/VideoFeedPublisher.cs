using Rti.Types.Dynamic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppUR.PUB
{
    internal class VideoFeedPublisher : Program
    {
        public static string debugCam = "";
        public static void RunPublisher()
        {

            var typeFactory = DynamicTypeFactory.Instance;

            var VideoFeed = typeFactory.BuildStruct()
                .WithName("VideoFeed")
                .AddMember(new StructMember("Index", typeFactory.GetPrimitiveType<int>()))
                .AddMember(new StructMember("Memory", typeFactory.CreateSequence(typeFactory.GetPrimitiveType<byte>(), 1500000)))
                .Create();

            var writer = SetupDataWriter("VideoFeed", Publisher_UR, VideoFeed);
            var sample = new DynamicData(VideoFeed);

            var n = 0;

            while (true)
            {
                if (colorData != null)
                {
                    Bitmap bitmap = GetDataPicture(640, 480, colorData);
                    byte [] jpegfeed = ImageToByte(bitmap);
                    n++;
                    debugCam = $" {n}  Image size  {jpegfeed.Length}                                   \n";
                    sample.SetValue("Index", n);
                    sample.SetValue("Memory", jpegfeed);
                    writer.Write(sample);
                }
            }
        }


        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        public static Bitmap GetDataPicture(int w, int h, byte[] data)
        {
            var pic = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    var arrayIndex = y * w + x;

                    var c = Color.FromArgb(
                       data[arrayIndex],
                       data[arrayIndex + 1],
                       data[arrayIndex + 2],
                       data[arrayIndex + 3]
                    );

                    pic.SetPixel(x, y, c);
                }
            }

            return pic;
        }

    }
}
