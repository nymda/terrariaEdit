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
    
    public class invItem
    {
        public int ID { get; set; }
        public int Offset { get; set; } //?
        public int Quantity { get; set; }
        public List<int> raw { get; set; }
        public invItem(int ID, int Offset, int Quantity, List<int> raw)
        {
            this.ID = ID;
            this.Offset = Offset;
            this.Quantity = Quantity;
            this.raw = raw;
        }
    }
}
