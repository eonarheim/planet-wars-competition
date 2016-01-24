class Fleet extends ex.Actor {
   
   private _dest: Planet;
   private _turns: number;
   private _fleetLabel: ex.Label;
   private _ships: number;

   constructor(sp: Planet, dp: Planet, color: ex.Color, ships: number) {
      super(sp.x, sp.y, Config.FleetWidth, Config.FleetHeight, color);
      this._ships = ships;
      this._dest = dp;
      var spsc = sp.getServerCoord();
      var dpsc = dp.getServerCoord();
      this._turns = Math.ceil(Math.sqrt(
         Math.pow(dpsc.x - spsc.x, 2) +
         Math.pow(dpsc.y - spsc.y, 2)));
   }

   static create(fleet: Server.Fleet) {
      var sp = GameSession.getPlanet(fleet.sourcePlanetId);
      var dp = GameSession.getPlanet(fleet.destinationPlanetId);
      var co = GameSession.getOwnerColor(fleet.ownerId);
      var ships = fleet.numberOfShips;
      return new Fleet(sp, dp, co, ships);
   }

   onInitialize(engine) {
      super.onInitialize(engine);

      this._fleetLabel = new ex.Label(`${this._ships}`, 0, 10, 'Arial');
      this._fleetLabel.color = ex.Color.White;
      this._fleetLabel.textAlign = ex.TextAlign.Center;

      this.add(this._fleetLabel);

      this.moveBy(this._dest.x, this._dest.y, GameSession.getTurnDuration() * (this._turns-1)).asPromise().then(() => this.kill());
   }

   private _v1: ex.Vector = new ex.Vector(0, 0);
   private _v2: ex.Vector = new ex.Vector(0, 0);

   update(engine: ex.Engine, delta: number) {
      super.update(engine, delta);

      this._v1.x = this._dest.x;
      this._v1.y = this._dest.y;
      this._v2.x = this.x;
      this._v2.y = this.y;

      this.rotation = this._v1.subtract(this._v2).toAngle();
   }
}