using LionKing.DAL;
using LionKing.Models;
using Npgsql.Internal.TypeHandlers.LTreeHandlers;
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

            
        }

        private async void btnCreate_Click(object sender, RoutedEventArgs e) // does this btn exists still?
        {
            DbRepository db = new(); // can we have this in the constructor instead? Or why have it here?

            //I just tried to think about what we need in order to create a new animal, haven't figured out yet how to retrive it from txtBoxes....
            AnimalClass mammal = new AnimalClass(1, "Mammal");
            Species masaiLion = new Species(1, "Masai Lion", "Panthera leo nubica ", mammal);
            Animal kiara = new Animal("Kiara", masaiLion, mammal);
            var animal = db.AddAnimal(kiara);

            // Create class
            string className = txtCreateClass.Text;
            var newClass = await db.AddClass(className);

        }

        private void btnCreateAnimal_Click(object sender, RoutedEventArgs e)
        {
            DbRepository db = new();
            string animalName = txtCreateAnimal.Text;
            string speciesName = cbxSelectSpecies.Text; // or is it something with selected, unsure of cbx...  
            var animal = db.AddAnimal(animalName, speciesName);
           
            MessageBox.Show($"Djuret {animalName} som är ett {animal.}"); // I can't retrive prop from animal??
        }




        //test




    }
}
