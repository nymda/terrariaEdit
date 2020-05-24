using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TerrariaEditor
{
    public partial class Form1 : Form
    {
        private const string EncryptionKey = "h3y_gUyZ";
        public List<Byte> decrypted = new List<Byte> { };
        public List<inventoryChunk> playerInv = new List<inventoryChunk> { };
        public string playerName = "";
        public int nameEndOffset = 0;
        terrariaItems itemContainer = new terrariaItems();
        public crypto cryptoHander = new crypto();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Input File";
                dlg.Filter = "Player | *.plr";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    FileInfo fi = new FileInfo(dlg.FileName);
                    decrypted = cryptoHander.decryptNew(File.ReadAllBytes(dlg.FileName), (int)fi.Length).ToList();
                }
            }
            groupBox1.Enabled = true;
            setNameData();
            this.Text = "Terraria Editor | " + playerName;
            setInvData();
            button1.Enabled = false;
        }

        public void setNameData()
        {
            byte[] printables = new byte[] { 0x27, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x5c, 0x27, 0x28, 0x29, 0x2a, 0x2b, 0x2c, 0x2d, 0x2e, 0x2f, 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b, 0x3c, 0x3d, 0x3e, 0x3f, 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x4b, 0x4c, 0x4d, 0x4e, 0x4f, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5a, 0x5b, 0x5c, 0x5c, 0x5d, 0x5e, 0x5f, 0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x6b, 0x6c, 0x6d, 0x6e, 0x6f, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7a, 0x7b, 0x7c, 0x7d, 0x7e };
            int startpos = 25;

            StringBuilder nameBuild = new StringBuilder();

            for (int i = startpos; i < 51; i++)
            {
                if (printables.Contains(decrypted[i]))
                {
                    nameBuild.Append(Encoding.ASCII.GetString(new byte[] { decrypted[i] }));
                }
                else
                {
                    nameEndOffset = i;
                    break;
                }
            }

            playerName = nameBuild.ToString();
        }

        public void setInvData()
        {
            int dataBeginOffset = nameEndOffset + 211;
            int dataEndOffset = dataBeginOffset + 500;

            int extCounter = 0;
            List<int> tmp = new List<int> { };
            for (int i = dataBeginOffset; i < dataEndOffset; i++)
            {
                extCounter++;
                tmp.Add(decrypted[i]);
                if (extCounter == 10)
                {
                    inventoryChunk iv = new inventoryChunk(tmp, itemContainer);
                    playerInv.Add(iv);
                    tmp = new List<int> { };
                    extCounter = 0;
                }
            }
            foreach (inventoryChunk iv in playerInv)
            {
                dataGridView1.Rows.Add(iv.item.name, iv.quantity);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            itemContainer.loadTerrariaItems(this);
            comboBox1.SelectedIndex = 0;
            dataGridView1.Columns[0].Width = dataGridView1.Width - dataGridView1.Columns[1].Width - 60;
            dataGridView1.AllowUserToAddRows = false;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgv1_cellclick(object sender, DataGridViewCellEventArgs e)
        {
            if(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value != null)
            {
                int id = itemContainer.searchByName(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value.ToString()).ID;
                comboBox1.SelectedItem = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value.ToString() + " (" + id + ")";
                numericUpDown1.Value = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[1].Value.ToString());
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int invindex = dataGridView1.CurrentCell.RowIndex;
            string procName = comboBox1.SelectedItem.ToString();
            string[] split = procName.Split(' ');
            string removedID = string.Join(" ", split.Take(split.Length - 1));
            item newitem = itemContainer.searchByName(removedID);
            int newQuant = (int)numericUpDown1.Value;
            playerInv[invindex].item = newitem;
            playerInv[invindex].quantity = newQuant;
            dataGridView1.Rows.Clear();
            foreach (inventoryChunk iv in playerInv)
            {
                dataGridView1.Rows.Add(iv.item.name, iv.quantity);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<Byte> output = reEncode();
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Title = "Save player file";
                dlg.Filter = "Terraria player | *.plr";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string savepath = dlg.FileName;
                    cryptoHander.encryptAndSave(output.ToArray(), savepath);
                }
            }
        }

        public List<Byte> reEncode()
        {
            List<Byte> buffer = new List<Byte> { };
            List<Byte> save = decrypted;
            foreach (inventoryChunk iv in playerInv)
            {
                List<Byte> tmp = iv.encode();
                foreach(byte b in tmp)
                {
                    buffer.Add(b);
                }
            }
            int dataBeginOffset = nameEndOffset + 211;
            int dataEndOffset = dataBeginOffset + 500;
            int extCount = 0;

            for (int i = dataBeginOffset; i < dataEndOffset; i++)
            {
                save[i] = buffer[extCount];
                extCount++;
            }
            return save;
        }

        private void dgv_resize(object sender, EventArgs e)
        {
            dataGridView1.Columns[0].Width = dataGridView1.Width - dataGridView1.Columns[1].Width - 60;
        }
    }

}
