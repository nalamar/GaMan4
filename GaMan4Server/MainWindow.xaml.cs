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
            List<Store> storeList = new List<Store>();
            storeList.Add(new Store());
            storeList[0].Name = "Bar 2";
            storeList[0].plist.Add(new Produkt(1, "Becks Pilsner", 0.33, 2.50));
            storeList[0].plist.Add(new Produkt(2, "Becks Gold", 0.33, 2.50));
            storeList[0].plist.Add(new Produkt(3, "Becks Level 7", 0.33, 2.50));
            storeList[0].plist.Add(new Produkt(4, "Becks Ice", 0.33, 2.50));
            storeList[0].plist.Add(new Produkt(5, "Parliament Vodka", 0.04, 3.00));
            storeList[0].plist[0].AnfangVoll = 296;
            dg1.ItemsSource = storeList[0].getList;
            
        }
    }
}
