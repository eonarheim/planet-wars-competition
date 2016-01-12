using PlanetWars.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetWars.Server
{
    public interface IGame
    {
        LogonResult LogonPlayer(string playerName);
        void Update(long delta);
        void Start();
        void Stop();
    }

    public class Game : IGame
    {
        public bool Running { get; private set; }
        private HighFrequencyTimer _gameLoop = null;

        public Game()
        {
            // todo init
        }

        public LogonResult LogonPlayer(string playerName)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            Running = true;
            _gameLoop.Start();
        }

        public void Stop()
        {
            Running = false;
            _gameLoop.Stop();
        }

        public void Update(long delta)
        {
            throw new NotImplementedException();
        }
    }
}
