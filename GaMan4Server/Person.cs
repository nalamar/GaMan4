using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaMan4Server
{
    class Person
    {
        private int id;
        private string vorname;
        private string nachname;
        private double lohn;

        public Person(int id, string vorname, string nachname)
        {
            this.id = id;
            this.vorname = vorname;
            this.nachname = nachname;
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Vorname
        {
            get { return vorname; }
            set { vorname = value; }
        }

        public string Nachname
        {
            get { return nachname; }
            set { nachname = value; }
        }

        public double Lohn
        {
            get { return lohn; }
            set { lohn = value; }
        }

    }
}
