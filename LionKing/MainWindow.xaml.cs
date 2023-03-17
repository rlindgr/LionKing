using LionKing.DAL;
using LionKing.Models;
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

            if (cbxAnimalClass.IsDropDownOpen == true) 
            {
                cbxAnimalClass.SelectedItem= null;
                //cbxAnimalClass.SelectedItem = 
            
            }

            
        }


        private async void btnFetchAnimal_Click_1(object sender, RoutedEventArgs e)
        {
            //När vi klickar på knappen skapas ett nytt objekt av DbRepository
            DbRepository db = new();
            var animal = await db.GetAnimalById(5);

            MessageBox.Show($"{animal.Animal_name}");


        }

        private async void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DbRepository db = new();
            var animals = await db.GetAnimals();
            lstResults.ItemsSource = animals;
            lstResults.DisplayMemberPath = "Animal_name"; 

            //todo: fixa så det går att ordna namn asc, desc - ska det vara egna metoder likt godiskalkylatorn?
            //if (rdbAsc.IsChecked == true) 
            //{
            //    animals.OrderBy(animal => animal.Animal_name);
            
            //}
            // else 
            //{
            //    animals.OrderDescending();           
            //}

        }

        private void lstResults_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (lstResults.SelectedItem is Animal select)
            {            
                MessageBox.Show($" {select.Animal_name} är en art X som tillhör djurklass X"); 
            }

        }

        private async  void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            DbRepository db = new();
            string findName = txtFindAnimalName.Text;
            var findAnimal = await db.FindAnimalByName(findName);


            //todo: fixa så det går att visa info om ett specifikt djur i listboxen
            //if (findAnimal != null) 
            {
                //lstResults.ItemsSource = findAnimal;
                lstResults.DisplayMemberPath = "Animal_name";

            }
            //todo: om ingen träff, visa fel meddelande
            //else 
            {
                MessageBox.Show("Det finns tyvärr inget djur med det namnet");
            } 
 
     
        }





    }

}
