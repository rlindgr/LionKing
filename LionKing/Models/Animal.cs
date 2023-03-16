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
       

        public Species Species { get; set; }
        //public AnimalClass Class { get; set; } kanske är bättre att ha den i species eftersom man då ändå får med den informationen hit. 

        public Animal(string name, Species species)
        {
            name = Animal_name;
            species = Species;

        }
        public Animal()
        {
            
        }

    }
}
