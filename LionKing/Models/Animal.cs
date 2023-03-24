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
        public AnimalClass Class { get; set; } 

        public int Class_id { get; set; } 
        public int Species_id { get; set; }


        public Animal()
        {
            Animal_id = Animal_id;          
            Animal_name = Animal_name;
            Class_id = Class_id;
            Species_id= Species_id;

        }

        public override string ToString()
        {
            if(Animal_name == null) 
            {
                return ("(namnlöst djur)");
            }
            else
            {
                return $"{Animal_name}";
            }
            
        }


    }
}
