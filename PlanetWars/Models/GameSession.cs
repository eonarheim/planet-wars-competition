using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanetWars.Models
{
    public class GameSession
    {
        public int GameId { get; set; }

        public IDictionary<int, string> Players { get; set; }
    }
}