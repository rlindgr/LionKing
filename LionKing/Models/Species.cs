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
        public int Class_id { get; set; }

        public AnimalClass Class { get; set; } 


        public Species(int id, string name, string latinName, AnimalClass animal_class)
        {
            id = Species_id;
            name = Species_name;
            latinName = Latin_name;
            //animal_class = Class_id; - I get an error from this code, not sure why?
            
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
            Species_id = Species_id;
            Species_name = Species_name;
            Latin_name = Latin_name;
            Class_id = Class_id;
            /*Class.Class_id = Class.Class_id;*/ //- Can this be replaced by code above? I get error: System.NullReferenceException: 'Object reference not set to an instance of an object.'


        }
        public Species(string name, string latinName, int classId)
        {
            name = Species_name;
            latinName = Latin_name;
            classId = Class.Class_id;
        }


        public override string ToString()
        {
            if (Latin_name == null)
            {
                return ($"{Species_name}, Latinskt namn saknas"); // not working, why?
            }
            else
            {
                return $"""{Species_name} "{Latin_name}" """;

            }
        }
    }
}
