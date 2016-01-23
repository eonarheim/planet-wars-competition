using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetWars.Shared
{
    public class MoveRequest
    {
        public MoveRequest(string authToken, int gameId, int sourcePlanetId, int destinationPlanetId, int numberOfShips)
        {
            this.AuthToken = authToken;
            this.GameId = gameId;
            this.SourcePlanetId = sourcePlanetId;
            this.DestinationPlanetId = destinationPlanetId;
            this.NumberOfShips = numberOfShips;

        }

        public string AuthToken { get; private set; }
        public int DestinationPlanetId { get; private set; }
        public int GameId { get; private set; }
        public int NumberOfShips { get; private set; }
        public int SourcePlanetId { get; private set; }
    }
}
