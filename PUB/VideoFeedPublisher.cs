using Rti.Types.Dynamic;
using System.Drawing;
using K4os.Compression.LZ4;

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
                    byte[] compressedjpeg = LZ4Pickler.Pickle(colorData);
                    n++;
                    debugCam = $" {n}  Image size  {colorData.Length}  compressed: {compressedjpeg.Length}                        \n";
                    sample.SetValue("Index", n);
                    sample.SetValue("Memory", compressedjpeg);
                    writer.Write(sample);
                    Thread.Sleep(40);
                }
            }
        }


        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
    }
}
