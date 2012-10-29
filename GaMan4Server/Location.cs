using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaMan4Server
{
    class Location
    {
        public List<Bar> storeList = new List<Bar>();
        private string name;

        public Location(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
