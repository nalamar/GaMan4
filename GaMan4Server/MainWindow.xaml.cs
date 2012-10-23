using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;

namespace GaMan4Server
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            List<string> tmp = new List<string>();
            
            tmp.Add("Becks Pilsner");
            tmp.Add("Becks Gold");
            tmp.Add("Becks Green Lemon");
            tmp.Add("Becks Ice");
            tmp.Add("Becks Level 7");

            List<Produkt> produkte = new List<Produkt>();

            foreach (string pro in tmp)
            {
                Produkt bier = new Produkt();
                bier.ProduktName = pro;
                produkte.Add(bier);
            }                        
            Testlabel.Content = produkte[0].ProduktName;
            dgv1.ItemsSource = produkte;
        }
    }
}
