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

            try
            {
                if (args.Length == 4)
                    BrownNoise(args[0], Convert.ToInt32(args[1]), Convert.ToInt32(args[2]), Convert.ToInt32(args[3]));
                else
                if (args.Length == 2)
                    BrownNoise(args[0], Convert.ToInt32(args[1]));
                else
                 if (args.Length == 6)
                    BrownNoise(args[0], Convert.ToInt32(args[1]), Convert.ToInt32(args[2]), Convert.ToInt32(args[3]), Convert.ToBoolean(args[4]), Convert.ToInt32(args[5]));
                else
                    BrownNoise();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error generating brown noise.");
                Console.WriteLine("Usage: BrownNoise.exe file_name seconds bitrate bitdepth");
            }
            

            //Thread.Sleep(-1);
        }

        private static Random r = new Random();

        private static void BrownNoise(string file_name = "out.wav", int seconds = 60, int bitrate = 44100, int bitdepth = 16, bool stereo = false, int lossy_div = 60)
        {
            float[] vals = new float[bitrate*seconds*(stereo ? 2 : 1)];

            //find scale values
            float max_dev = 0;
            float lossy = 1.0f - (1.0f / (bitrate / (float)lossy_div));

            var last_sample = GetRandomSample();

            for (int i = 0; i < vals.Length; i++)
            {
                vals[i] = (float)(last_sample + GetRandomSample());
                last_sample = vals[i]*lossy;

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

            WaveFormat waveFormat = new WaveFormat(bitrate, bitdepth, stereo ? 2 : 1);

            WaveFileWriter writer = new WaveFileWriter(file_name, waveFormat);

            if(stereo)
            {
                float[] nb = new float[vals.Length];

                int nb_ctr = 0;
                int val_ctr_l = 0;
                int val_ctr_r = vals.Length/2;

                for (int i = 0; i < vals.Length; i+=2)
                {

                    nb[nb_ctr] = vals[val_ctr_l];
                    nb[nb_ctr + 1] = vals[val_ctr_r];

                    nb_ctr += 2;
                    val_ctr_l++;
                    val_ctr_r++;
                }

                vals = nb;
            }

            writer.WriteSamples(vals, 0, vals.Length);

            writer.Flush();
            writer.Close();
        }

        private static double GetRandomSample()
        {
            return (r.NextDouble() - 0.5);
        }
    }
}
