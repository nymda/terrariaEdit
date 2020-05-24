using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrariaEditor
{
    class terrariaItems
    {


    }

    class item
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
