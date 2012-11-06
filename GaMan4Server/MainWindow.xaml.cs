using System;
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

            Server server = new Server("::1", 4850);
            Subscribe(server);            
            TbLog.AppendText("GaMan 4 Server" + Environment.NewLine);
            if (server.Start())
            {
                TbLog.AppendText("Server gestartet" + Environment.NewLine);
            }
            else
            {
                TbLog.AppendText("Server nicht gestartet" + Environment.NewLine);
            }
            server.CreateDemoXML();


            /////////////////////////////////////////////////
            ///     Beispieldaten zum Anwendungstest      ///
            /// /////////////////////////////////////////////
            

            // Liste mit Locations anlegen
            List<Location> locList = new List<Location>();

            // Location zur Liste hinzufügen
            locList.Add(new Location("Bunker"));

            // Bar zur Location hinzufügen
            locList[0].storeList.Add(new Bar(0, "Bar 2"));
            
            // Der BarComboBox die Lager-Auswahlmöglichkeiten hinzufügen
            CbBar.DisplayMemberPath = "Name";
            CbBar.SelectedValuePath = "Id";
            CbBar.ItemsSource = locList[0].storeList;
            CbBar.SelectedIndex = 0;
            
            // Der Bar mit dem Index 0 5 Produkte mit diversen Eigenschaften hinzufügen
            locList[0].storeList[0].plist.Add(new Produkt(1, "Becks Pilsner", 0.33, 2.50));
            locList[0].storeList[0].plist.Add(new Produkt(2, "Becks Gold", 0.33, 2.50));
            locList[0].storeList[0].plist.Add(new Produkt(3, "Becks Level 7", 0.33, 2.50));
            locList[0].storeList[0].plist.Add(new Produkt(4, "Becks Ice", 0.33, 2.50));
            locList[0].storeList[0].plist.Add(new Produkt(5, "Parliament Vodka", 0.04, 3.00));
            locList[0].storeList[0].plist[0].AnfangVoll = 296;
            
            // Dem spezillen Lager "Bar X" Personal hinzufügen
            locList[0].storeList[0].personal.Add(new Person(1, "Jan", "Rothe"));
            
            // Aus der ComboBox auslesen, welches Lager/Bar ausgewählt ist
            Bar tmp = (Bar)CbBar.SelectedItem;
            int id = tmp.Id;
            
            // Anhand des gewählten Lagers/bar die Produkt und Personalliste wählen und anzeigen.
            dgProdukte.ItemsSource = locList[0].storeList[id].plist;
            dgPersonal.ItemsSource = locList[0].storeList[id].personal;
            
        }

        private void addText(Server s, LogText e)
        {            
            this.Dispatcher.Invoke(new Action<System.Windows.Controls.TextBox>(TbLog => TbLog.Text += Environment.NewLine + e.SetText.ToString()), this.TbLog);
        }
        
        public void Subscribe(Server s)
        {
            s.Log += new Server.LogHandler(addText);
        }
    }
}
