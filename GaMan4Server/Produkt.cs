
using System.Collections.ObjectModel;
namespace GaMan4Server
{
    public class Produkt
    {
        // Variablen
        private int produktID;
        private string produktName;
        private double produktVK;
        private double produktEKNetto;
        private double gebindeEK;
        private double gebindeVK;
        private bool isOpen;
        private int anfangVoll;
        private int anfangAnriss;
        private int anfangGramm;
        private int endeVoll;
        private int endeAnriss;
        private int endeGramm;
        
        // Konstruktor
        public Produkt(string name)
        {
            this.produktName = name;
        }
        
        // Funktionen
        public int ProduktID
        {
            get { return produktID; }
            set { produktID = value; }
        }

        public string ProduktName 
        {
            get { return produktName; }
            set { produktName = value; }
        }

        public double ProduktVK
        {
            get { return produktVK; }
            set { produktVK = value; }
        }

        public double ProduktEKNetto
        {
            get { return produktEKNetto; }
            set { produktEKNetto = value; }
        }

        public double GebindeEK
        {
            get { return gebindeEK; }
            set { gebindeEK = value; }
        }

        public double GebindeVK
        {
            get { return gebindeVK; }
            set { gebindeVK = value; }
        }

        public bool IsOpen
        {
            get { return isOpen; }
            set { isOpen = value; }
        }

        public int AnfangVoll
        {
            get { return anfangVoll; }
            set { anfangVoll = value; }
        }

        public int AnfangAnriss
        {
            get { return anfangAnriss; }
            set { anfangAnriss = value; }
        }

        public int AnfangGramm
        {
            get { return anfangGramm; }
            set { anfangGramm = value; }
        }

        public int EndeVoll
        {
            get { return endeVoll; }
            set { endeVoll = value; }
        }

        public int EndeAnriss
        {
            get { return endeAnriss; }
            set { endeAnriss = value; }
        }

        public int EndeGramm
        {
            get { return endeGramm; }
            set { endeGramm = value; }
        }
    }
}
