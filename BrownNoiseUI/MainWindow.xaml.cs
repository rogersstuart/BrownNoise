using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.Windows.Controls; // Add this using directive

namespace BrownNoiseUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            textBoxOutputFile.Text = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "out.wav");
        }

        private async void ButtonGenerate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Use the sender parameter instead of the field
                var generateButton = (Button)sender;
                
                // Validate inputs
                if (!int.TryParse(textBoxDuration.Text, out int duration) || duration <= 0)
                {
                    MessageBox.Show("Duration must be a positive number.", "Invalid Input", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                if (!int.TryParse(textBoxSampleRate.Text, out int sampleRate) || sampleRate <= 0)
                {
                    MessageBox.Show("Sample rate must be a positive number.", "Invalid Input", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(textBoxOutputFile.Text))
                {
                    MessageBox.Show("Please specify an output file.", "Invalid Input", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                // Disable UI during generation
                generateButton.IsEnabled = false;
                
                int bitDepth = comboBoxBitDepth.SelectedIndex switch
                {
                    0 => 16,
                    1 => 24,
                    2 => 32,
                    _ => 16
                };
                
                // Capture UI values before switching to background thread
                string outputFile = textBoxOutputFile.Text;
                bool isStereo = comboBoxChannels.SelectedIndex == 1;
                int leakiness = (int)sliderLeakiness.Value;
                
                try
                {
                    await Task.Run(() =>
                    {
                        BrownNoise.Program.BrownNoise(
                            outputFile,
                            duration,
                            sampleRate,
                            bitDepth,
                            isStereo,
                            leakiness);
                    });
                    
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = outputFile,
                        UseShellExecute = true
                    });
                }
                finally 
                {
                    generateButton.IsEnabled = true;
                    GC.Collect();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ButtonBrowse_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                Filter = "Wave File (*.wav)|*.wav",
                FileName = "out.wav"
            };

            if (sfd.ShowDialog() == true)
            {
                textBoxOutputFile.Text = sfd.FileName;
            }
        }

        private void SliderLeakiness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (textBoxLeakiness != null)
            {
                textBoxLeakiness.Text = ((int)sliderLeakiness.Value).ToString();
            }
        }

        private void TextBoxLeakiness_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (int.TryParse(textBoxLeakiness.Text, out int value))
            {
                if (value >= sliderLeakiness.Minimum && value <= sliderLeakiness.Maximum)
                {
                    sliderLeakiness.Value = value;
                }
            }
        }

        private void TextBoxDuration_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (int.TryParse(textBoxDuration.Text, out int duration) && sliderLeakiness != null)
            {
                sliderLeakiness.Maximum = duration;
                if (sliderLeakiness.Value > duration)
                {
                    sliderLeakiness.Value = duration;
                }
            }
        }
    }
}