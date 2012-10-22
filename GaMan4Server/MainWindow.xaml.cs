using System.Windows;

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
            Produkt Bier = new Produkt();
            Bier.ProduktName = "Schwarzes";
            Testlabel.Content = Bier.ProduktName;
        }
    }
}
