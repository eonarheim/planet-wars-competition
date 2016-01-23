using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetWars.Shared
{
    public class StatusResult : BaseResult
    {
        public bool IsGameOver { get; set; }
        public bool Status { get; set; }
    }
}
