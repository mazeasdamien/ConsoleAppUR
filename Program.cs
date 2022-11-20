﻿using Intel.RealSense;
using Rti.Dds.Core;
using Rti.Dds.Domain;
using Rti.Dds.Publication;
using Rti.Dds.Subscription;
using Rti.Dds.Topics;
using Rti.Types.Dynamic;
using System.ComponentModel;

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
        public static Thread ConsoleDebug = null!;

        public static UniversalRobot_Outputs UrOutputs = new UniversalRobot_Outputs();
        public static UniversalRobot_Inputs UrInputs = new UniversalRobot_Inputs();

        public static RtdeClient Ur3 = new RtdeClient();
        public static string IPadress = null!;

        public const int CAMERA_WIDTH = 640;
        public const int CAMERA_HEIGHT = 480;

        public const int FPS = 30;

        public static byte[] colorArray = new byte[CAMERA_WIDTH * CAMERA_HEIGHT * 3];
        public static List<float> depthArray = new List<float>();

        public static bool ptsent;

        static void Main()
        {
            Console.CancelKeyPress += delegate {
                OnDestroy();
            };

            bool showMenu1 = true;
            ptsent = false;
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
            Ur3.Connect(IPadress, 2);

            // setup
            Ur3.Setup_Ur_Inputs(UrInputs);
            Ur3.Setup_Ur_Outputs(UrOutputs, 150);
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
"  new_pose = [read_input_float_register(20), read_input_float_register(21), read_input_float_register(22), read_input_float_register(23), read_input_float_register(24), read_input_float_register(25)]\n" +
"  servoj(new_pose, t = 0.02, lookahead_time = 1, gain = 350)\n" +
"  sync()\n" +
" end\n" +
"end\n";
            RtdeClient.URscriptCommand(IPadress, m);
            Console.Clear();

            //Control Threads
            PUB_RobotState = new(() => RobotStatePublisher.RunPublisher());
            SUB_Teleop = new(() => TeleopSubscriber.RunSubscriber());
            ConsoleDebug = new(() => consoleDebug.UpdateConsole());
            PUB_CameraRS = new(() => CameraIntelPublisher.RunPublisher());
            PUB_CameraRS.Start();
            ConsoleDebug.Start();
            PUB_RobotState.Start();
            SUB_Teleop.Start();

            var cfg = new Config();
            cfg.EnableStream(Intel.RealSense.Stream.Depth, CAMERA_WIDTH, CAMERA_HEIGHT, Format.Z16, FPS);
            cfg.EnableStream(Intel.RealSense.Stream.Color, CAMERA_WIDTH, CAMERA_HEIGHT, Format.Rgb8, FPS);
            var pipe = new Pipeline();
            var pc = new PointCloud();
            pipe.Start(cfg);
            
            while (true)
            {
                if (ptsent == false)
                {
                    depthArray.Clear();

                    using (var frames = pipe.WaitForFrames())
                    using (var depth = frames.DepthFrame)
                    using (var color = frames.ColorFrame)
                    {
                        Align align = new Align(Intel.RealSense.Stream.Color).DisposeWith(frames);
                        Frame aligned = align.Process(frames).DisposeWith(frames);
                        FrameSet alignedframeset = aligned.As<FrameSet>().DisposeWith(frames);
                        var colorFrame = alignedframeset.ColorFrame.DisposeWith(alignedframeset);
                        colorFrame.CopyTo(colorArray);
                        
                        for (int x = 0; x < CAMERA_WIDTH - 1; x++)
                        {
                            for (int y = 0; y < CAMERA_HEIGHT - 1; y++)
                            {
                                depthArray.Add(depth.GetDistance(x, y));
                            }
                        }
                        ptsent = true;
                    }
                }
            }
        }

        public static void OnDestroy()
        {
            Console.WriteLine("stop");
            Ur3.Disconnect();
            publisher.Dispose();
            subscriber.Dispose();
            domainParticipant.Dispose();
            provider.Dispose();
            PUB_CameraRS.Interrupt();
            ConsoleDebug.Interrupt();
            PUB_RobotState.Interrupt();
            SUB_Teleop.Interrupt();
            Environment.Exit(0);
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