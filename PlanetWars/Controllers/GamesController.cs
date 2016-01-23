using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PlanetWars.Models;
using PlanetWars.Server;
using PlanetWars.Shared;
using PlanetWars.Validators;

namespace PlanetWars.Controllers
{
    public class GamesController : ApiController
    {
        private readonly GameManager _gameManager;

        public GamesController()
        {
            _gameManager = GameManager.Instance;
        }

        // GET api/games
        public IEnumerable<GameSession> Get()
        {
            var games = GameManager.Instance.Games;

            foreach (var game in games)
            {
                yield return new GameSession()
                {
                    GameId = game.Key,
                    Players = game.Value.Players.Select(x => x.Key).ToArray()
                };
            }
        }

        public Game Get(int sessionId)
        {
            return GameManager.Instance.Games[sessionId];
        }

        /// <summary>
        /// Initiates an agent logon with the simulation server by name. Once an agent is logged on, 
        /// a logon result is returned with the id and starting time of the next game.
        /// </summary>
        /// <param name="agentName"></param>
        /// <returns>LogonResult</returns>
        [HttpPost]
        [Route("api/logon")]
        public BaseResult<LogonResult> Logon(LogonRequest logon)
        {
            var validator = new LogonRequestValidator();
            var results = validator.Validate(logon);
            if (results.IsValid)
            {
                return _gameManager.Execute(logon);
            }
            else
            {
                return BaseResult<LogonResult>.Fail(errors:results.Errors.Select(e => e.ErrorMessage));
            }
        }

        [HttpPost]
        [Route("api/status")]
        public BaseResult<StatusResult> Status(StatusRequest status)
        {
            var validator = new StatusRequestValidator();
            var results = validator.Validate(status);
            if (results.IsValid)
            {
                return _gameManager.Execute(status);
            }
            else
            {
                return BaseResult<StatusResult>.Fail(errors: results.Errors.Select(e => e.ErrorMessage));
            }
        }


        [HttpPost]
        [Route("api/move")]
        public BaseResult<MoveResult> Move(MoveRequest move)
        {
            var validator = new MoveRequestValidator();
            var results = validator.Validate(move);

            if (results.IsValid)
            {
                return _gameManager.Execute(move);
            }
            else
            {
                return BaseResult<MoveResult>.Fail(errors: results.Errors.Select(e => e.ErrorMessage));
            }
        }

    }
}
