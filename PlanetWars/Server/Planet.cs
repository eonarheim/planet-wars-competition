using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetWars.Server
{
    public class Planet
    {        
        public int Id { get; set; }
        public int NumberOfShips { get; set; }
        public Vector Position { get; set; }

        /// <summary>
        /// Owner string as guid
        /// </summary>
        public string Owner { get; set; }

    }
}
