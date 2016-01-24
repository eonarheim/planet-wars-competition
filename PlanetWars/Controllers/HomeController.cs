using PlanetWars.Models;
using PlanetWars.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlanetWars.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult Game(int id)
        {
            if (GameManager.Instance.Games.ContainsKey(id))
            {

                var gameModel = new GameSession()
                {
                    GameId = id,
                    Players = GameManager.Instance.Games[id].Players.Select(p => p.Key).ToArray()
                };
                return View(gameModel);
            }
            throw new HttpException(404, "Game not found");
        }
    }
}
