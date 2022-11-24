using Rti.Dds.Publication;
using Rti.Types.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppUR.PUB
{
    internal class IntelRealSenseColor : Program
    {
        public static string debugCam = "";

        public static void RunPublisher()
        {
            var typeFactory = DynamicTypeFactory.Instance;

            var CameraColorTopic = typeFactory.BuildStruct()
                .WithName("CameraColorTopic")
                .AddMember(new StructMember("Index", typeFactory.GetPrimitiveType<int>()))
                .AddMember(new StructMember("Color", typeFactory.CreateSequence(typeFactory.GetPrimitiveType<byte>(), 1000000)))
                .Create();

            var writer = SetupDataWriter("CameraColorTopic", Publisher_UR, CameraColorTopic);
            var sample = new DynamicData(CameraColorTopic);

            var n = 0;

            while (true)
            {
                if (ptsentColor == true)
                {
                    n++;
                    sample.SetValue("Index", n);
                    sample.SetValue("Color", COLORDATA.ToArray());

                    debugCam =$" {n}  Color size {COLORDATA.ToArray().Length}                                 \n";

                    if (COLORDATA.ToArray().Length > 1000)
                    {
                        writer.Write(sample);
                    }

                    COLORDATA.Clear();
                    ptsentColor = false;
                }
            }
        }
    }
}
