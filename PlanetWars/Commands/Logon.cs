using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetWars.Commands
{
    public class LogonResult
    {
        public string AuthToken { get; internal set; }
        public object GameId { get; internal set; }
        public int GameStart { get; internal set; }
        public bool Success { get; set; }
    }


}
