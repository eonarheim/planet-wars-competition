/// <reference path="../excalibur-0.6.0.d.ts"/>
/// <reference path="../typings/lodash/lodash.d.ts"/>
/// <reference path="Config.ts"/>
/// <reference path="Assets.ts"/>
/// <reference path="Planet.ts"/>
/// <reference path="Fleet.ts"/>

class GameSession {
   
   static Game: ex.Engine;
   static Id: number;
   static State: Server.StatusResult;

   static create(gameId: number) {
      GameSession.Id = gameId;

      var game = new ex.Engine({
         canvasElementId: "game",
         displayMode: ex.DisplayMode.Container
      });
      game.backgroundColor = ex.Color.Black;

      // load assets
      var loader = new ex.Loader();      
      _.forIn(Assets, (a: ex.ILoadable) => loader.addResource(a));

      game.start(loader).then(g => GameSession.init());

      GameSession.Game = game;
   }

   // Game Objects
   private static _planets: { [key: number]: Planet } = [];
   private static _fleets: { [key: number]: Fleet } = [];
   private static _turnTimer: ex.Timer;

   static init() {
      
      GameSession.Game.add(new Starfield());
      
      GameSession.updateSessionState().then(() => {

         GameSession._turnTimer = new ex.Timer(() => GameSession.updateSessionState(), GameSession.getTurnDuration(), true);
         GameSession.Game.add(GameSession._turnTimer);
         GameSession.Game.add(new Chart(GameSession.Game.getWidth() / 2, Config.ChartOffsetY, Config.ChartWidth, Config.ChartHeight, Config.ChartBackground))
      });

   }

   static mapPlanetSize(growthRate: number) {
      var sf = growthRate / _.max(_.map(GameSession.State.planets, p => p.growthRate));

      return ex.Util.clamp(sf * Config.PlanetMaxSize, Config.PlanetMinSize, Config.PlanetMaxSize);
   }

   static mapServerCoordsToWorld(p: Server.Point): ex.Point {
      // all planet pos
      var px = _.map(GameSession.State.planets, k => k.position.x);
      var py = _.map(GameSession.State.planets, k => k.position.y);

      // min/max ranges of planet pos
      var pxMin = _.min(px);
      var pxMax = _.max(px);
      var pyMin = _.min(py);
      var pyMax = _.max(py);

      // relative scale factors
      var sfx = p.x / pxMax;
      var sfy = p.y / pyMax;

      // position in grid world will be 
      var x = (sfx * Config.MapSize);
      var y = (sfy * Config.MapSize);
      
      // center map
      var vw = GameSession.Game.getWidth();
      var vh = GameSession.Game.getHeight();

      x = ((vw / 2) - (Config.MapSize / 2)) + x;
      y = ((vh / 2) - (Config.MapSize / 2)) + y;

      return new ex.Point(x, y);
   }

   static updateSessionState(): JQueryPromise<Server.StatusResult> {

      return $.post("/api/status", { gameId: GameSession.Id }).then(s => {
         GameSession.State = <Server.StatusResult>s;

         if (GameSession.State.currentTurn > 0) {
            $("#game-turns span").text(GameSession.State.currentTurn);
         }

         $("[data-id='1'] span").text(GameSession.State.playerAScore);
         $("[data-id='2'] span").text(GameSession.State.playerBScore);

         // add planets to game
         _.each(GameSession.State.planets, (p) => {
            var planet = new Planet(p);

            if (!GameSession._planets[p.id]) {
               GameSession.Game.add(planet);
               GameSession._planets[p.id] = planet;
            } else {
               GameSession._planets[p.id].updateState(p);
            }
         });

         if (GameSession.State.isGameOver) {

            $("#game-over").show();
            $("#game-over span").text(GameSession.State.status);

            return;
         }

         // add fleets
         _.each(GameSession.State.fleets, (f) => {
            var fleet = Fleet.create(f);
            if (!GameSession._fleets[f.id]) {
               GameSession.Game.add(fleet);
               GameSession._fleets[f.id] = fleet;
            }      
         });
      });

   }

   static getPlanet(planetId: number) {
      if (!GameSession._planets[planetId]) {
         throw "Planet does not exist";
      }

      return GameSession._planets[planetId];
   }

   static getOwnerColor(ownerId: number) {
      if (ownerId == GameSession.State.playerA) {
         return Config.PlayerAColor;
      }
      if (ownerId == GameSession.State.playerB) {
         return Config.PlayerBColor;
      }
      return Config.PlanetNeutralColor;
   }

   static getTurnDuration() {
      return GameSession.State.playerTurnLength; // + GameSession.State.serverTurnLength;
   }
}