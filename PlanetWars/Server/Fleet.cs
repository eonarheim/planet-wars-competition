using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetWars.Server
{
    public class Fleet
    {
        public int Id { get; set; }
        public int NumberOfShips { get; set; }
        public int NumberOfTurnsToDestination { get; set; }
        public Planet Source { get; set; }
        public Planet Destination { get; set; }
    }
}
