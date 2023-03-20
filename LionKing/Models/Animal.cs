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
        public Species Species { get; set; } // can these values be null??
        public AnimalClass Class { get; set; } 


        public Animal(string name, Species species, AnimalClass animalClass, int animalId)
        {
            name = Animal_name;
            species = Species;
            animalClass = Class;
            animalId = Animal_id;
        }
        public Animal()
        {
            
        }

        public override string ToString()
        {
            return $"{Animal_name}";
        }
    }
}
