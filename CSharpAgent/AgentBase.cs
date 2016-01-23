using PlanetWars.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CSharpAgent
{
    public class AgentBase
    {
        private bool _isRunning = false;
        private readonly HttpClient _client = null;

        private List<MoveRequest> _pendingMoveRequests = new List<MoveRequest>();

        protected long TimeToNextTurn { get; set; }
        protected int GameId { get; set; }

        // string guid that acts as an authorization token, definitely not crypto secure
        public string AuthToken { get; set; }
        public string Name { get; set; }

        public AgentBase(string name, string endpoint)
        {
            Name = name;
            // connect to api and handle gzip compressed messasges
            _client = new HttpClient() { BaseAddress = new Uri(endpoint) };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        protected async Task<LogonResult> Logon()
        {
            var response = await _client.PostAsJsonAsync("api/game/logon", new LogonRequest()
            {
                AgentName = Name
            });
            var result = await response.Content.ReadAsAsync<LogonResult>();
            AuthToken = result.AuthToken;
            GameId = result.GameId;
            Console.WriteLine("Your game Id is " + result.GameId);
            return result;
        }

        protected async Task<StatusResult> UpdateGameState()
        {
            var response = await _client.PostAsJsonAsync("api/game/status", new StatusRequest()
            {
                AuthToken = AuthToken,
                GameId = GameId
            });
            var result = await response.Content.ReadAsAsync<StatusResult>();
            
            return result;
        }

        protected async Task<List<MoveResult>> SendUpdate(List<MoveRequest> moveCommands)
        {
            var results = new List<MoveResult>();
            foreach (var moveCommand in moveCommands)
            {
                // Console.WriteLine(string.Format("posting move {0} for elevator {1}", moveCommand.Direction, moveCommand.ElevatorId));
                var response = await _client.PostAsJsonAsync("api/game/move", moveCommand);
                var result = await response.Content.ReadAsAsync<MoveResult>();
                Console.WriteLine(result.Message);
                results.Add(result);
            }

            return results;
        }

        public async Task Start()
        {
            await Logon();
            if (!_isRunning)
            {
                _isRunning = true;
                while (_isRunning)
                {

                    var gs = await UpdateGameState();
                    if (gs.IsGameOver)
                    {
                        _isRunning = false;
                        Console.WriteLine("Game Over!");
                        Console.WriteLine(gs.Status);
                        _client.Dispose();
                        break;
                    }

                    Update(gs);
                    var ur = await SendUpdate(this._pendingMoveRequests);
                    this._pendingMoveRequests.Clear();
                    if (TimeToNextTurn > 0)
                    {
                        await Task.Delay((int)(TimeToNextTurn));
                    }
                }
            }
        }

        private Task SendUpdate(object _pendingMoveRequests)
        {
            throw new NotImplementedException();
        }

        private void Update(StatusResult gs)
        {
            throw new NotImplementedException();
        }
    }
}
