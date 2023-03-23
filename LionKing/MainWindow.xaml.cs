using LionKing.DAL;
using LionKing.Models;
using Npgsql.Internal.TypeHandlers.LTreeHandlers;
using NpgsqlTypes;
using System;
using System.Collections;
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
            try
            {
                DbRepository db = new();
                var selectClass = await db.GetAllClasses();
                cbxSelectClass.ItemsSource = null;
                cbxSelectClass.ItemsSource = selectClass;
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Presents all Species in combobox in Create area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cbxSelectSpecies_DropDownOpened(object sender, EventArgs e)
        {
            try
            {
                DbRepository db = new();
                var allSpecies = await db.GetAllSpecies();
                cbxSelectSpecies.ItemsSource = null;
                cbxSelectSpecies.ItemsSource = allSpecies;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }

        /// <summary>
        /// Creates new Class and presents results in ?
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnCreateClass_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DbRepository db = new();
                var newClass = await db.AddClass(txtCreateClass.Text);
                MessageBox.Show($"Djurklassen {newClass} har skapats och lagts till i databasen");
                txtCreateClass.Clear();
                lstAnimalClass.ItemsSource = null;
                var allClasses = await db.GetAllClasses();
                lstAnimalClass.ItemsSource = allClasses;
            }
            catch (Exception ex)  
            {
                MessageBox.Show(ex.Message );
            }


        }

        private async void btnCreateSpecies_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DbRepository db = new();
                string speciesName = txtCreateSpecies.Text;
                string latinName = txtLatinName.Text;
                AnimalClass selectedClass = (AnimalClass)cbxSelectClass.SelectedItem;
                var newSpecies = await db.AddSpecies(speciesName, latinName, selectedClass);
                lstSpecies.ItemsSource = null;
                MessageBox.Show($"Djurarten {newSpecies} har skapats och lagts till i klassen {selectedClass}");
                txtCreateSpecies.Clear();
                txtLatinName.Clear();
                cbxSelectClass.ItemsSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            
           

        }
        /// <summary>
        /// Create animal and returns name, class and species.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnCreateAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DbRepository db = new();
                string animalName = txtCreateAnimal.Text;
                Species selectedSpecies = (Species)cbxSelectSpecies.SelectedItem;

                var newAnimal = await db.AddAnimal(animalName, selectedSpecies);
                var className = await db.GetClassById(newAnimal.Class_id);

                MessageBox.Show($"Djuret {newAnimal} har lagts till i databasen som en/ett {selectedSpecies} som tillhör {className}");
                txtCreateAnimal.Clear();
                cbxSelectSpecies.ItemsSource = null;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
          
        }



        #endregion

        #region READ

        /// <summary>
        /// Gets Animal based on Animal_Name entered in textbox in View area
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

                    txtCurrentClass.Text = matchingClasses.ToString();
                    txtCurrentSpecies.Text = matchingSpecies.ToString();
                    txtCurrentName.Text = GetAnimal;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Displays all AnimalClasses in listbox in View area
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
        /// Displays all Species in listbox in View Area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnViewSpecies_Click(object sender, RoutedEventArgs e)
        {
            lstAnimalClass.ItemsSource = null;
            lstAnimal.ItemsSource = null;
            
            DbRepository db = new();
            var allSpecies = await db.GetAllSpecies();
            lstSpecies.ItemsSource = allSpecies;
        }
        /// <summary>
        /// Displays all Animals in listbox in View area
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
        /// Event for when user chooses an AnimalClass in listbox in View area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void lstAnimalClass_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AnimalClass animalClass = (AnimalClass)lstAnimalClass.SelectedItem;
            DbRepository db = new DbRepository();
            var matchingSpecies = await db.GetAllSpeciesFromClass(animalClass.Class_id);
            lstSpecies.ItemsSource = matchingSpecies;

            txtCurrentClass.Text = lstAnimalClass.SelectedItem.ToString();
        }
        /// <summary>
        /// Event for when user chooses a Species in listbox in View area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void lstSpecies_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Species species = (Species)lstSpecies.SelectedItem;
            DbRepository db = new DbRepository();
            var matchingAnimals = await db.GetAnimalsBySpeciesId(species.Species_id);
            lstAnimal.ItemsSource = matchingAnimals;
            var matchingClasses = await db.GetClassById(species.Class_id);
            lstAnimalClass.SelectedItem = matchingClasses;  

            txtCurrentSpecies.Text = lstSpecies.SelectedItem.ToString();
        }
        /// <summary>
        /// Presents which Species and Class that a selected Animal belongs to in View area
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

            //List <Species> species = new List<Species>();
            //species.Add(matchingSpecies); 
            //lstSpecies.ItemsSource = species;

            lstSpecies.Items.Add(matchingSpecies);
            lstAnimalClass.Items.Add(matchingClasses);

            txtCurrentName.Text = lstAnimal.SelectedItem.ToString();
            txtCurrentSpecies.Text = matchingSpecies.ToString(); 
            txtCurrentClass.Text = matchingClasses.ToString();

        }
        private async void rbtnSortByAsc_Checked(object sender, RoutedEventArgs e)
        {
            DbRepository db = new();
            var allAnimals = await db.GetAllAnimals();
            db.SortAnimalByNameAcs();
            lstAnimal.ItemsSource = null;
            lstAnimal.ItemsSource = db.animals;
        }
        private async void rbtnSortByDesc_Checked(object sender, RoutedEventArgs e)
        {
            DbRepository db = new();
            var allAnimals = await db.GetAllAnimals();
            db.SortAnimalByNameDecs();
            lstAnimal.ItemsSource = null;
            lstAnimal.ItemsSource = db.animals;

        }


        #endregion

        #region UPDATE

        private async void cbxEditAnimalClass_DropDownOpened(object sender, EventArgs e)
        {
            DbRepository db = new();
            var selectClass = await db.GetAllClasses();
            cbxEditAnimalClass.ItemsSource = selectClass;

        }

        private async void cbxEditSpecies_DropDownOpened(object sender, EventArgs e)
        {
            DbRepository db = new();
            var allSpecies = await db.GetAllSpecies();           
            cbxEditSpecies.ItemsSource = allSpecies;
        }

        //first draft of edit animal function
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            string newAnimalName = txtEditName.Text;

        }

        #endregion

        #region DELETE

        private async void btnRemoveAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DbRepository db = new();
                var selectedAnimal = (Animal)cbxRemoveAnimal.SelectedItem;
                var deletedAnimal = await db.DeleteAnimal(selectedAnimal);
                var animalClass = await db.GetClassById(selectedAnimal.Class_id);
                var animalSpecies = await db.GetSpeciesById(selectedAnimal.Species_id);
                MessageBox.Show($"Du har nu tagit bort djuret {selectedAnimal} som är en/ett {animalSpecies} som tillhör {animalClass} från databasen");

            }
            catch (Exception ex)
            { 
                MessageBox.Show(ex.Message);
                
            }
           

        }

        private async void cbxRemoveAnimal_DropDownOpened(object sender, EventArgs e)
        {
            DbRepository db = new DbRepository();
            var getAllAnimals = await db.GetAllAnimals();
            cbxRemoveAnimal.ItemsSource = getAllAnimals;
        }

        private async void cbxRemoveClass_DropDownOpened(object sender, EventArgs e)
        {
            DbRepository db = new DbRepository();
            var getAllClasses = await db.GetAllClasses();
            cbxRemoveClass.ItemsSource = getAllClasses;
        }

        private async void cbxRemoveSpecies_DropDownOpened(object sender, EventArgs e)
        {
            DbRepository db = new DbRepository();
            var getAllSpecies = await db.GetAllSpecies();
            cbxRemoveSpecies.ItemsSource = getAllSpecies;
        }
        #endregion



        private void btnClearLists_Click(object sender, RoutedEventArgs e)
        {
            txtGetAnimal.Clear(); 
            
            lstAnimalClass.ItemsSource = null; //should itemSource=null or items.Clear  be used? Or could both be needed to empty listboxes?
            lstSpecies.ItemsSource= null;
            lstAnimal.ItemsSource = null;
            lstAnimalClass.Items.Clear();
            lstSpecies.Items.Clear();
            lstAnimal.Items.Clear();

            txtCurrentClass.Clear();
            txtCurrentSpecies.Clear();
            txtCurrentName.Clear();
            cbxEditAnimalClass.ItemsSource = null;
            cbxEditSpecies.ItemsSource = null;
            
        }

      
        private async void btnRemoveAnimalClass_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DbRepository db = new();
                var selectedAnimalClass = (AnimalClass)cbxRemoveClass.SelectedItem;
                var animalsByClass = await db.GetAnimalsByClassId(selectedAnimalClass.Class_id);
                var speciesByClass = await db.GetAllSpeciesFromClass(selectedAnimalClass.Class_id);
                if (animalsByClass != null || speciesByClass != null)
                {
                    var answer = MessageBox.Show("Du kan inte radera en klass som inehåller arter och/eller djur utan att först ta bort alla djuren och/eller arterna. Vill du göra det?", "Otilåten radering av art", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                    if (answer == MessageBoxResult.Yes)
                    {
                        db.DeleteAllAnimalsInClass(selectedAnimalClass); // if there are animals but no class, do i get an error trying to erase both?
                        db.DeleteAllSpeciesinClass(selectedAnimalClass);
                        db.DeleteAnimalClass(selectedAnimalClass);
                        MessageBox.Show($"Du har tagit bort {selectedAnimalClass} samt arter och djur som tillhör den");

                    }
                }
                else
                {
                    db.DeleteAnimalClass(selectedAnimalClass);
                    MessageBox.Show($"Du har nu tagit bort {selectedAnimalClass}");
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }

        }

        private async void btnRemoveSpecies_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                DbRepository db = new();
                var selectedSpecies = (Species)cbxRemoveSpecies.SelectedItem;
                var animalsBySpecies = await db.GetAnimalsBySpeciesId(selectedSpecies.Species_id);
                if (animalsBySpecies != null)
                {
                    var answer = MessageBox.Show("Du kan inte radera en art som inehåller djur utan att först ta bort alla djuren. Vill du göra det?", "Otilåten radering av art", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                    if (answer == MessageBoxResult.Yes)
                    {
                        db.DeleteAllAnimalsInSpecies(selectedSpecies);
                        db.DeleteSpecies(selectedSpecies);
                        MessageBox.Show($"Du har nu tagit bort {selectedSpecies} och alla tillhörande djur");
                    }

                }
                else
                {
                    db.DeleteSpecies(selectedSpecies);
                    MessageBox.Show($"Du har nu tagit bort {selectedSpecies}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); 
            }


        }
    }

    
}
