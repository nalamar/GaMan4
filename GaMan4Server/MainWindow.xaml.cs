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
            // Liste mit "Lagern" anlegen
            List<Bar> storeList = new List<Bar>();
            
            // Vorrübergehend 2 "Lager" zur Liste hinzufügen, hier Speziallager Bar
            storeList.Add(new Bar(0, "Bar 2"));
            storeList.Add(new Bar(2, "Bar 3"));
            
            // Der ComboBox die Auswahlmöglichkeiten hinzufügen
            CbBar.DisplayMemberPath = "Name";
            CbBar.SelectedValuePath = "Id";
            CbBar.ItemsSource = storeList;
            CbBar.SelectedIndex = 0;
            
            // Der Bar mit dem Index 0 5 Produkte mit diversen Eigenschaften hinzufügen
            storeList[0].plist.Add(new Produkt(1, "Becks Pilsner", 0.33, 2.50));
            storeList[0].plist.Add(new Produkt(2, "Becks Gold", 0.33, 2.50));
            storeList[0].plist.Add(new Produkt(3, "Becks Level 7", 0.33, 2.50));
            storeList[0].plist.Add(new Produkt(4, "Becks Ice", 0.33, 2.50));
            storeList[0].plist.Add(new Produkt(5, "Parliament Vodka", 0.04, 3.00));
            storeList[0].plist[0].AnfangVoll = 296;
            
            // Dem spezillen Lager "Bar X" Personal hinzufügen
            storeList[0].personal.Add(new Person(1, "Jan", "Rothe"));
            
            // Aus der ComboBox auslesen, welches Lager/Bar ausgewählt ist
            Bar tmp = (Bar)CbBar.SelectedItem;
            int id = tmp.Id;
            
            // Anhand des gewählten Lagers/bar die Produkt und Personalliste wählen und anzeigen.
            dgProdukte.ItemsSource = storeList[id].plist;
            dgPersonal.ItemsSource = storeList[id].personal;            
            
        }
    }
}
