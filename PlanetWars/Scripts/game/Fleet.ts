class Fleet extends ex.Actor {
   
   private _dest: Planet;
   private _turns: number;
   private _fleetLabel: ex.Label;
   private _ships: number;
   private static _sheet: ex.SpriteSheet;

   constructor(sp: Planet, dp: Planet, anim: ex.Animation, ships: number) {
      super(sp.x, sp.y, Config.FleetWidth, Config.FleetHeight);

      this.addDrawing('default', anim);
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
      var ships = fleet.numberOfShips;

      if (!Fleet._sheet) {
         Fleet._sheet = new ex.SpriteSheet(Assets.TextureFleets, 4, 1, 12, 10);
      }

      var anim: ex.Animation;

      if (fleet.ownerId === GameSession.State.playerA) {
         anim = Fleet._sheet.getAnimationBetween(GameSession.Game, 0, 1, Config.FleetAnimSpeed);
      } else {
         anim = Fleet._sheet.getAnimationBetween(GameSession.Game, 2, 3, Config.FleetAnimSpeed);
      }
      
      return new Fleet(sp, dp, anim, ships);
   }

   private _v1: ex.Vector = new ex.Vector(0, 0);
   private _v2: ex.Vector = new ex.Vector(0, 0);

   onInitialize(engine) {
      super.onInitialize(engine);

      this._fleetLabel = new ex.Label(`${this._ships}`, 0, 10, 'Arial');
      this._fleetLabel.color = ex.Color.White;
      this._fleetLabel.textAlign = ex.TextAlign.Center;

      this.add(this._fleetLabel);

      this._v1.x = this._dest.x;
      this._v1.y = this._dest.y;
      this._v2.x = this.x;
      this._v2.y = this.y;

      this.rotation = this._v1.subtract(this._v2).toAngle();
      this._fleetLabel.rotation = -this.rotation;

      this.moveBy(this._dest.x, this._dest.y, GameSession.getTurnDuration() * this._turns).asPromise().then(() => this.kill());
   }
}