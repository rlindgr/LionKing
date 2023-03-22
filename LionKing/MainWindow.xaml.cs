using LionKing.DAL;
using LionKing.Models;
using Npgsql.Internal.TypeHandlers.LTreeHandlers;
using NpgsqlTypes;
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

        #region CREATE

        /// <summary>
        /// Presents all AnimalClasses in Combobox in Create area 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cbxSelectClass_DropDownOpened(object sender, EventArgs e)
        {
            DbRepository db = new();
            var selectClass = await db.GetAllClasses();
            cbxSelectClass.ItemsSource = selectClass; 
        }
        /// <summary>
        /// Presents all species in combobox in Create area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cbxSelectSpecies_DropDownOpened(object sender, EventArgs e)
        {
            DbRepository db = new();
            var allSpecies = await db.GetAllSpecies();
            cbxSelectSpecies.ItemsSource = allSpecies;

        }

        /// <summary>
        /// Creates new class and presents results in ?
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnCreateClass_Click(object sender, RoutedEventArgs e)
        {
            DbRepository db = new();
            var newClass = await db.AddClass(txtCreateClass.Text);
            MessageBox.Show($"Djurarten {newClass} har skapats och lagts till i databasen");
            lstAnimalClass.Items.Clear();
            var selectClass = await db.GetAllClasses();
            lstAnimalClass.ItemsSource = selectClass;


        }

        private async void btnCreateSpecies_Click(object sender, RoutedEventArgs e)
        {
            DbRepository db = new();
            string speciesName = txtCreateSpecies.Text;
            string latinName = txtLatinName.Text;
            AnimalClass selectedClass = (AnimalClass)cbxSelectClass.SelectedItem; //??
            var newSpecies = await db.AddSpecies(speciesName, latinName, selectedClass);

        } 



        #endregion

        #region READ

        /// <summary>
        /// Displays all animals classes in listbox in View area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnViewAnimalClass_Click(object sender, RoutedEventArgs e)
        {
            lstSpecies.ItemsSource = null; //Do we need to create a method that clears listboxes for this to work? issues with ItemSource vs item.add()
            lstAnimal.ItemsSource = null; //Do we need to create a method that clears listboxes for this to work? issues with ItemSource vs item.add()

            DbRepository db = new();
            var allClasses = await db.GetAllClasses();
            lstAnimalClass.ItemsSource = allClasses;

        }
        /// <summary>
        /// Displays all species in listbox in View Area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnViewSpecies_Click(object sender, RoutedEventArgs e)
        {
            lstAnimalClass.ItemsSource = null;
            lstAnimal.ItemsSource= null;
            
            DbRepository db = new();
            var allSpecies = await db.GetAllSpecies();
            lstSpecies.ItemsSource = allSpecies;
        }
        /// <summary>
        /// Displays all animals in listbox in View area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnViewAnimal_Click(object sender, RoutedEventArgs e)
        {
            lstAnimalClass.ItemsSource = null;
            lstSpecies.ItemsSource = null; 
            
            DbRepository db = new();
            var allAnimals = await db.GetAllAnimals();
            lstAnimal.ItemsSource = allAnimals;
        }
        /// <summary>
        /// Gets animal based on animal name entered in textbox in View area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    var matchingSpecies = await db.GetSpeciesById(animal.Species_id);                   
                    var matchingClasses = await db.GetClassById(animal.Class_id);
            
                    lstAnimal.Items.Add(animal.Animal_name);
                    lstSpecies.Items.Add(matchingSpecies);
                    lstAnimalClass.Items.Add(matchingClasses);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region UPDATE

        #endregion

        #region DELETE

        #endregion





        /// <summary>
        /// Method used to find class_id from selected item in combobox
        /// </summary>
        /// <param name="animalClass"></param>
        private void SelectClass(AnimalClass animalClass)
        {

            foreach (AnimalClass c in lstAnimalClass.Items)
            {
                if (c.Class_id == animalClass.Class_id)
                {
                    int index = lstAnimalClass.Items.IndexOf(c);
                    lstAnimalClass.SelectedItem = c;
                    break;
                }
            }

            //foreach (Film f in listBox.Items)
            //{
            //    if (f.Film_id == film.Film_id)
            //    {
            //        int index = listBox.Items.IndexOf(f);
            //        listBox.SelectedItem = f;
            //        break;
            //    }
        }  //OBS! Is this code needed now?

        /// <summary>
        /// Event for when user chooses an animalClass in listbox in View area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void lstAnimalClass_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AnimalClass animalClass = (AnimalClass)lstAnimalClass.SelectedItem;
            DbRepository db = new DbRepository();
            var matchingSpecies = await db.GetAllSpeciesFromClass(animalClass.Class_id);
            lstSpecies.ItemsSource = matchingSpecies;            
        }


        private async void lstSpecies_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Species species= (Species)lstSpecies.SelectedItem;
            DbRepository db = new DbRepository();
            var matchingAnimals = await db.GetAnimalsBySpeciesId(species.Species_id);
            lstAnimal.ItemsSource = matchingAnimals;
        }


        /// <summary>
        /// Presents which species and class that a selected animal belongs to in View area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void lstAnimal_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            lstAnimalClass.ItemsSource = null;
            lstSpecies.ItemsSource = null;

            Animal animal = (Animal)lstAnimal.SelectedItem;
            DbRepository db = new DbRepository();           
            var matchingSpecies = await db.GetSpeciesById(animal.Species_id);          
            var matchingClasses = await db.GetClassById(animal.Class_id);

            lstSpecies.Items.Add(matchingSpecies); 
            lstAnimalClass.Items.Add(matchingClasses);
        }

       
    }

    
}
