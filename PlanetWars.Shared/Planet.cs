using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetWars.Shared
{
    public class Planet
    {
        public int Id { get; set; }
        public int NumberOfShips { get; set; }
        public int Size { get; set; }
        public Point Position { get; set; }
        public int OwnerId { get; set; } = -1;
    }
}
