using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlanetWars.Shared;

namespace CSharpAgent
{
    public class Agent : AgentBase
    {
        public Agent(string name, string endpoint) : base(name, endpoint){}

        public override void Update(StatusResult gs)
        {
            // do cool ai stuff
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}]Current Turn: {gs.CurrentTurn}");
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}]End of Current Turn: {gs.EndOfCurrentTurn.ToLocalTime().ToLongTimeString()}");
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}]Start of Next Turn: {gs.NextTurnStart.ToLocalTime().ToLongTimeString()}");
        }
    }
}
