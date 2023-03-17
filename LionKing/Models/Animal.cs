using LionKing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionKing
{
    public class Animal
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int Animal_id { get; set; }
        public string? Animal_name { get; set; }

        public static List<Animal> AnimalList = new List<Animal>(); //To store all animals created so that we easily can acces the id number of each animal.
                                                                    //Each animal we created retrives it's id number from the database (sets it to Animal_id).
                                                                    //So here we can have all the information stored. Or is it better to retrive all the infornation from the database eachtime?
     

        public Species Species { get; set; }
        public AnimalClass Class { get; set; } 


        public Animal(string name, Species species, AnimalClass animalClass)
        {
            name = Animal_name;
            species = Species;
            animalClass = Class;

        }
        public Animal()
        {
            
        }

    }
}
