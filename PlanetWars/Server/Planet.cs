using PlanetWars.Shared;
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
        public Point Position { get; set; }

        /// <summary>
        /// Owner string as guid, neutral player is -1
        /// </summary>
        public int OwnerId { get; set; } = -1;

    }
}
