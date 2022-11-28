using K4os.Compression.LZ4;
using Rti.Types.Dynamic;

namespace ConsoleAppUR.PUB
{
    internal class IntelRealSenseDataPublisher : Program
    {
        public static string debugCam = "";

        public static void RunPublisher()
        {
            var typeFactory = DynamicTypeFactory.Instance;

            var CameraDepthTopic = typeFactory.BuildStruct()
                .WithName("CameraDepthTopic")
                .AddMember(new StructMember("Index", typeFactory.GetPrimitiveType<int>()))
                .AddMember(new StructMember("Depth", typeFactory.CreateSequence(typeFactory.GetPrimitiveType<float>(), 2000000)))
                .Create();

            var writer = SetupDataWriter("CameraDepthTopic", Publisher_UR, CameraDepthTopic);
            var sample = new DynamicData(CameraDepthTopic);

            var n = 0;

            while (true)
            {
                if (ptsentDepth == true)
                {
                    n++;
                    sample.SetValue("Index", n);

                    sample.SetValue("Depth", DEPTHDATA.ToArray());

                    debugCam = $" {n}  Depth size  {DEPTHDATA.ToArray().Length}                      \n";

                    if (DEPTHDATA.ToArray().Length > 1000)
                    {
                        writer.Write(sample);
                    }

                    DEPTHDATA.Clear();
                    ptsentDepth = false;
                }
            }
        }
    }
}