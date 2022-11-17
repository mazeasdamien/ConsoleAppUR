using Intel.RealSense;
using Rti.Dds.Publication;
using Rti.Types.Dynamic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;


namespace ConsoleAppUR
{
    internal class CameraIntelPublisher : Program
    {
        public static string debugCam = "";

        const int CAMERA_WIDTH = 640;
        const int CAMERA_HEIGHT = 480;

        const int FPS = 30;

        // Stocke des valeurs de couleur et de profondeur de la dernière frame
        private static byte[] colorArray = new byte[CAMERA_WIDTH * CAMERA_HEIGHT * 3];
        private static UInt16[] depthArray = new UInt16[CAMERA_WIDTH * CAMERA_HEIGHT];

        public static void RunPublisher()
        {
            var typeFactory = DynamicTypeFactory.Instance;
            StructType RobotImage = typeFactory.BuildStruct()
                .WithName("Video")
                .AddMember(new StructMember("Index", typeFactory.GetPrimitiveType<int>()))
                .AddMember(new StructMember("Color", typeFactory.CreateSequence(typeFactory.GetPrimitiveType<byte>(), 1000000)))
                .Create();

            DataWriter<DynamicData> writer = SetupDataWriter("Video_Topic", publisher, RobotImage);
            var sample1 = new DynamicData(RobotImage);

            var typeFactory2 = DynamicTypeFactory.Instance;
            StructType RobotImage2 = typeFactory2.BuildStruct()
                .WithName("Depth")
                .AddMember(new StructMember("Index", typeFactory2.GetPrimitiveType<int>()))
                .AddMember(new StructMember("Values", typeFactory2.CreateSequence(typeFactory2.GetPrimitiveType<float>(), 1000000)))
                .Create();

            DataWriter<DynamicData> writer2 = SetupDataWriter("Depth_Topic", publisher, RobotImage2);
            var sample2 = new DynamicData(RobotImage2);

            var cfg = new Config();
            cfg.EnableStream(Intel.RealSense.Stream.Depth, CAMERA_WIDTH, CAMERA_HEIGHT, Format.Z16, FPS);
            cfg.EnableStream(Intel.RealSense.Stream.Color, CAMERA_WIDTH, CAMERA_HEIGHT, Format.Rgb8, FPS);

            var pipe = new Pipeline();
            pipe.Start(cfg);


            int n = 0;
            while (true)
            {
                using (var frames = pipe.WaitForFrames())
                {
                    n++;
                    Align align = new Align(Intel.RealSense.Stream.Color).DisposeWith(frames);
                    Frame aligned = align.Process(frames).DisposeWith(frames);
                    FrameSet alignedframeset = aligned.As<FrameSet>().DisposeWith(frames);
                    var colorFrame = alignedframeset.ColorFrame.DisposeWith(alignedframeset);
                    var depthFrame = alignedframeset.DepthFrame.DisposeWith(alignedframeset);

                    colorFrame.CopyTo(colorArray);
                    depthFrame.CopyTo(depthArray);

                    int mid = (colorArray.Length + 1) / 2;
                    byte[] firstHalf = new byte[mid];
                    byte[] secondHalf = new byte[colorArray.Length - mid];
                    Array.Copy(colorArray, 0, firstHalf, 0, mid);
                    Array.Copy(colorArray, mid, secondHalf, 0, secondHalf.Length);

                    int mid2 = (depthArray.Length + 1) / 2;
                    float[] firstHalf2 = new float[mid2];
                    float[] secondHalf2 = new float[depthArray.Length - mid2];
                    Array.Copy(depthArray, 0, firstHalf2, 0, mid2);
                    Array.Copy(depthArray, mid2, secondHalf2, 0, secondHalf2.Length);

                    debugCam = $" PointCloud: {n}";
                    sample1.SetValue("Index", n);
                    sample1.SetValue("Color", firstHalf);
                    writer.Write(sample1);
                    Thread.Sleep(30);
                    sample1.SetValue("Index", n);
                    sample1.SetValue("Color", secondHalf);
                    writer.Write(sample1);
                    Thread.Sleep(30);
                    sample2.SetValue("Index", n);
                    sample2.SetValue("Values", firstHalf2);
                    writer2.Write(sample1);
                    Thread.Sleep(30);
                    sample2.SetValue("Index", n);
                    sample2.SetValue("Values", secondHalf2);
                    writer2.Write(sample1);
                    Thread.Sleep(30);
                }               
            }
        }

    }
}
