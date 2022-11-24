using ConsoleAppUR.PUB;
using ConsoleAppUR.SUB;
using ConsoleAppUR.URrobot;
using Intel.RealSense;
using Rti.Dds.Core;
using Rti.Dds.Domain;
using Rti.Dds.Publication;
using Rti.Dds.Subscription;
using Rti.Types.Dynamic;
using System.Drawing;

namespace ConsoleAppUR
{
    public class Program
    {
        public static DomainParticipant domainParticipant = null!;
        public static QosProvider provider = null!;
        public static Publisher Publisher_UR = null!;
        public static Subscriber Subscriber_UR = null!;

        private static Thread PUB_RobotState = null!;
        private static Thread SUB_Teleop = null!;
        private static Thread PUB_CameraColorTopicPublisher = null!;
        private static Thread PUB_CameraDepthTopicPublisher = null!;
        private static Thread ConsoleDebug = null!;
        private static Thread PUB_VideoFeed = null!;
        private static Thread Run_Camera = null!;

        public static UniversalRobot_Outputs UrOutputs = new UniversalRobot_Outputs();
        public static UniversalRobot_Inputs UrInputs = new UniversalRobot_Inputs();

        public static RtdeClient Ur3 = new RtdeClient();
        public static string IPadress = null!;

        public static byte[] colorData;

        public static List<float> DEPTHDATA = new List<float>();
        public static List<byte> COLORDATA = new List<byte>();

        public static bool ptsentColor;
        public static bool ptsentDepth;

        static void Main()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.CancelKeyPress += delegate
            {
                OnDestroy();
            };

            var showMenu1 = true;
            ptsentColor = false;
            ptsentDepth = false;

            while (showMenu1)
                showMenu1 = TriggerMenu();

            ///////////////////////////////////////////////////////////

            InitDDS();
            InitRobotUR16e();
            InitThreads();
            Console.Clear();
        }

        public static void InitThreads()
        {
            Console.WriteLine("Init Threads");
            Run_Camera = new(() => RunCamera.RunCam());
            PUB_RobotState = new(() => UR16eDataPublisher.RunPublisher());
            SUB_Teleop = new(() => UnityIKSolutionSubscriber.RunSubscriber());
            ConsoleDebug = new(() => consoleDebugPrinter.UpdateConsole());
            PUB_CameraDepthTopicPublisher = new(() => IntelRealSenseDataPublisher.RunPublisher());
            PUB_CameraColorTopicPublisher = new(() => IntelRealSenseColor.RunPublisher());
            PUB_VideoFeed = new(() => VideoFeedPublisher.RunPublisher());
            Run_Camera.Start();
            PUB_CameraDepthTopicPublisher.Start();
            PUB_CameraColorTopicPublisher.Start();
            ConsoleDebug.Start();
            PUB_RobotState.Start();
            SUB_Teleop.Start();
            PUB_VideoFeed.Start();
        }
        public static void InitRobotUR16e()
        {
            Console.WriteLine("Init Robot UR16e");

            Ur3.Connect(IPadress, 2);

            Ur3.Setup_Ur_Inputs(UrInputs);
            Ur3.Setup_Ur_Outputs(UrOutputs, 150);
            Ur3.Ur_ControlStart();

            UrInputs.input_double_register_20 = Math.PI / 180 * -96;
            UrInputs.input_double_register_21 = Math.PI / 180 * -60;
            UrInputs.input_double_register_22 = Math.PI / 180 * -90;
            UrInputs.input_double_register_23 = Math.PI / 180 * -110;
            UrInputs.input_double_register_24 = Math.PI / 180 * 90;
            UrInputs.input_double_register_25 = Math.PI / 180 * 0;

            // start program
            Console.WriteLine("Start program");

            var m = "def unity():\n" +
"  set_tcp(p[0,0,0.1493,0,0,0])\n" +
" while (True):\n" +
"  new_pose = [read_input_float_register(20), read_input_float_register(21), read_input_float_register(22), read_input_float_register(23), read_input_float_register(24), read_input_float_register(25)]\n" +
"  servoj(new_pose, t = 0.02, lookahead_time = 1, gain = 350)\n" +
"  sync()\n" +
" end\n" +
"end\n";

            RtdeClient.URscriptCommand(IPadress, m);
        }
        public static void InitDDS()
        {
            Console.WriteLine("Init DDS");
            provider = new QosProvider("URGrenoble.xml");
            domainParticipant = DomainParticipantFactory.Instance.CreateParticipant(0, provider.GetDomainParticipantQos());
            PublisherQos publisherQos = provider.GetPublisherQos("QosURGrenoble::QosProfile");
            Publisher_UR = domainParticipant.CreatePublisher(publisherQos);
            SubscriberQos subscriberQos = provider.GetSubscriberQos("QosURGrenoble::QosProfile");
            Subscriber_UR = domainParticipant.CreateSubscriber(subscriberQos);
        }
        public static void OnDestroy()
        {
            Console.WriteLine("stop");
            PUB_CameraColorTopicPublisher.Interrupt();
            PUB_CameraColorTopicPublisher.Interrupt();
            PUB_VideoFeed.Interrupt();
            ConsoleDebug.Interrupt();
            PUB_RobotState.Interrupt();
            SUB_Teleop.Interrupt();
            Ur3.Disconnect();
            Publisher_UR.Dispose();
            Subscriber_UR.Dispose();
            domainParticipant.Dispose();
            provider.Dispose();
            Environment.Exit(0);
        }
        public static DataWriter<DynamicData> SetupDataWriter(string topicName, Publisher publisher, DynamicType dynamicData)
        {
            DataWriter<DynamicData> writer;
            var topic = domainParticipant.CreateTopic(topicName, dynamicData);
            var writerQos = provider.GetDataWriterQos("QosURGrenoble::QosProfile");
            writer = publisher.CreateDataWriter(topic, writerQos);
            return writer;
        }
        public static DataReader<DynamicData> SetupDataReader(string topicName, Subscriber subscriber, DynamicType dynamicData)
        {
            DataReader<DynamicData> reader;
            var topic = domainParticipant.CreateTopic(topicName, dynamicData);
            var readerQos = provider.GetDataReaderQos("QosURGrenoble::QosProfile");
            reader = subscriber.CreateDataReader(topic, readerQos);
            return reader;
        }
        static bool TriggerMenu()
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