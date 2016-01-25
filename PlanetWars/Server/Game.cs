using AutoMapper;
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
        void Update(DateTime currentTime);
        void Start();
        void Stop();
    }

    public class Game : IGame
    {

        // todo planet generation

        private static int _MAXID = 0;
        private int _MAXPLAYERID = 1;
        private int _MAXPLANETID = 0;
        private int _MAXFLEETID = 0;
        private int _NUM_PLANETS = 4;
        public static readonly long START_DELAY = 10000; // 5 seconds
        public static readonly long PLAYER_TURN_LENGTH = 700; // 200 ms
        public static readonly long SERVER_TURN_LENGTH = 200; // 200 ms
        public static readonly int MAX_TURN = 200; // default 200 turns

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

        private List<Planet> _planets = new List<Planet>();
        private List<Fleet> _fleets = new List<Fleet>();

        private DateTime gameStart;
        private DateTime endPlayerTurn;
        private DateTime endServerTurn;

        private object synclock = new object();

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
            GenerateMap();

            UpdateTimeInfo(DateTime.UtcNow);
        }

        public void UpdateTimeInfo(DateTime currentTime)
        {
            gameStart = currentTime.AddMilliseconds(START_DELAY);
            endPlayerTurn = gameStart.AddMilliseconds(PLAYER_TURN_LENGTH);
            endServerTurn = endPlayerTurn.AddMilliseconds(SERVER_TURN_LENGTH);
        }

        public void SetPlanets(List<Planet> planets)
        {
            this._planets = planets;
        }

        public void SetFleets(List<Fleet> fleets)
        {
            this._fleets = fleets;
        }

        public List<Fleet> GetFleets()
        {
            return this._fleets;
        }

        private void GenerateMap()
        {
            for (var i = 0; i < _NUM_PLANETS; i++)
            {
                _planets.Add(new Planet()
                {
                    Id = _MAXPLANETID++,
                    OwnerId = -1,
                    Position = new Point(i * 4, i * 4),
                    NumberOfShips = 40
                });
            }
            _planets[0].OwnerId = 1;
            _planets[_NUM_PLANETS - 1].OwnerId = 2;
        }
        
        public MoveResult MoveFleet(MoveRequest request)
        {

            var result = new MoveResult();
            
            // non-zero ships
            if(request.NumberOfShips <= 0) {
                result.Success = false;
                result.Message = "Can't send a zero fleet";
                return result;
            }

            var validSourcePlanets = _getPlanetsForPlayer(request.AuthToken);

            // A planet of the requested source ID exists, belongs to that planer, AND it has enough ships
            var sourceValid = validSourcePlanets.FirstOrDefault(p => p.Id == request.SourcePlanetId && request.NumberOfShips <= p.NumberOfShips);

            // A planet of the requested destination ID exists
            var destinationValid = _planets.FirstOrDefault(p => p.Id == request.DestinationPlanetId);


            if (sourceValid != null && destinationValid != null)
            {
                lock (synclock)
                {
                    // Subtract ships from planet
                    sourceValid.NumberOfShips -= request.NumberOfShips;

                    // Build fleet
                    var newFleet = new Fleet()
                    {
                        Id = _MAXFLEETID++,
                        OwnerId = _authTokenToId(request.AuthToken),
                        Source = sourceValid,
                        Destination = destinationValid,
                        NumberOfShips = request.NumberOfShips,
                        NumberOfTurnsToDestination = (int)Math.Ceiling(sourceValid.Position.Distance(destinationValid.Position))
                    };

                    _fleets.Add(newFleet);

                    result.Fleet = Mapper.Map<Shared.Fleet>(newFleet);
                    result.Success = true;
                }
            }
            else
            {
                result.Success = false;
                result.Message = "Invalid move command, check if the planet of the requested source/dest ID exists, belongs to that player, AND it has enough ships.";
            }


            return result;
        }

        private int _authTokenToId(string authToken)
        {
            var player = Players.Values.Where(p => p.AuthToken == authToken).FirstOrDefault();
            if (player != null)
            {
                return player.Id;
            }
            else
            {
                throw new ArgumentException("Invalid auth token, no player exists for that auth token!!!!");
            }

        }

        private List<Planet> _getPlanetsForPlayer(string authToken)
        {
            var id = _authTokenToId(authToken);
            return _getPlanetsForPlayer(id);
        }

        private List<Planet> _getPlanetsForPlayer(int id)
        {
            var planets = _planets.Where(p => p.OwnerId == id).ToList();
            return planets;
        }

        private List<Fleet> _getFleetsForPlayer(string authToken)
        {
            var id = _authTokenToId(authToken);
            return _getFleetsForPlayer(id);
        }

        private List<Fleet> _getFleetsForPlayer(int id)
        {
            var fleets = _fleets.Where(f => f.OwnerId == id).ToList();
            return fleets;
        }

        private int _getPlayerScore(int id)
        {
            return _getFleetsForPlayer(id).Sum(f => f.NumberOfShips) + _getPlanetsForPlayer(id).Sum(p => p.NumberOfShips);
        }

        public LogonResult LogonPlayer(string playerName)
        {
            var result = new LogonResult();
            if (!Players.ContainsKey(playerName))
            {
                var newPlayer = new Player()
                {
                    AuthToken = System.Guid.NewGuid().ToString(),
                    PlayerName = playerName,
                    Id = _MAXPLAYERID++
                };

                var success = Players.TryAdd(playerName, newPlayer);
                var success2 = AuthTokens.TryAdd(newPlayer.AuthToken, newPlayer);

                if (success && success2)
                {
                    System.Diagnostics.Debug.WriteLine("Player logon [{0}]:[{1}]", newPlayer.PlayerName,
                        newPlayer.AuthToken);
                }


                result.AuthToken = newPlayer.AuthToken;
                result.Id = newPlayer.Id;
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

        public void StartDemoAgent(string playerName)
        {
            var agentTask = Task.Factory.StartNew(() =>
            {
                string endpoint = "";
                if (IsRunningLocally)
                {
                    endpoint = "http://localhost:52802";
                }
                else {
                    endpoint = "http://planetwars.azurewebsites.net";
                }
                var sweetDemoAgent = new Agent(playerName, endpoint);
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


        public void Update(DateTime currentTime)
        {
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
            if (currentTime >= endPlayerTurn)
            {
                // server processing                
                Processing = true;

                // Grow ships on planets
                foreach (var planet in _planets)
                {
                    // if the planet is not controlled by neutral update
                    if (planet.OwnerId != -1)
                    {
                        planet.NumberOfShips += planet.GrowthRate;
                    }
                }

                // Send fleets 
                foreach (var fleet in _fleets.ToList())
                {
                    // travel 1 unit distance each turn
                    fleet.NumberOfTurnsToDestination--;
                }               
               
                // Resolve planet battles
                foreach(var planet in _planets.ToList())
                {
                    var combatants = new Dictionary<int, int>();
                    combatants.Add(planet.OwnerId, planet.NumberOfShips);
                    // find fleets destined for this planet
                    var fleets = _fleets.ToList().Where(f => f.Destination.Id == planet.Id && f.NumberOfTurnsToDestination <= 0).ToList();
                    foreach(var fleet in fleets)
                    {
                        if (combatants.ContainsKey(fleet.OwnerId))
                        {
                            combatants[fleet.OwnerId] += fleet.NumberOfShips;
                        }
                        else
                        {
                            combatants.Add(fleet.OwnerId, fleet.NumberOfShips);
                        }
                    }

                    if(fleets.Count <= 0)
                    {
                        continue;
                    }

                    KeyValuePair<int, int> second = new KeyValuePair<int, int>(1, 0);
                    KeyValuePair<int, int> winner = new KeyValuePair<int, int>(2, 0);
                    foreach(var keyval in combatants)
                    {
                        if(keyval.Value > second.Value)
                        {
                            if(keyval.Value > winner.Value)
                            {
                                second = winner;
                                winner = keyval;
                            }
                            else
                            {
                                second = keyval;
                            }                            
                        }
                    }

                    if(winner.Value > second.Value)
                    {
                        planet.NumberOfShips = winner.Value - second.Value;
                        planet.OwnerId = winner.Key;
                    }
                    else
                    {
                        planet.NumberOfShips = 0;
                    }
                }

                // remove finished fleets
                lock (synclock)
                {
                    _fleets.RemoveAll(f => f.NumberOfTurnsToDestination <= 0);
                }

                // Check game over conditions
                if(_planets.All(p => p.OwnerId == 1) || _planets.All(p => p.OwnerId == 2))
                {
                    // player has won
                    this.GameOver = true;
                }

                // Turn complete
                Turn++;
                endPlayerTurn = currentTime.AddMilliseconds(PLAYER_TURN_LENGTH);
                endServerTurn = endPlayerTurn.AddMilliseconds(SERVER_TURN_LENGTH);
                Processing = false;
                System.Diagnostics.Debug.WriteLine($"Game {Id} : Turn {Turn} : Next Turn Start {endServerTurn.Subtract(DateTime.UtcNow).TotalMilliseconds}ms");
            }


            if (Turn >= MAX_TURN)
            {
                this.GameOver = true;
            }

        }

        public StatusResult GetStatus(StatusRequest request)
        {
            var status = new StatusResult()
            {
                IsGameOver = this.GameOver,
                CurrentTurn = Turn,
                NextTurnStart = endServerTurn,
                EndOfCurrentTurn = endPlayerTurn,
                PlayerTurnLength = (int)PLAYER_TURN_LENGTH,
                ServerTurnLength = (int)SERVER_TURN_LENGTH,
                Planets = _planets.ToList().Select(p => Mapper.Map<Shared.Planet>(p)).ToList(),
                Fleets = _fleets.ToList().Select(f => Mapper.Map<Shared.Fleet>(f)).ToList(),
                PlayerA = 1,
                PlayerAScore = _getPlayerScore(1),
                PlayerB = 2,
                PlayerBScore = _getPlayerScore(2)
            };
            return status;
        }
    }
}
