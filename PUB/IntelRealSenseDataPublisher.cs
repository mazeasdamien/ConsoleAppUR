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
                .AddMember(new StructMember("Depth", typeFactory.CreateSequence(typeFactory.GetPrimitiveType<byte>(), 2000000)))
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

                    var byteArray = new byte[DEPTHDATA.ToArray().Length * 4];
                    Buffer.BlockCopy(DEPTHDATA.ToArray(), 0, byteArray, 0, byteArray.Length);
                    sample.SetValue("Depth", byteArray);

                    debugCam = $" {n}  Depth size  {byteArray.Length}                                   \n";

                    if (byteArray.Length > 1000)
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