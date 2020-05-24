using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrariaEditor
{
    class types
    {

    }
    public class inventoryChunk
    {
        public List<int> data { get; set; }
        public inventoryChunk(List<int> data)
        {
            this.data = data;
        }
        public int resolveEncodedData(int b1, int b2)
        {
            int ID = 0;
            ID += b1;
            ID += 256 * b2;
            return ID;
        }
        public int getItemID()
        {
            return resolveEncodedData(data[0], data[1]);
        }
        public int getItemQuantity()
        {
            return resolveEncodedData(data[4], data[5]);
        }
        public string getPrintable()
        {
            List<String> buffer = new List<String> { };
            foreach (int i in data)
            {
                StringBuilder tmp = new StringBuilder();
                int bufferNeeded = 3 - i.ToString().Length;
                if (bufferNeeded > 0)
                {
                    tmp.Append('0', bufferNeeded);
                }
                tmp.Append(i.ToString());
                buffer.Add(tmp.ToString());
            }
            String final = string.Join(",", buffer);
            return final;
        }
    }
}
