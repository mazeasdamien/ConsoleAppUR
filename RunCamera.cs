using Intel.RealSense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppUR
{
    internal class RunCamera : Program
    {
        public static void RunCam()
        {

            Config cfg = new();
            cfg.EnableStream(Intel.RealSense.Stream.Depth, 640, 480, Format.Z16, 30);
            cfg.EnableStream(Intel.RealSense.Stream.Color, 640, 480, Format.Rgb8, 30);
            Pipeline pipe = new();
            PointCloud pc = new();
            HoleFillingFilter holeFillingFilter = new();
            pipe.Start(cfg);

            while (true)
            {
                if (ptsentColor == false && ptsentDepth == false)
                {
                    FrameSet frames;

                    using (frames = pipe.WaitForFrames())
                    {
                        frames = holeFillingFilter.Process<FrameSet>(frames).DisposeWith(frames);
                        Align align = new Align(Intel.RealSense.Stream.Color).DisposeWith(frames);
                        frames = align.Process<FrameSet>(frames).DisposeWith(frames);

                        DepthFrame depthFrame = frames.DepthFrame.DisposeWith(frames);
                        VideoFrame colorFrame = frames.ColorFrame.DisposeWith(frames);
                        colorData = new byte[colorFrame.Stride * colorFrame.Height];
                        colorFrame.CopyTo(colorData);

                        Points pts = pc.Process<Points>(depthFrame).DisposeWith(frames);
                        float[] vertices = new float[pts.Count * 3];
                        pts.CopyVertices(vertices);


                        for (int i = 0; i < vertices.Length; i += 3)
                        {
                            if (i % 2 == 0)
                            {
                                if (vertices[i + 2] < 1)
                                {
                                    DEPTHDATA.Add(vertices[i]);
                                    DEPTHDATA.Add(vertices[i + 1]);
                                    DEPTHDATA.Add(vertices[i + 2]);
                                    COLORDATA.Add(colorData[i]);
                                    COLORDATA.Add(colorData[i + 1]);
                                    COLORDATA.Add(colorData[i + 2]);
                                }
                            }
                        }

                        ptsentColor = true;
                        ptsentDepth = true;
                        Thread.Sleep(50);
                    }
                }
            }
        }
    }
}
