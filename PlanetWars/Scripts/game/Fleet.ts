class Fleet extends ex.Actor {
   
   private _dest: Planet;
   private _turns: number;   

   constructor(dp: Planet, sp: Planet, color: ex.Color) {
      super(sp.x, sp.y, Config.FleetWidth, Config.FleetHeight, color);
      
      this._dest = dp;
      this._turns = Math.ceil(Math.sqrt(
         Math.pow(dp.x - sp.x, 2) +
         Math.pow(dp.y - sp.y, 2)));
   }

   static create(fleet: Server.Fleet) {
      var sp = GameSession.getPlanet(fleet.sourcePlanetId);
      var dp = GameSession.getPlanet(fleet.destinationPlanetId);
      var co = GameSession.getOwnerColor(fleet.owner);

      return new Fleet(sp, dp, co);
   }

   onInitialize() {
      this.moveBy(this._dest.x, this._dest.y, GameSession.getTurnDuration() * this._turns).asPromise().then(() => this.kill());
   }

   private _v1: ex.Vector = new ex.Vector(0, 0);
   private _v2: ex.Vector = new ex.Vector(0, 0);

   update(engine: ex.Engine, delta: number) {

      this._v1.x = this._dest.x;
      this._v1.y = this._dest.y;
      this._v2.x = this.x;
      this._v2.y = this.y;

      this.rotation = this._v1.subtract(this._v2).toAngle();
   }
}