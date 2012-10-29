using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaMan4Server
{
    class Bar : Store
    {
        public List<Person> personal = new List<Person>();

        public Bar(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
        
        public List<Person> getPersonal
        {
            get { return personal; }
        }
    }
}
