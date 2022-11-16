using Rti.Dds.Publication;
using Rti.Types.Dynamic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Sockets;

namespace ConsoleAppUR
{
    internal class RobotStatePublisher : Program
    {
        public const float Rad2Deg = 57.29578f;
        public static string debugRobotState = "";
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
                .AddMember(new StructMember("X", typeFactory.GetPrimitiveType<double>()))
                .AddMember(new StructMember("Y", typeFactory.GetPrimitiveType<double>()))
                .AddMember(new StructMember("Z", typeFactory.GetPrimitiveType<double>()))
                .AddMember(new StructMember("RX", typeFactory.GetPrimitiveType<double>()))
                .AddMember(new StructMember("RY", typeFactory.GetPrimitiveType<double>()))
                .AddMember(new StructMember("RZ", typeFactory.GetPrimitiveType<double>()))
                .Create();

            DataWriter<DynamicData> writer = SetupDataWriter("RobotState_Topic", publisher, RobotState);
            var sample = new DynamicData(RobotState);
            
            int n = 1;
            double[] robotValue = new double[12];

            while (true)
            {
                sample.SetValue("J1", UrOutputs.actual_q[0]);
                sample.SetValue("J2", UrOutputs.actual_q[1]);
                sample.SetValue("J3", UrOutputs.actual_q[2]);
                sample.SetValue("J4", UrOutputs.actual_q[3]);
                sample.SetValue("J5", UrOutputs.actual_q[4]);
                sample.SetValue("J6", UrOutputs.actual_q[5]);
                sample.SetValue("X", UrOutputs.actual_TCP_pose[0]);
                sample.SetValue("Y", UrOutputs.actual_TCP_pose[1]);
                sample.SetValue("Z", UrOutputs.actual_TCP_pose[2]);
                sample.SetValue("RX", UrOutputs.actual_TCP_pose[3]);
                sample.SetValue("RZ", UrOutputs.actual_TCP_pose[4]);
                sample.SetValue("RY", UrOutputs.actual_TCP_pose[5]);

                if (robotValue[0] != UrOutputs.actual_q[0] ||
                    robotValue[1] != UrOutputs.actual_q[1] ||
                    robotValue[2] != UrOutputs.actual_q[2] ||
                    robotValue[3] != UrOutputs.actual_q[3] ||
                    robotValue[4] != UrOutputs.actual_q[4] ||
                    robotValue[5] != UrOutputs.actual_q[5])
                {
                    robotValue[0] = UrOutputs.actual_q[0];
                    robotValue[1] = UrOutputs.actual_q[1];
                    robotValue[2] = UrOutputs.actual_q[2];
                    robotValue[3] = UrOutputs.actual_q[3];
                    robotValue[4] = UrOutputs.actual_q[4];
                    robotValue[5] = UrOutputs.actual_q[5];
                    robotValue[6] = UrOutputs.actual_TCP_pose[0];
                    robotValue[7] = UrOutputs.actual_TCP_pose[1];
                    robotValue[8] = UrOutputs.actual_TCP_pose[2];
                    robotValue[9] = UrOutputs.actual_TCP_pose[3];
                    robotValue[10] = UrOutputs.actual_TCP_pose[4];
                    robotValue[11] = UrOutputs.actual_TCP_pose[5];

                    debugRobotState = $" Sample data UR stream {n}:   \n" +
                        $"J1: {Math.Round(UrOutputs.actual_q[0] * Rad2Deg, 2)}   \n" +
                        $"J2: {Math.Round(UrOutputs.actual_q[1] * Rad2Deg, 2)}   \n" +
                        $"J3: {Math.Round(UrOutputs.actual_q[2] * Rad2Deg, 2)}   \n" +
                        $"J4: {Math.Round(UrOutputs.actual_q[3] * Rad2Deg, 2)}   \n" +
                        $"J5: {Math.Round(UrOutputs.actual_q[4] * Rad2Deg, 2)}   \n" +
                        $"J6: {Math.Round(UrOutputs.actual_q[5] * Rad2Deg, 2)}   \n\n" +
                        $"X: {Math.Round(UrOutputs.actual_TCP_pose[0], 3)}   \n" +
                        $"Y: {Math.Round(UrOutputs.actual_TCP_pose[1], 3)}   \n" +
                        $"Z: {Math.Round(UrOutputs.actual_TCP_pose[2], 3)}   \n" +
                        $"RX: {Math.Round(UrOutputs.actual_TCP_pose[3], 3)}   \n" +
                        $"RY: {Math.Round(UrOutputs.actual_TCP_pose[4], 3)}   \n" +
                        $"RZ: {Math.Round(UrOutputs.actual_TCP_pose[5], 3)}   \n\n";
                    n++;
                    writer.Write(sample);
                }
                Thread.Sleep(2);
            }
        }
    }
}
