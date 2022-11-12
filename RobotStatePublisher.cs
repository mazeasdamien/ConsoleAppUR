using Rti.Dds.Publication;
using Rti.Types.Dynamic;
using System.Diagnostics;
using System.Net.Sockets;

namespace ConsoleAppUR
{
    internal class RobotStatePublisher : Program
    {
        public static void RunPublisher()
        {
            var typeFactory = DynamicTypeFactory.Instance;
            StructType RobotState = typeFactory.BuildStruct()
                .WithName("RobotState")
                .AddMember(new StructMember("J1", typeFactory.GetPrimitiveType<double>()))
                .AddMember(new StructMember("J2", typeFactory.GetPrimitiveType<double>()))
                .AddMember(new StructMember("J3", typeFactory.GetPrimitiveType<double>()))
                .AddMember(new StructMember("J4", typeFactory.GetPrimitiveType<double>()))
                .AddMember(new StructMember("J5", typeFactory.GetPrimitiveType<double>()))
                .AddMember(new StructMember("J6", typeFactory.GetPrimitiveType<double>()))
                .Create();

            DataWriter<DynamicData> writer = SetupDataWriter("RobotState_Topic", publisher, RobotState);
            var sample = new DynamicData(RobotState);

            List<double> tempJoints = new();
            tempJoints.Add(6);

            int n = 0;
            //
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(UrOutputs.input_bit_register_64);

                sample.SetValue(memberName: "J1", UrOutputs.actual_q[0]);
                sample.SetValue(memberName: "J2", UrOutputs.actual_q[1]);
                sample.SetValue(memberName: "J3", UrOutputs.actual_q[2]);
                sample.SetValue(memberName: "J4", UrOutputs.actual_q[3]);
                sample.SetValue(memberName: "J5", UrOutputs.actual_q[4]);
                sample.SetValue(memberName: "J6", UrOutputs.actual_q[5]);

                if (tempJoints[0] != UrOutputs.actual_q[0] ||
                    tempJoints[1] != UrOutputs.actual_q[1] ||
                    tempJoints[2] != UrOutputs.actual_q[2] ||
                    tempJoints[3] != UrOutputs.actual_q[3] ||
                    tempJoints[4] != UrOutputs.actual_q[4] ||
                    tempJoints[5] != UrOutputs.actual_q[5])
                {
                    writer.Write(sample);
                    n++;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Joints value sent " +n);
                tempJoints.Clear();
                    tempJoints.Add(UrOutputs.actual_q[0]);
                    tempJoints.Add(UrOutputs.actual_q[1]);
                    tempJoints.Add(UrOutputs.actual_q[2]);
                    tempJoints.Add(UrOutputs.actual_q[3]);
                    tempJoints.Add(UrOutputs.actual_q[4]);
                    tempJoints.Add(UrOutputs.actual_q[5]);
                }

            }
        }
    }
}
