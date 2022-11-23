using Rti.Dds.Publication;
using Rti.Types.Dynamic;
using UnityEditor.PackageManager.UI;

namespace ConsoleAppUR
{
    internal class CameraDepthTopicPublisher : Program
    {
        public static string debugCam = "";

        public static void RunPublisher()
        {
            var typeFactory = DynamicTypeFactory.Instance;
            StructType CameraDepthTopic = typeFactory.BuildStruct()
                .WithName("CameraDepthTopic")
                .AddMember(new StructMember("Index", typeFactory.GetPrimitiveType<int>()))
                .AddMember(new StructMember("Depth", typeFactory.CreateSequence(typeFactory.GetPrimitiveType<float>(), 1000000)))
                .AddMember(new StructMember("Color", typeFactory.CreateSequence(typeFactory.GetPrimitiveType<byte>(), 1000000)))
                .Create();

            DataWriter<DynamicData> writer = SetupDataWriter("CameraDepthTopic", publisher, CameraDepthTopic);
            var sample = new DynamicData(CameraDepthTopic);

            int n = 0;
            while (true)
            {
                if (ptsentDepth == true)
                {
                    n++;
                    sample.SetValue("Index", n);
                    sample.SetValue("Depth", DEPTHDATA.ToArray());
                    sample.SetValue("Color", COLORDATA.ToArray());
                    debugCam = $" {n}  Depth size  {COLORDATA.ToArray().Length}                      \n" +
                        $" {n}  Color size {COLORDATA.ToArray().Length}                                 \n";
                    if (COLORDATA.ToArray().Length > 1000 && DEPTHDATA.ToArray().Length > 1000)
                    {
                        writer.Write(sample);
                    }
                    DEPTHDATA.Clear();
                    COLORDATA.Clear();
                    ptsentDepth = false;
                    ptsentColor = false;
                }
            }
        }
    }
}
