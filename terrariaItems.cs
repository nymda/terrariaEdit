using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TerrariaEditor
{
    public class terrariaItems
    {
        public List<item> globalItems = new List<item> { };

        public item searchByID(int id)
        {
            foreach(item i in globalItems)
            {
                if(i.ID == id)
                {
                    return i;
                }
            }
            return new item(0, "Empty", "Empty");
        }

        public item searchByName(string name)
        {
            foreach (item i in globalItems)
            {
                if (i.name == name)
                {
                    return i;
                }
            }
            return new item(0, "Empty", "Empty");
        }

        public void loadTerrariaItems(Form1 host)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "TerrariaEditor.terrItemCSV.txt";
            string result;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            List<String> dataArray = result.Split('\n').ToList();
            foreach(string s in dataArray)
            {
                try
                {
                    string[] tmp = s.Split(',');
                    item tmpItem = new item(Int32.Parse(tmp[0]), tmp[1], tmp[2]);
                    globalItems.Add(tmpItem);
                    host.comboBox1.Items.Add(tmpItem.name + " (" + tmpItem.ID + ")");
                }
                catch
                {

                }
            }
        }
    }

    public class inventoryChunk
    {
        public item item { get; set; }
        public int quantity { get; set; }
        public int modifier { get; set; }
        public terrariaItems itemDirectory { get; set; }
        public inventoryChunk(List<int> data, terrariaItems itemDirectory)
        {
            this.itemDirectory = itemDirectory;
            decode(data);
        }
        public void decode(List<int> encoded)
        {
            int id = resolveEncodedData(encoded[0], encoded[1]);
            item = itemDirectory.searchByID(id);
            quantity = resolveEncodedData(encoded[4], encoded[5]);
            modifier = encoded[8];
        }
        public int resolveEncodedData(int b1, int b2)
        {
            int ID = 0;
            ID += b1;
            ID += 256 * b2;
            return ID;
        }
        public List<int> encodeData(int inp)
        {
            int count256 = 0;
            while (inp > 256)
            {
                inp -= 256;
                count256 += 1;
            }
            return new List<int> { inp, count256 };
        }
        public List<Byte> encode()
        {
            List<Byte> final = new List<Byte> {  };
            List<int> encodedItem = encodeData(item.ID);
            Console.WriteLine(encodedItem[0]);
            List<int> encodedQuant = encodeData(quantity);
            final.Add((byte)encodedItem[0]);
            final.Add((byte)encodedItem[1]);
            final.Add(0x00);
            final.Add(0x00);
            final.Add((byte)encodedQuant[0]);
            final.Add((byte)encodedQuant[1]);
            final.Add(0x00);
            final.Add(0x00);
            final.Add((byte)modifier);
            final.Add(0x00);
            return final;
        }
    }
    public class item
    {
        public int ID { get; set; }
        public string name { get; set; }
        public string internalName { get; set; }
        public item(int ID, string name, string internalName)
        {
            this.ID = ID;
            this.name = name;
        }
    }
}
