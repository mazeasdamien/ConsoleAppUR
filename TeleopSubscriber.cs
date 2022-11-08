using Rti.Dds.Subscription;
using Rti.Types.Dynamic;

namespace ConsoleAppUR
{
    public class TeleopSubscriber : Program
    {
        public static void RunSubscriber()
        {
            var typeFactory = DynamicTypeFactory.Instance;

            StructType OperatorNewPose = typeFactory.BuildStruct()
               .WithName("OperatorNewPose")
               .AddMember(new StructMember("J1", typeFactory.GetPrimitiveType<double>()))
               .AddMember(new StructMember("J2", typeFactory.GetPrimitiveType<double>()))
               .AddMember(new StructMember("J3", typeFactory.GetPrimitiveType<double>()))
               .AddMember(new StructMember("J4", typeFactory.GetPrimitiveType<double>()))
               .AddMember(new StructMember("J5", typeFactory.GetPrimitiveType<double>()))
               .AddMember(new StructMember("J6", typeFactory.GetPrimitiveType<double>()))
               .Create();

            DataReader<DynamicData> reader = SetupDataReader("Teleop_Topic", subscriber, OperatorNewPose);

            int n = 0;
            while (true)
            {
                using var samples = reader.Take();
                foreach (var sample in samples)
                {
                    if (sample.Info.ValidData)
                    {
                        n++;
                        DynamicData data = sample.Data;
                        double j1 = data.GetValue<double>("J1");
                        double j2 = data.GetValue<double>("J2");
                        double j3 = data.GetValue<double>("J3");
                        double j4 = data.GetValue<double>("J4");
                        double j5 = data.GetValue<double>("J5");
                        double j6 = data.GetValue<double>("J6");                      
                          
                        UrInputs.input_double_register_20 = j1;
                        UrInputs.input_double_register_21 = j2;
                        UrInputs.input_double_register_22 = j3;
                        UrInputs.input_double_register_23 = j4;
                        UrInputs.input_double_register_24 = j5;
                        UrInputs.input_double_register_25 = j6;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("New joints values set " + n);
                        Thread.Sleep(8);
                    }
                }
            }
        }
    }
}
