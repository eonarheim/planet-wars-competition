/// <reference path="../excalibur-0.6.0.d.ts"/>
/// <reference path="../typings/lodash/lodash.d.ts"/>
/// <reference path="Config.ts"/>
/// <reference path="Assets.ts"/>
/// <reference path="Planet.ts"/>

class GameSession {
   
   static Game: ex.Engine;
   static SessionId: number;
   static SessionState: Server.StatusResult;

   static create(sessionId: number) {
      GameSession.SessionId = sessionId;

      var game = new ex.Engine({
         canvasElementId: "game",
         height: 480,
         width: 720
      });

      // load assets
      var loader = new ex.Loader();      
      _.forIn(Assets, (a) => loader.addResource(a));

      game.start(loader).then(GameSession.init);

      GameSession.Game = game;
   }

   // Game Objects
   private static _planets: {[key: number]: Planet} = [];

   static init() {

      this.updateSessionState().then(() => {
         
         // add planets to game
         _.each(this.SessionState.planets, (p) => {
            var planet = new Planet(p);
            this._planets[p.id] = planet;
            this.Game.add(planet);
         });

      });

   }

   static mapPlanetSize(s: number) {
      return ((Config.PlanetMaxSize - Config.PlanetMinSize) / Config.PlanetMaxSize) * s;
   }

   static mapServerCoordsToWorld(p: Server.Point): ex.Point {
      // all planet pos
      var px = _.map(this.SessionState.planets, k => k.position.x);
      var py = _.map(this.SessionState.planets, k => k.position.y);

      // min/max ranges of planet pos
      var pxMin = _.min(px);
      var pxMax = _.max(px);
      var pyMin = _.min(py);
      var pyMax = _.max(py);

      // relative scale factors
      var sfx = (pxMax - pxMin) / pxMax;
      var sfy = (pyMax - pyMin) / pyMax;

      // position in grid world will be 
      var x = p.x * (sfx * Config.MapSize);
      var y = p.y * (sfy * Config.MapSize);

      // drawable space starts after padding
      x += Config.MapPadding;
      y += Config.MapPadding;
      
      return new ex.Point(x, y);
   }

   static updateSessionState(): JQueryPromise<Models.IGameSession> {

      return $.getJSON(`/api/games/${GameSession.SessionId}`).then(sess => {
         GameSession.SessionState = sess;
      });

   }
}