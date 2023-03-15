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



    }
}
