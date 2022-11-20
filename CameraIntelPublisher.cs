using Rti.Dds.Publication;
using Rti.Types.Dynamic;

namespace ConsoleAppUR
{
    internal class CameraIntelPublisher : Program
    {
        public static string debugCam = "";

        public static void RunPublisher()
        {
            var typeFactory = DynamicTypeFactory.Instance;
            StructType Camera = typeFactory.BuildStruct()
                .WithName("Video")
                .AddMember(new StructMember("Index", typeFactory.GetPrimitiveType<int>()))
                .AddMember(new StructMember("Color", typeFactory.CreateSequence(typeFactory.GetPrimitiveType<byte>(), 922000)))
                .AddMember(new StructMember("Depth", typeFactory.CreateSequence(typeFactory.GetPrimitiveType<float>(), 308000)))
                .Create();

            DataWriter<DynamicData> writer = SetupDataWriter("Camera1_Topic", publisher, Camera);
            var sample = new DynamicData(Camera);

            int n = 0;
            while (true)
            {
                if (ptsent == true)
                {
                    n++;
                    sample.SetValue("Index", n);
                    sample.SetValue("Color", colorArray);
                    sample.SetValue("Depth", depthArray.ToArray());
                    debugCam = $" {n}    {colorArray.Length}   {depthArray.ToArray().Length}     ";
                    writer.Write(sample);
                    ptsent = false;
                }
            }
        }
    }
}
