class Fleet extends ex.Actor {
   
   private _dest: Planet;
   private _turns: number;

   constructor(fleet: Server.Fleet) {

      var sp = GameSession.getPlanet(fleet.sourcePlanetId);
      var dp = GameSession.getPlanet(fleet.destinationPlanetId);
      var co = GameSession.getOwnerColor(fleet.Owner);

      super(sp.x, sp.y, Config.FleetWidth, Config.FleetHeight, co);

      this._dest = dp;
      this._turns = Math.ceil(Math.sqrt(
         Math.pow(dp.x - sp.x, 2) +
         Math.pow(dp.y - sp.y, 2)));
   }

   onInitialize() {
      this.moveBy(this._dest.x, this._dest.y, GameSession.getTurnDuration() * this._turns);
   }
}