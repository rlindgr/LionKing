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
        public int Class_id { get; set; }
        public int Species_id { get; set; }


        public override string ToString()
        {
            return base.ToString();
        }
    }
}
