using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionKing.Models
{
    public class AnimalClass
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int Class_id { get; set; }

        public string? Class_name { get; set; }
        public AnimalClass(int id, string name)
        {
            id = Class_id;
            name = Class_name;
        }



    }
}
