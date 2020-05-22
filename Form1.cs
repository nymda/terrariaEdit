using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TerrariaEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        crypto lib = new crypto();
        public List<Byte> decData = new List<Byte> { };
        public List<int> decInts = new List<int> { };
        public int postNameOffset = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Input File";
                dlg.Filter = "Player | *.plr";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    decData = lib.getRawData(dlg.FileName);              
                }
            }

            foreach (byte b in decData)
            {
                decInts.Add(Convert.ToInt32(b));
            }
        }

        public int findNameOffset()
        {
            //strip unprintable characters. seperate strings with 0x00.
            byte[] printables = new byte[] { 27, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x5c, 0x27, 0x28, 0x29, 0x2a, 0x2b, 0x2c, 0x2d, 0x2e, 0x2f, 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b, 0x3c, 0x3d, 0x3e, 0x3f, 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x4b, 0x4c, 0x4d, 0x4e, 0x4f, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5a, 0x5b, 0x5c, 0x5c, 0x5d, 0x5e, 0x5f, 0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x6b, 0x6c, 0x6d, 0x6e, 0x6f, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7a, 0x7b, 0x7c, 0x7d, 0x7e };
            List<Byte> buffer = new List<Byte> { };
            bool swit = true;
            foreach (byte b in decData)
            {
                if (printables.Contains(b))
                {
                    buffer.Add(b);
                    swit = true;
                }
                else
                {
                    if (swit)
                    {
                        buffer.Add(0x00);
                        swit = false;
                    }
                }
            }

            string dat = Encoding.UTF8.GetString(buffer.ToArray());
            string[] rawSplit = dat.Split((char)0x00);

            string terrDataString = Encoding.ASCII.GetString(decData.ToArray());

            foreach (string s in rawSplit)
            {
                if (s.Length > 2)
                {
                    Console.WriteLine(s);
                    return terrDataString.IndexOf(s) + s.Length;
                }
            }

            return -1;
        }

        public void getInvData()
        {
            int extCount = 0;
            List<int> buffer = new List<int> { };
            for(int i = 0; i < 500; i++)
            {
                if(extCount == 9)
                {
                    Console.WriteLine(string.Join(",", buffer));
                    extCount = 0;
                    buffer.Clear();
                }
                else
                {
                    buffer.Add(decInts[201 + postNameOffset + i]);
                    extCount++;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            postNameOffset = findNameOffset();
            getInvData();
        }
    }
}
