using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionKing.Models
{
    public class Species
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int Species_id { get; set; }
        public string? Species_name { get; set; }
        public string? Latin_name { get; set; }
       

        public AnimalClass Class { get; set; }

        public Species(int id, string name, string latinName, AnimalClass animal_class)
        {
            id = Species_id;
            name = Species_name;
            latinName = Latin_name;
            animal_class = Class;
            
        }
        public Species(string name, string latinName)
        {
            object newName = (object)name ?? DBNull.Value;
            object newLatinName = (object)latinName ?? DBNull.Value;
            newName = Species_name;
            newLatinName = Latin_name;
        }
        public Species()
        {
            
        }
        public Species(string name, string latinName, int classId)
        {
            name = Species_name;
            latinName = Latin_name;
            classId = Class.Class_id;
        }


        public override string ToString()
        {
            return $"{Species_name}";
        }

    }
}
