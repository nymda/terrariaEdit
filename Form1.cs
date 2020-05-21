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
            string done = string.Join(",", decInts);

            int count = 0;
            foreach(int i in decInts)
            {
                Console.WriteLine(count + " | " + i + " | " + Encoding.ASCII.GetString(new byte[] { (byte)i }));
                count++;
            }

            postNameOffset = findNameEnd();
            Console.WriteLine(postNameOffset);

            getInvData();

            Console.WriteLine(done);
            Console.WriteLine(decInts.Count);
        }

        public int findNameEnd()
        {
            int count = 0;
            List<int> buffer = new List<int> { };
            foreach(int i in decInts)
            {
                if(i == 3)
                {
                    foreach(int e in buffer)
                    {
                        Console.WriteLine(Encoding.UTF8.GetString(new byte[] { (byte)e }));
                    }

                    return count + 1;
                }
                else
                {
                    buffer.Add(i);
                    count++;
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
    }
}
