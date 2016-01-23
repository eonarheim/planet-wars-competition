using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetWars.Server
{
    public class GameManager
    {
        private static readonly GameManager _instance = new GameManager();
        public Dictionary<int, Game> Games { get; set; }

        public static GameManager Instance
        {
            get { return _instance; }
        }

        private GameManager()
        {
            if (Games == null)
            {
                Games = new Dictionary<int, Game>();
            }
        }

        public Game GetNewGame()
        {
            var game = new Game();
            Games.Add(game.Id, game);
            return game;
        }

        
        public LogonResult Execute(LogonCommand command)
        {
            var game = GetNewGame();
            var demoResult = game.LogonPlayer("DemoAgent");
            game.StartDemoAgent(demoResult, "DemoAgent");
            var result = game.LogonPlayer(command.AgentName);
            game.Start();

            return result;
        }

        
        public LogonResult Execute(LogonP1Command command)
        {
            var game = GetNewGame();
            game.Waiting = true;
            game.Start();
            return game.LogonPlayer(command.AgentName);
        }

        
        public LogonResult Execute(LogonP2Command command)
        {
            var game = Games[command.GameId];
            LogonResult result = null;
            if (game.Waiting)
            {
                result = game.LogonPlayer(command.AgentName);
                game.Waiting = false;
            }
            return result;
        }

        public KillResult Execute(KillCommand command)
        {
            return new KillResult();
        }

        [ValidateAuthToken]
        public StatusResult Execute(StatusCommand command)
        {
            var game = Games[command.GameId];
            var result = game.GetStatus(command);
            return result;
        }
    }
}
