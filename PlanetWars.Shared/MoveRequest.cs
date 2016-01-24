using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetWars.Shared
{
    public class MoveRequest
    {
        public MoveRequest() { }

        public string AuthToken { get; set; }
        public int DestinationPlanetId { get; set; }
        public int GameId { get; set; }
        public int NumberOfShips { get; set; }
        public int SourcePlanetId { get; set; }
    }
}
