using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetWars.Shared
{
    public class LogonResult : BaseResult<LogonResult>
    {
        public string AuthToken { get; set; }
        public int GameId { get; set; }
        // Game Start Date Time in UTC
        public DateTime GameStart { get; set; }
    }
}
