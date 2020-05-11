using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BrownNoiseUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            textBox3.Text = AppDomain.CurrentDomain.BaseDirectory + "out.wav";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //save button clicked

            try
            {
                BrownNoise.Program.BrownNoise(textBox3.Text,
                Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox2.Text),
                comboBox1.SelectedIndex == 0 ? 16 : 24, comboBox2.SelectedIndex == 0 ? false : true, trackBar1.Value);

                Process.Start(@textBox3.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Please check the input and try again.", "Error");
            }

            GC.Collect();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //close button clicked
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //set button clicked

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            sfd.Filter = "Wave File (*.wav)|*.wav";
            sfd.FileName = "out.wav";
            var res = sfd.ShowDialog();
            if (res == DialogResult.OK)
                textBox3.Text = sfd.FileName;
        }

        private void trackBar1_ValueChanged_1(object sender, EventArgs e)
        {
            numericUpDown1.Value = trackBar1.Value;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            trackBar1.Value = (int)numericUpDown1.Value;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int res = 0;
            if (Int32.TryParse(textBox2.Text, out res))
            {
                numericUpDown1.Maximum = res;
                trackBar1.Maximum = res;
            }
                
        }
    }
}
