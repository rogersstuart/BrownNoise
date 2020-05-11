using NAudio.Wave;
using System.CommandLine;
using System;
using System.IO;
using System.CommandLine.Invocation;
using System.Windows.Forms;

namespace BrownNoise
{
    public class Program
    {
        static void Main(string[] args)
        {
            var root_command = new RootCommand{ 
                new Option<FileInfo>("-f", getDefaultValue: () => new FileInfo("out.wav"), "Where the audio will be stored"),
                new Option<int>("-s", getDefaultValue: () => 60, "The length of the audio clip"),
                new Option<int>("-b", getDefaultValue: () => 44100, "The audio bitrate"),
                new Option<int>("-d", getDefaultValue: () => 16, "The audio bit depth"),
                new Option<bool>("-m", getDefaultValue: () => false, "Enable stereo audio"),
                new Option<int>("-l", getDefaultValue: () => 60, "Leakyness of the integrator")
            };

            root_command.Description = "Generates a WAV file containing processed noise.";

            root_command.Handler = CommandHandler.Create(() =>
            {
                try
                {
                    var res = root_command.Parse(args);

                    BrownNoise(
                        ((FileInfo)res.ValueForOption("-f")).FullName,
                        (int)res.ValueForOption("-s"),
                        (int)res.ValueForOption("-b"),
                        (int)res.ValueForOption("-d"),
                        (bool)res.ValueForOption("-m"),
                        (int)res.ValueForOption("-l")
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occured. The operation has been aborted.");
                }
            });

            root_command.InvokeAsync(args).Wait();
        }

        private static Random r = new Random();

        public static void BrownNoise(string file_name = "out.wav", int seconds = 60, int bitrate = 44100, int bitdepth = 16, bool stereo = false, int lossy_div = 60)
        {
            var now_is = DateTime.Now;

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

            WaveFormat waveFormat;

            if (bitdepth == 32)
                waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(bitrate, stereo ? 2 : 1);
            else
                waveFormat = new WaveFormat(bitrate, bitdepth, stereo ? 2 : 1);

            using (WaveFileWriter writer = new WaveFileWriter(file_name, waveFormat))
            {
                //stereo is interleaved. correct the buffer if needed.
                if (stereo)
                {
                    float[] nb = new float[vals.Length];

                    int val_ctr_l = 0;
                    int val_ctr_r = vals.Length / 2;

                    for (int i = 0; i < vals.Length; i += 2)
                    {

                        nb[i] = vals[val_ctr_l];
                        nb[i + 1] = vals[val_ctr_r];

                        val_ctr_l++;
                        val_ctr_r++;
                    }

                    vals = nb;
                }

                writer.WriteSamples(vals, 0, vals.Length);

                writer.Flush();
                writer.Close();
            }

            var now_is_2 = DateTime.Now;

            Console.WriteLine("completed in " + (now_is_2 - now_is).TotalMilliseconds);

            GC.Collect();
        }

        private static double GetRandomSample()
        {
            return (r.NextDouble() - 0.5) * 2.0;
        }
    }
}
