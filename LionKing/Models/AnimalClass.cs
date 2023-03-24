using System;
using System.CodeDom;
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
        public int Class_id { get; set; } 
        public string Class_name { get; set; } //NOT NULL
        


        
        public AnimalClass()
        {
            Class_id = Class_id;
            Class_name = Class_name;            
        }

        public override string ToString()
        {
            return $"{Class_name}";
        }

    }

}
