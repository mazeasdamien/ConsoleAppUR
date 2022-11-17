using Rti.Dds.Subscription;
using Rti.Types.Dynamic;

namespace ConsoleAppUR
{
    public class TeleopSubscriber : Program
    {
        public static string debugTeleop = "";
        public static void RunSubscriber()
        {
            var typeFactory = DynamicTypeFactory.Instance;

            StructType UnityTCP = typeFactory.BuildStruct()
               .WithName("TCPunityPose")
               .AddMember(new StructMember("J1", typeFactory.GetPrimitiveType<double>()))
               .AddMember(new StructMember("J2", typeFactory.GetPrimitiveType<double>()))
               .AddMember(new StructMember("J3", typeFactory.GetPrimitiveType<double>()))
               .AddMember(new StructMember("J4", typeFactory.GetPrimitiveType<double>()))
               .AddMember(new StructMember("J5", typeFactory.GetPrimitiveType<double>()))
               .AddMember(new StructMember("J6", typeFactory.GetPrimitiveType<double>()))
               .Create();

            DataReader<DynamicData> reader = SetupDataReader("UnityTCP_Topic", subscriber, UnityTCP);

            int n = 1;
            while (true)
            {
                using var samples = reader.Take();
                foreach (var sample in samples)
                {
                    if (sample.Info.ValidData)
                    {
                        DynamicData data = sample.Data;
                        double J1 = data.GetValue<double>("J1");
                        double J2 = data.GetValue<double>("J2");
                        double J3 = data.GetValue<double>("J3");
                        double J4 = data.GetValue<double>("J4");
                        double J5 = data.GetValue<double>("J5");
                        double J6 = data.GetValue<double>("J6");

                        debugTeleop = $" Sample TCP from unity {n}:           \n" +
                             $"X: {Math.Round(J1,2)}                           \n" +
                             $"Y: {Math.Round(J2,2)}                            \n" +
                             $"Z: {Math.Round(J3,2)}                             \n" +
                             $"RX: {Math.Round(J4,2)}                             \n" +
                             $"RY: {Math.Round(J5,2)}                              \n" +
                             $"RZ: {Math.Round(J6,2)}                               \n\n";
                        n++;                                                   
                        UrInputs.input_double_register_20 = J1;
                        UrInputs.input_double_register_21 = J2;
                        UrInputs.input_double_register_22 = J3;
                        UrInputs.input_double_register_23 = J4;
                        UrInputs.input_double_register_24 = J5;
                        UrInputs.input_double_register_25 = J6;
                    }
                }
            }
        }
    }
}
