class Planet extends ex.Actor {
   private _planet: Models.IPlanet;

   constructor(planet: Models.IPlanet) {
      var [x, y] = GameSession.mapServerCoordsToWorld(planet.x, planet.y);

      super(x, y, planet.size * 10, (this._radius = planet.size * 10));

      this.updateState(planet);
   }

   updateState(planet: Models.IPlanet) {
      this._planet = planet;
      // todo anything else
   }

   draw(ctx: CanvasRenderingContext2D, delta: number) {
      super.draw(ctx, delta);

      // draw an ellipse
      ctx.beginPath();
      ctx.arc(this.x, this.y, this._radius, 0, Math.PI * 2);
      ctx.fillStyle = ex.Color.Gray.toString();
      ctx.closePath();
      ctx.fill();
   }
}