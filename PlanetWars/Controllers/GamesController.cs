using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PlanetWars.Models;
using PlanetWars.Server;

namespace PlanetWars.Controllers
{
    public class GamesController : ApiController
    {
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
    }
}
