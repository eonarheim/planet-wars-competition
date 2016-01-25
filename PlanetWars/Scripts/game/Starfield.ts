interface Star {
   x: number;
   y: number;
   o: number;
}

class Starfield extends ex.Actor {
   
   private _stars: Star[] = [];
   private _fadeTimer: ex.Timer;
   private _meteorTimer: ex.Timer;

   constructor() {
      super(0, 0, 0, 0);
   }

   onInitialize() {
      this.setWidth(GameSession.Game.getWidth());
      this.setHeight(GameSession.Game.getHeight());

      // generate stars
      for (var i = 0; i < Config.StarfieldSize; i++) {
         this._stars.push({
            x: ex.Util.randomIntInRange(0, this.getWidth()),
            y: ex.Util.randomIntInRange(0, this.getHeight()),
            o: ex.Util.randomInRange(Config.StarfieldMinFade, Config.StarfieldMaxFade)
         });
      }

      this._fadeTimer = new ex.Timer(() => this._updateFaded(), Config.StarfieldRefreshRate, true);
      this._meteorTimer = new ex.Timer(() => this._shootMeteor(), ex.Util.randomIntInRange(Config.StarfieldMeteorFreqMin, Config.StarfieldMeteorFreqMax), true);
      GameSession.Game.add(this._fadeTimer);
      GameSession.Game.add(this._meteorTimer);
      this._updateFaded();
   }

   private _updateFaded() {
      
      // randomly choose % stars to fade
      var totalFaded = Math.floor(this._stars.length *
         ex.Util.randomInRange(Config.StarfieldMinFadeRefreshAmount, Config.StarfieldMaxFadeRefreshAmount));

      for (var i = 0; i < totalFaded; i++) {

         // can overwrite
         this._stars[ex.Util.randomIntInRange(0, this._stars.length - 1)].o = ex.Util.randomInRange(Config.StarfieldMinFade, Config.StarfieldMaxFade);
      }
   }

   private _shootMeteor() {
      
      var dest = new ex.Vector(
         ex.Util.randomInRange(0, this.getWidth()),
         ex.Util.randomIntInRange(50, this.getHeight() / 2));
      var meteor = new ex.Actor(ex.Util.randomIntInRange(0, this.getWidth()), 0, 2, 2, ex.Color.fromRGB(164, 237, 255, 1));
      
      meteor.moveBy(dest.x, dest.y, Config.StarfieldMeteorSpeed).asPromise().then(() => meteor.kill());

      GameSession.Game.add(meteor);

      // schedule next metor
      this._meteorTimer.interval = ex.Util.randomIntInRange(Config.StarfieldMeteorFreqMin, Config.StarfieldMeteorFreqMax);
   }

   draw(ctx: CanvasRenderingContext2D, delta: number) {

      for (var i = 0; i < this._stars.length; i++) {
         ctx.fillStyle = ex.Color.fromRGB(255, 255, 255, this._stars[i].o);
         ctx.fillRect(this._stars[i].x, this._stars[i].y, 1, 1);
      }
      
   }
}