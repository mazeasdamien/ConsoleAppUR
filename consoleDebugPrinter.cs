using ConsoleAppUR.PUB;
using ConsoleAppUR.SUB;

namespace ConsoleAppUR
{
    internal class consoleDebugPrinter : Program
    {
        public static void UpdateConsole()
        {
            Console.Title = "UR16e Remote controll";

            while (true)
            {
                Console.SetCursorPosition(0, 0);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(UR16eDataPublisher.debugRobotState);
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(UnityIKSolutionSubscriber.debugTeleop);
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(IntelRealSenseDataPublisher.debugCam + IntelRealSenseColor.debugCam);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(VideoFeedPublisher.debugCam);
            }
        }
    }
}