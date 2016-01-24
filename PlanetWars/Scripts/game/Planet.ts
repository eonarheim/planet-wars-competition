class Planet extends ex.Actor {
   private _planet: Server.Planet;
   private _initialShips: number;
   private _planetColor: ex.Color;
   private _shipLabel: ex.Label;

   constructor(planet: Server.Planet) {
      var p = GameSession.mapServerCoordsToWorld(planet.position);
      var s = GameSession.mapPlanetSize(planet.size);

      super(p.x, p.y, s, s);

      this._planetColor = Config.PlanetNeutralColor;
      this._initialShips = planet.numberOfShips;

      this.updateState(planet);
   }

   getServerCoord() : ex.Point {
      return new ex.Point(this._planet.position.x, this._planet.position.y);
   }

   onInitialize(engine: ex.Engine) {
      super.onInitialize(engine);

      this._shipLabel = new ex.Label(null, 0, 0, 'Arial');
      this._shipLabel.color = ex.Color.White;
      this._shipLabel.textAlign = ex.TextAlign.Center;

      this.add(this._shipLabel);
   }

   updateState(planet: Server.Planet) {
      this._planet = planet;
   }

   update(engine: ex.Engine, delta: number) {
      super.update(engine, delta);

      this._shipLabel.text = `Ships: ${this._planet.numberOfShips.toString()}`;

      if (this._planet.ownerId === GameSession.State.playerA) {
         this._planetColor = Config.PlayerAColor;
      } else if (this._planet.ownerId === GameSession.State.playerB) {
         this._planetColor = Config.PlayerBColor;
      } else {
         this._planetColor = Config.PlanetNeutralColor;
      }
   }

   draw(ctx: CanvasRenderingContext2D, delta: number) {
      // draw an ellipse
      ctx.beginPath();
      ctx.arc(this.x, this.y, this.getWidth(), 0, Math.PI * 2);
      ctx.fillStyle = this._planetColor.toString();
      ctx.closePath();
      ctx.fill();

      super.draw(ctx, delta);
   }
}