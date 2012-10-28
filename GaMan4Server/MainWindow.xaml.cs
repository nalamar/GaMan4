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
            List<Produkt> plist = new List<Produkt>();
            plist.Add(new Produkt("Becks Pilsner"));
            plist[0].ProduktVK = 2.50;
            plist[0].GebindeVK = 0.33;
            plist[0].AnfangVoll = 296;
            dg1.ItemsSource = plist;
            
        }
    }
}
