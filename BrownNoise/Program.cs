using NAudio.Wave;
using System.CommandLine;
using System;
using System.IO;
using System.CommandLine.Invocation;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace BrownNoise
{
    public class Program
    {
        static async Task<int> Main(string[] args)
        {
            var fileOption = new Option<FileInfo>(
                aliases: new[] { "--file", "-f" },
                getDefaultValue: () => new FileInfo("out.wav"),
                description: "Where the audio will be stored");
                
            var durationOption = new Option<int>(
                aliases: new[] { "--seconds", "-s" },
                getDefaultValue: () => 60,
                description: "The length of the audio clip in seconds");
            
            var bitrateOption = new Option<int>(
                aliases: new[] { "--bitrate", "-b" },
                getDefaultValue: () => 44100,
                description: "The audio bitrate");
            
            var bitdepthOption = new Option<int>(
                aliases: new[] { "--bitdepth", "-d" },
                getDefaultValue: () => 16,
                description: "The audio bit depth");
            
            var stereoOption = new Option<bool>(
                aliases: new[] { "--stereo", "-m" },
                getDefaultValue: () => false,
                description: "Enable stereo audio");
            
            var leakinessOption = new Option<int>(
                aliases: new[] { "--leakiness", "-l" },
                getDefaultValue: () => 60,
                description: "Leakyness of the integrator");

            var rootCommand = new RootCommand("Generates a WAV file containing brown noise")
            {
                fileOption,
                durationOption,
                bitrateOption,
                bitdepthOption,
                stereoOption,
                leakinessOption
            };
            
            rootCommand.Handler = CommandHandler.Create<FileInfo, int, int, int, bool, int>((file, seconds, bitrate, bitdepth, stereo, leakiness) =>
            {
                BrownNoise(file.FullName, seconds, bitrate, bitdepth, stereo, leakiness);
            });
            
            return await rootCommand.InvokeAsync(args);
        }

        private static Random r = new Random();

        public static void BrownNoise(string file_name = "out.wav", int seconds = 60, int bitrate = 44100, 
            int bitdepth = 16, bool stereo = false, int lossy_div = 60, 
            IProgress<int> progress = null)
        {
            var now_is = DateTime.Now;
            float[] vals = new float[bitrate * seconds * (stereo ? 2 : 1)];
            
            float lossy = 1.0f - (1.0f / (bitrate / (float)lossy_div));
            
            // Generate mono or left channel
            float last_sample = (float)GetRandomSample();
            float max_dev = 0;
            for (int i = 0; i < bitrate * seconds; i++)
            {
                vals[i] = (float)(last_sample + GetRandomSample());
                last_sample = vals[i] * lossy;
                
                float dev = Math.Abs(vals[i]);
                if (dev > max_dev)
                    max_dev = dev;
                
                // Report progress every 1%
                if (progress != null && i % (vals.Length / 100) == 0)
                {
                    progress.Report((int)((i * 100.0) / vals.Length));
                }
            }
            
            // Generate right channel if stereo
            if (stereo)
            {
                last_sample = (float)GetRandomSample();
                for (int i = bitrate * seconds; i < vals.Length; i++)
                {
                    vals[i] = last_sample + (float)GetRandomSample();
                    last_sample = vals[i] * lossy;
                }
            }
            
            // Normalize if needed
            if (max_dev > 1.0f)
            {
                float scalar = 1.0f / max_dev;
                for (int i = 0; i < vals.Length; i++)
                {
                    vals[i] *= scalar;
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
