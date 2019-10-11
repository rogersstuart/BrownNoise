using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BrownNoise
{
    class Program
    {
        static void Main(string[] args)
        {
            //args file_name seconds

            if(args.Length == 4)
                BrownNoise(args[0], Convert.ToInt32(args[1]), Convert.ToInt32(args[2]), Convert.ToInt32(args[3]));
            else
                if (args.Length == 2)
                BrownNoise(args[0], Convert.ToInt32(args[1]));
            else
                BrownNoise();


            //Thread.Sleep(-1);
        }

        private static Random r = new Random();

        private static void BrownNoise(string file_name = "out.wav", int seconds = 60, int bitrate = 44100, int bitdepth = 16)
        {
            float[] vals = new float[bitrate*seconds];

            //find scale values
            float max_dev = 0;

            var last_sample = GetRandomSample();

            for (int i = 0; i < vals.Length; i++)
            {
                vals[i] = (float)(last_sample + GetRandomSample());
                last_sample = vals[i];

                float dev = Math.Abs(vals[i]);
                if (dev > max_dev)
                    max_dev = dev;
            }

            //scale samples
            if (max_dev > 1.0f)
            {
                float scalar = 1.0f - ((max_dev - 1.0f) / max_dev);

                for (int i = 0; i < vals.Length; i++)
                {
                    vals[i] = vals[i] * scalar;
                }
            }

            WaveFormat waveFormat = new WaveFormat(bitrate, bitdepth, 1);

            WaveFileWriter writer = new WaveFileWriter(file_name, waveFormat);

            writer.WriteSamples(vals, 0, vals.Length);

            writer.Flush();
            writer.Close();
        }

        private static double GetRandomSample()
        {
            return (r.NextDouble() - 0.5) * 2.0;
        }
    }
}
