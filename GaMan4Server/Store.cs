using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaMan4Server
{
    class Store
    {
        // Variablen        
        private int id;
        private string name;
        public List<Produkt> plist = new List<Produkt>();

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        
        public string Name
        {
            get { return name; }
            set { name = value; }
        }  

        public List<Produkt> getList
        {
            get { return plist; }
        }
    }
}
