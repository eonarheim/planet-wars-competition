using CSharpAgent;
using PlanetWars.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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

        // todo planet generation

        private static int _MAXID = 0;
        private static readonly long START_DELAY = 10000; // 5 seconds
        private static readonly long PLAYER_TURN_LENGTH = 700; // 200 ms
        private static readonly long SERVER_TURN_LENGTH = 200; // 200 ms
        private static readonly int MAX_TURN = 200; // default 200 turns

        public static bool IsRunningLocally = HttpContext.Current.Request.IsLocal;
        public bool Running { get; private set; }
        public int Id { get; private set; }
        public Random Random { get; set; }
        public int Turn { get; private set; }
        public bool Waiting { get; internal set; }
        public bool GameOver { get; private set; }
        public int TIME_TO_WAIT { get; private set; }
        public bool Processing { get; private set; }

        private HighFrequencyTimer _gameLoop = null;
        public ConcurrentDictionary<string, Player> Players = new ConcurrentDictionary<string, Player>();
        public ConcurrentDictionary<string, Player> AuthTokens = new ConcurrentDictionary<string, Player>();

        private DateTime gameStart;
        private DateTime endPlayerTurn;
        private DateTime endServerTurn;

        private bool _started;
        private bool serverComplete;

        public Game(int? seed, int? id) : base()
        {
            if (seed != null && seed.HasValue)
            {
                Random = new Random(seed.Value);
            }

            if (id != null && id.HasValue)
            {
                Id = id.Value;
            }
        }



        public Game()
        {
            if (Random == null)
            {
                Random = new Random();
            }

            Id = _MAXID++;

            Turn = 0;
            Running = false;
            _started = false;
            _gameLoop = new HighFrequencyTimer(60, this.Update);
            gameStart = DateTime.UtcNow.AddMilliseconds(START_DELAY);
            endPlayerTurn = gameStart.AddMilliseconds(PLAYER_TURN_LENGTH);
            endServerTurn = endPlayerTurn.AddMilliseconds(SERVER_TURN_LENGTH);
        }

        public MoveResult MoveFleet(MoveRequest request)
        {
            throw new NotImplementedException();
        }

        public LogonResult LogonPlayer(string playerName)
        {
            var result = new LogonResult();
            if (!Players.ContainsKey(playerName))
            {
                var newPlayer = new Player()
                {
                    AuthToken = System.Guid.NewGuid().ToString(),
                    PlayerName = playerName
                };

                var success = Players.TryAdd(playerName, newPlayer);
                var success2 = AuthTokens.TryAdd(newPlayer.AuthToken, newPlayer);

                if (success && success2)
                {
                    System.Diagnostics.Debug.WriteLine("Player logon [{0}]:[{1}]", newPlayer.PlayerName,
                        newPlayer.AuthToken);
                }


                result.AuthToken = newPlayer.AuthToken;
                result.GameId = Id;
                result.GameStart = this.gameStart;
                result.Success = true;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Player {0} already logged on!", playerName);
            }
            result.GameId = Id;

            return result;
        }

        public void StartDemoAgent(LogonResult demoResult, string playerName)
        {
            var agentTask = Task.Factory.StartNew(() =>
            {
                string endpoint = "";
                if (IsRunningLocally)
                {
                    endpoint = "http://localhost:3193";
                }
                else {
                    endpoint = "http://planetwars.azurewebsites.net";
                }
                AgentBase sweetDemoAgent = new AgentBase(playerName, endpoint);
                sweetDemoAgent.Start().Wait();
            });
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
            var currentTime = DateTime.UtcNow;
            if (this.Waiting)
            {

                if (currentTime > gameStart)
                {
                    this.Waiting = false;
                }
                return;
            }

            if (GameOver)
            {
                this.Stop();
                return;
            }

            // check if we are in the server window
            if (currentTime > endPlayerTurn)
            {
                // server processing                
                Processing = true;

                // Process ships movement

                // Update ship counts

                // Resolve collisions

                // Update scores

                // Turn complete
                Turn++;                
                endPlayerTurn = currentTime.AddMilliseconds(PLAYER_TURN_LENGTH);
                endServerTurn = endPlayerTurn.AddMilliseconds(SERVER_TURN_LENGTH);
                Processing = false;
                System.Diagnostics.Debug.WriteLine($"Game {Id} : Turn {Turn} : Next Turn Start {endServerTurn.Subtract(DateTime.UtcNow).TotalMilliseconds}ms");

                // TODO UPDATE VIZ
            }


            if (Turn >= MAX_TURN)
            {
                this.GameOver = true;
            }

        }

        public StatusResult GetStatus(StatusRequest request)
        {
            return new StatusResult()
            {


                IsGameOver = this.GameOver,
                CurrentTurn = Turn,
                NextTurnStart = endServerTurn,
                EndOfCurrentTurn = endPlayerTurn


            };
        }
    }
}
