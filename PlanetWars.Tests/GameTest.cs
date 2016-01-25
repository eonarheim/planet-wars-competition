using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlanetWars.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PlanetWars.Tests
{
    public static class MappingExtensions
    {
        public static IMappingExpression<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        {
            var sourceType = typeof(TSource);
            var destinationType = typeof(TDestination);
            var existingMaps = Mapper.GetAllTypeMaps().First(x => x.SourceType.Equals(sourceType)
                && x.DestinationType.Equals(destinationType));
            foreach (var property in existingMaps.GetUnmappedPropertyNames())
            {
                expression.ForMember(property, opt => opt.Ignore());
            }
            return expression;
        }
    }

    [TestClass]
    public class GameTest
    {
        private Game game;
        private Shared.LogonResult p1Logon;
        private Shared.LogonResult p2Logon;
        private List<Planet> planets;
        private DateTime startTime;

        [TestInitialize]
        public void TestInit()
        {
            HttpContext.Current = new HttpContext(new HttpRequest(null, "http://tempuri.org", null), new HttpResponse(null));
            // Configure AutoMapper
            Mapper.CreateMap<Server.Planet, Shared.Planet>().IgnoreAllNonExisting();
            Mapper.CreateMap<Server.Fleet, Shared.Fleet>().ForMember(
                dest => dest.DestinationPlanetId,
                opt => opt.MapFrom(src => src.Destination.Id)).ForMember(
                dest => dest.SourcePlanetId,
                opt => opt.MapFrom(src => src.Source.Id)).IgnoreAllNonExisting();


            Mapper.AssertConfigurationIsValid();


            game = new Game();
            // start engine at 0 ticks 
            startTime = new DateTime(0);
            game.UpdateTimeInfo(startTime);
            p1Logon = game.LogonPlayer("p1");
            p2Logon = game.LogonPlayer("p2");

            planets = new List<Planet>()
            {
                new Planet()
                {
                    Id = 0,
                    OwnerId = p1Logon.Id,
                    NumberOfShips = 40,
                    GrowthRate = 0,
                    Position = new Shared.Point(1, 0)
                },
                new Planet()
                {
                    Id = 1,
                    OwnerId = -1, // neutral
                    NumberOfShips = 39,
                    GrowthRate = 0,
                    Position = new Shared.Point(2, 0)
                },
                new Planet()
                {
                    Id = 2,
                    OwnerId = p2Logon.Id, // neutral
                    NumberOfShips = 40,
                    GrowthRate = 0,
                    Position = new Shared.Point(3, 0)
                },
            };
            game.SetPlanets(planets);

        }

        [TestMethod]
        public void TestGameExists()
        {
            var game = new Game();
            Assert.IsNotNull(game, "Game should exist");
        }

        [TestMethod]
        public void TestPlayersCanLogon()
        {
            var game = new Game();
            var p1logon = game.LogonPlayer("p1");

            Assert.IsTrue(p1logon.Success, "p1 should be able to logon");
            Assert.IsNotNull(p1logon.AuthToken, "auth token should not be null");
            Assert.IsNotNull(p1logon.GameStart, "game start should not be null");
            Assert.AreEqual(p1logon.Id, 1, "First player should get #1 as ID");
            Assert.AreEqual(p1logon.GameId, 0, "Game ID should be 0");
        }

        [TestMethod]
        public void TestPlayerCanColonizePlanet()
        {
           
            var moveResult = game.MoveFleet(new Shared.MoveRequest()
            {
                AuthToken = p1Logon.AuthToken,
                GameId = p1Logon.GameId,
                NumberOfShips = 40,
                DestinationPlanetId = 1,
                SourcePlanetId = 0
            });

            // Move result should be valid
            Assert.IsTrue(moveResult.Success, "Valid move result should succeed");
            Assert.AreEqual(1, moveResult.Fleet.NumberOfTurnsToDestination, "Planet is 1 move away");
            Assert.AreEqual(0, planets[0].NumberOfShips, "Move should have removed the ships, planet has a growth rate of 0");

            // Tick engine forward to processing
            game.Update(startTime.AddMilliseconds(Game.START_DELAY + Game.PLAYER_TURN_LENGTH));

            // Check planets update the way they should
            Assert.AreEqual(0, game.GetFleets().Count, "Fleet should have arrived and been removed");
            Assert.AreEqual(1, planets[1].NumberOfShips, "Planet should have 1 ship on it after colonization");
            Assert.AreEqual(p1Logon.Id, planets[1].OwnerId, "Planet should be owned by player 1");
            
        }

        [TestMethod]
        public void Test3ForcesColonizingPlanetP1HasMore()
        {
            
            // P1 moves fleets
            var p1MoveResult = game.MoveFleet(new Shared.MoveRequest()
            {
                AuthToken = p1Logon.AuthToken,
                GameId = p1Logon.GameId,
                NumberOfShips = 40,
                DestinationPlanetId = 1,
                SourcePlanetId = 0
            });

            var p2MoveResult = game.MoveFleet(new Shared.MoveRequest()
            {
                AuthToken = p2Logon.AuthToken,
                GameId = p2Logon.GameId,
                NumberOfShips = 39,
                DestinationPlanetId = 1,
                SourcePlanetId = 2
            });



            // Move result should be valid
            Assert.IsTrue(p1MoveResult.Success, "Valid move result should succeed");
            Assert.IsTrue(p2MoveResult.Success, "Valid move result should succeed");
            Assert.AreEqual(1, p1MoveResult.Fleet.NumberOfTurnsToDestination, "Planet is 1 move away");
            Assert.AreEqual(1, p2MoveResult.Fleet.NumberOfTurnsToDestination, "Planet is 1 move away");
            Assert.AreEqual(0, planets[0].NumberOfShips, "Move should have removed the ships, planet has a growth rate of 0");
            Assert.AreEqual(1, planets[2].NumberOfShips, "Move should have removed the ships, planet has a growth rate of 0");

            // Tick engine forward to processing
            game.Update(startTime.AddMilliseconds(Game.START_DELAY + Game.PLAYER_TURN_LENGTH));

            // Check planets update the way they should
            Assert.AreEqual(0, game.GetFleets().Count, "Fleet should have arrived and been removed");
            Assert.AreEqual(1, planets[1].NumberOfShips, "Planet should have 1 ship on it after colonization");
            Assert.AreEqual(p1Logon.Id, planets[1].OwnerId, "Planet should be owned by player 1");
        }

        [TestMethod]
        public void TestOrderDoesntMatterColonizingPlanet()
        {

        }

        [TestMethod]
        public void TestFleetArrivesWhenItShould()
        {

        }

        [TestMethod]
        public void TestOriginalPlayerOwnsPlanetAfterEqualCombat()
        {

        }

        [TestMethod]
        public void CheckPlayerWinsWhenAllOtherPlanetsTaken()
        {

        }

        [TestMethod]
        public void CheckDrawWhenBothPlayersWithoutShips()
        {

        }
    }
}
