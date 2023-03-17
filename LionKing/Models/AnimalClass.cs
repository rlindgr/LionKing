using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LionKing.Models
{
    public class AnimalClass
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int Class_id { get; set; } // do i have to set this to nullable in c# in order to create a new clas so that i can retriew the class id from database?

        public string Class_name { get; set; } //NOT NULL
        public AnimalClass(int id, string name)
        {
            id = Class_id;
            name = Class_name;
        }
        public AnimalClass(string name)
        {
            name = Class_name;
        }
       
        



    }
}
