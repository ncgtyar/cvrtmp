using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace CVRTMP
{
    public class Object
    {
        public string label;
        public int x;
        public int y;
        public int w;
        public int h;

        public Object(string _label, float _x, float _y, float _w, float _h)
        {
            label = _label;
            x = (int)_x;
            y = (int)_y;
            w = (int)_w;
            h = (int)_h;
        }
    }

    internal static class RTMP
    {
        internal static VideoCapture capture;
        internal static Mat frame = new Mat();

        internal static void open(string rtmpUrl)
        {
            capture = new VideoCapture(rtmpUrl, VideoCaptureAPIs.FFMPEG);

            if (!capture.IsOpened())
            {
                Console.WriteLine("[CVRTMP] Failed to open stream!");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }
    }

    internal static class CV
    {
        internal static Net net;
        internal static string[] labels = { };

        internal static void open(string modelPath, string[] classNames)
        {
            net = CvDnn.ReadNetFromOnnx(modelPath);
            net.SetPreferableBackend(Backend.DEFAULT);
            net.SetPreferableTarget(Target.CPU);
            labels = classNames;
        }

        internal static Object detect()
        {
            var blob = CvDnn.BlobFromImage(RTMP.frame, 1.0 / 255.0, new OpenCvSharp.Size(640, 640), new Scalar(), true, false);
            net.SetInput(blob);
            var output = net.Forward();
            float confidenceThreshold = 0.5F;
            for (int i = 0; i < output.Rows; i++)
            {
                float confidence = output.At<float>(i, 2);

                if (confidence < confidenceThreshold)
                    continue;

                int classId = (int)output.At<float>(i, 1);

                float x = output.At<float>(i, 3);
                float y = output.At<float>(i, 4);
                float w = output.At<float>(i, 5);
                float h = output.At<float>(i, 6);

                string label = classId >= 0 && classId < labels.Length ? labels[classId] : "unknown";

                return new Object(label, x, y, w, h);
            }
            return null;
        }
    }

    public static class cvrtmp
    {
        public static List<Object> objects = new List<Object>();

        public static void init(string modelPath, string[] classNames, string rtmpUrl)
        {
            CV.open(modelPath, classNames);
            RTMP.open(rtmpUrl);
        }

        public static Task start()
        {
            while (true)
            {
                if (!RTMP.capture.Read(RTMP.frame) || RTMP.frame.Empty())
                {
                    objects.Clear();
                    Object obj = CV.detect();
                    if (obj != null)
                        objects.Add(obj);
                }
            }
        }
    }
}
