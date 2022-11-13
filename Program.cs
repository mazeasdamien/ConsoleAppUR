using ConsoleAppUR;
using Rti.Dds.Core;
using Rti.Dds.Domain;
using Rti.Dds.Publication;
using Rti.Dds.Subscription;
using Rti.Dds.Topics;
using Rti.Types.Dynamic;
using System;

namespace ConsoleAppUR
{
    public class Program
    {
        public static DomainParticipant domainParticipant = null!;
        public static QosProvider provider = null!;
        public static Publisher publisher = null!;
        public static Subscriber subscriber = null!;

        public static Thread PUB_RobotState = null!;
        public static Thread SUB_Teleop = null!;
        public static Thread PUB_CameraRS = null!;

        public static UniversalRobot_Outputs UrOutputs = new UniversalRobot_Outputs();
        public static UniversalRobot_Inputs UrInputs = new UniversalRobot_Inputs();

        public static string IPadress = null!;

        static void Main()
        {
            bool showMenu1 = true;
            while (showMenu1)
            {
                showMenu1 = Menu1();
            }

            //DDS stuff
            provider = new QosProvider("TelexistenceRig.xml");
            domainParticipant = DomainParticipantFactory.Instance.CreateParticipant(0, provider.GetDomainParticipantQos());
            var publisherQos = provider.GetPublisherQos("RigQoSLibrary::RigQoSProfile");
            publisher = domainParticipant.CreatePublisher(publisherQos);
            var subscriberQos = provider.GetSubscriberQos("RigQoSLibrary::RigQoSProfile");
            subscriber = domainParticipant.CreateSubscriber(subscriberQos);

            //Robot stuff
            RtdeClient Ur3 = new RtdeClient();
            Ur3.Connect(IPadress, 2);

            // setup
            Ur3.Setup_Ur_Inputs(UrInputs);
            Ur3.Setup_Ur_Outputs(UrOutputs, 125);
            Ur3.Ur_ControlStart();



            UrInputs.input_double_register_20 = Math.PI / 180 * - 96;
            UrInputs.input_double_register_21 = Math.PI / 180  * - 60;
            UrInputs.input_double_register_22 = Math.PI / 180 * - 90;
            UrInputs.input_double_register_23 = Math.PI / 180 * - 110;
            UrInputs.input_double_register_24 = Math.PI / 180 * 90;
            UrInputs.input_double_register_25 = Math.PI / 180 * 0 ;

            //start program
            Console.WriteLine("Start program");
            string m = "def unity():\n" +
"  set_tcp(p[0,0,0.1493,0,0,0])\n" +
" while (True):\n" +
"  write_output_boolean_register(90, False)\n" +
"  new_pose = [read_input_float_register(20), read_input_float_register(21), read_input_float_register(22), read_input_float_register(23), read_input_float_register(24), read_input_float_register(25)]\n" +
"  servoj(new_pose, t = 0.5, lookahead_time = 0.8, gain = 350)\n" +
"  sync()\n" +
" end\n" +
"end\n";
            RtdeClient.URscriptCommand(IPadress, m);

            //Control Threads
            PUB_RobotState = new(() => RobotStatePublisher.RunPublisher());
            SUB_Teleop = new(() => TeleopSubscriber.RunSubscriber());
            //PUB_CameraRS = new(() => CameraIntelPublisher.RunPublisher());
            //PUB_CameraRS.Start();
            PUB_RobotState.Start();
            SUB_Teleop.Start();
        }

        public static DataWriter<DynamicData> SetupDataWriter(string topicName, Publisher publisher, DynamicType dynamicData)
        {
            DataWriter<DynamicData> writer;
            Topic<DynamicData> topic = domainParticipant.CreateTopic(topicName, dynamicData);
            var writerQos = provider.GetDataWriterQos("RigQoSLibrary::RigQoSProfile");
            writer = publisher.CreateDataWriter(topic, writerQos);
            return writer;
        }

        public static DataReader<DynamicData> SetupDataReader(string topicName, Subscriber subscriber, DynamicType dynamicData)
        {
            DataReader<DynamicData> reader;
            Topic<DynamicData> topic = domainParticipant.CreateTopic(topicName, dynamicData);
            var readerQos = provider.GetDataReaderQos("RigQoSLibrary::RigQoSProfile");
            reader = subscriber.CreateDataReader(topic, readerQos);
            return reader;
        }

        private static bool Menu1()
        {
            Console.Clear();
            Console.WriteLine("1) Simulation UR16e");
            Console.WriteLine("2) Physical Robot UR16e");
            Console.Write("\r\nSelect an option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    IPadress = "192.168.56.103";
                    return false;
                case "2":
                    IPadress = "169.254.56.120";
                    return false;
                default:
                    return true;
            }
        }
    }
}