using LionKing.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LionKing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private async void btnFetchAnimal_Click_1(object sender, RoutedEventArgs e)
        {
            //När vi klickar på knappen skapas ett nytt objekt av DbRepository
            DbRepository db = new();
            var animal = await db.GetAnimal(5);

            MessageBox.Show($"{animal.Animal_name}");

            //hej

        }
      
    }
}
