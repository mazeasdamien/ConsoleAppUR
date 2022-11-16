namespace ConsoleAppUR
{
    internal class consoleDebug : Program
    {
        public static void UpdateConsole()
        {
            Console.Title = "UR16e Remote controll";
            Console.SetWindowSize(40, 40);
            while (true)
            {
                Console.SetCursorPosition(0,0);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(RobotStatePublisher.debugRobotState);
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(TeleopSubscriber.debugTeleop);
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(CameraIntelPublisher.debugCam);
            }
        }
    }
}
