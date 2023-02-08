using Rti.Types.Dynamic;

namespace ConsoleAppUR.PUB
{
    internal class UR16eDataSimuPublisher : Program
    {
        public const float Rad2Deg = 57.29578f;
        public static void RunPublisher()
        {
            var typeFactory = DynamicTypeFactory.Instance;

            var RobotStateTopic = typeFactory.BuildStruct()
                .WithName("RobotStateSimuTopic")
                .AddMember(new StructMember("J1", typeFactory.GetPrimitiveType<double>()))
                .AddMember(new StructMember("J2", typeFactory.GetPrimitiveType<double>()))
                .AddMember(new StructMember("J3", typeFactory.GetPrimitiveType<double>()))
                .AddMember(new StructMember("J4", typeFactory.GetPrimitiveType<double>()))
                .AddMember(new StructMember("J5", typeFactory.GetPrimitiveType<double>()))
                .AddMember(new StructMember("J6", typeFactory.GetPrimitiveType<double>()))
                .Create();

            var writer = SetupDataWriter("RobotStateSimuTopic", Publisher_UR, RobotStateTopic);
            var sample = new DynamicData(RobotStateTopic);

            var robotValue = new double[12];

            while (true)
            {
                sample.SetValue("J1",sharedJ1);
                sample.SetValue("J2",sharedJ2);
                sample.SetValue("J3",sharedJ3);
                sample.SetValue("J4",sharedJ4);
                sample.SetValue("J5",sharedJ5);
                sample.SetValue("J6", sharedJ6);

                if (robotValue[0] != sharedJ1 ||
                    robotValue[1] != sharedJ2 ||
                    robotValue[2] != sharedJ3 ||
                    robotValue[3] != sharedJ4 ||
                    robotValue[4] != sharedJ5 ||
                    robotValue[5] != sharedJ6)
                {
                    robotValue[0] = sharedJ1;
                    robotValue[1] = sharedJ2;
                    robotValue[2] = sharedJ3;
                    robotValue[3] = sharedJ4;
                    robotValue[4] = sharedJ5;
                    robotValue[5] = sharedJ6;

                    writer.Write(sample);
                }

                Thread.Sleep(2);
            }
        }
    }
}