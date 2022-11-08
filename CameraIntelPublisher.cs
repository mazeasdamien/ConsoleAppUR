using Intel.RealSense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppUR
{
    internal class CameraIntelPublisher : Program
    {
        public static void RunPublisher()
        {

            var pipe = new Pipeline();
            pipe.Start();

            while (true)
            {
                using (var frames = pipe.WaitForFrames())
                using (var depth = frames.DepthFrame)
                {
                    Console.WriteLine("The camera is pointing at an object " +
                        depth.GetDistance(depth.Width / 2, depth.Height / 2) + " meters away\t");

                    Console.SetCursorPosition(0, 0);
                }
            }
        }
    }
}
