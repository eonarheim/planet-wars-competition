using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetWars.Shared
{
    public class StatusResult : BaseResult<StatusResult>
    {
        public bool IsGameOver { get; set; }
        public bool Status { get; set; }
        public int PlayerA { get; set; }
        public int PlayerB { get; set; }

        public int CurrentTurn { get; set; }
        public DateTime EndOfCurrentTurn { get; set; }
        public DateTime NextTurnStart { get; set; }

        public int PlayerTurnLength {get; set;}
        public int ServerTurnLength { get; set;}

        public List<Planet> Planets { get; set;}
        public List<Fleet> Fleets { get; set; }
    }
}
