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
            cbxSelectClass.ItemsSource = selectClass; // dont understand why this is not working? Why isn't animal classes shown in interface? 
                                                      // You still used cbxViewClass but the btn pressed was cbxSelectClass :)  
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
       


        #endregion

        #region READ

        /// <summary>
        /// Presents all AnimalClasses in Combobox in View area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cbxViewClass_DropDownOpened(object sender, EventArgs e)
        {
            DbRepository db = new();
            var allClasses = await db.GetAllClasses();
            cbxViewClass.ItemsSource = allClasses;


        }
        /// <summary>
        /// Presents all Species in Combobox in View area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ViewSpecies_DropDownOpened(object sender, EventArgs e)
        {
            DbRepository db = new();
            var allSpecies = await db.GetAllSpecies();
            cbxViewSpecies.ItemsSource = allSpecies;
        }
        /// <summary>
        /// Presents all Animals in Combobox in View area 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private async void cbxViewAnimal_DropDownOpened(object sender, EventArgs e)
        {
            DbRepository db = new();
            var allAnimals = await db.GetAllAnimals();
            cbxViewAnimal.ItemsSource = allAnimals;
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
                    lstAnimal.Items.Clear();
                    lstAnimal.Items.Add(animal.Animal_name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Trying to enable showing info about animals in listbox once an animalclass is shown - OBS! not working properly yet
        private void cbxViewClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DbRepository db = new();
            AnimalClass animalClass = new AnimalClass();
            //SelectClass(animalClass); 

            //How to we establish which animalclass is choosen? Can we reach/use class_id that is fetched in SelectClass_DropDownOpened somehow?
            //{
            //    var allSpecies = await db.GetAllSpeciesFromClass(1);
            //    lstAnimal.Items.Add(allSpecies);
            //}

            //cbxViewClass.SelectedItem = ()
            //ComboboxItem selectedCar = (ComboboxItem)comboBox1.SelectedItem;

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

            foreach (AnimalClass c in cbxViewClass.Items)
            {
                if (c.Class_id == animalClass.Class_id)
                {
                    int index = cbxViewClass.Items.IndexOf(c);
                    cbxViewClass.SelectedItem = c;
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
        }

       
    }

    
}
