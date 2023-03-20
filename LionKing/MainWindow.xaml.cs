using LionKing.DAL;
using LionKing.Models;
using Npgsql.Internal.TypeHandlers.LTreeHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        

        private async void btnGetAnimal_Click_1(object sender, RoutedEventArgs e)
        {
            DbRepository db = new();
            string GetAnimal = txtGetAnimal.Text;
            try
            {
                var animal = await db.GetAnimalByName(GetAnimal);

                if (animal.Animal_name == null)
                {
                    MessageBox.Show("Det finns inget djur med det namnet");
                }
                else
                {
                    lstAnimal.Items.Clear();
                    lstAnimal.Items.Add(animal.Animal_name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCreateClass_Click(object sender, RoutedEventArgs e)
        {
            //DbRepository db = new(); // can we have this in the constructor instead? Or why have it here?

            ////I just tried to think about what we need in order to create a new animal, haven't figured out yet how to retrive it from txtBoxes....
            //AnimalClass mammal = new AnimalClass(1, "Mammal");
            //Species masaiLion = new Species(1, "Masai Lion", "Panthera leo nubica ", mammal);
            //Animal kiara = new Animal("Kiara", masaiLion, mammal);
            //var animal = db.AddAnimal(kiara);

            //// Create class
            //string className = txtCreateClass.Text;
            //var newClass = await db.AddClass(className);
        }

        private void btnCreateAnimal_Click(object sender, RoutedEventArgs e)
        {
            DbRepository db = new();
            string animalName = txtCreateAnimal.Text;
            string speciesName = cbxSelectSpecies.Text; // or is it something with selected, unsure of cbx...  
            var animal = db.AddAnimal(animalName, speciesName);

            //MessageBox.Show($"Djuret {animalName} som är ett {animal.}"); // I can't retrive prop from animal??
        }

        //Presents all AnimalClasses in Combobox in View area
        private async void cbxViewClass_DropDownOpened(object sender, EventArgs e)
        {
            DbRepository db = new();
            var allClasses = await db.GetAllClasses();
            cbxViewClass.ItemsSource = allClasses;

        }

        //Presents all Species in Combobox in View area
        private async void ViewSpecies_DropDownOpened(object sender, EventArgs e)
        {
            DbRepository db = new();
            var allSpecies = await db.GetAllSpecies();
            cbxViewSpecies.ItemsSource = allSpecies;
        }

        //Presents all Animals in Combobox in View area - OBS! Need if-statement to handle null values
        private async void cbxViewAnimal_DropDownOpened(object sender, EventArgs e)
        {
            DbRepository db = new();
            var allAnimals = await db.GetAllAnimals();
            cbxViewAnimal.ItemsSource = allAnimals;
        }

        //Presents all AnimalClasses in Combobox in Create area - OBS! Not working properly
        private async void cbxSelectClass_DropDownOpened(object sender, EventArgs e)
        {
            DbRepository db = new();
            var selectClass = await db.GetAllClasses();
            cbxViewClass.ItemsSource = selectClass; // dont understand why this is not working? Why isn't animal classes shown in interface?
        }

        //Presents all Species in Combobox in Create area - OBS! Not working properly
        private async void cbxSelectSpecies_DropDownOpened(object sender, EventArgs e)
        {
            DbRepository db = new();
            var allSpecies = await db.GetAllSpecies();
            cbxViewSpecies.ItemsSource = allSpecies; //why isn't species shown in interface??

        }

        //Trying to enable showing info about species in listbox once an animalclass is shown - OBS! not working properly yet
        private async void cbxViewClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DbRepository db = new();
       
            //How to we establish which animalclass is choosen? Can we reach/use class_id that is fetched in SelectClass_DropDownOpened somehow?
            {
                var allSpecies = await db.GetAllSpeciesFromClass(1);
                lstAnimal.Items.Add(allSpecies);
            }
            
        }
    }
}
