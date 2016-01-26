class Chart extends ex.Actor {
   
   update(engine: ex.Engine, delta: number) {
      super.update(engine, delta);
   }

   draw(ctx: CanvasRenderingContext2D, delta: number) {
      super.draw(ctx, delta);
      
      this._drawLine(ctx, GameSession.State.playerAScoreOverTime, Config.PlayerAColor);
      this._drawLine(ctx, GameSession.State.playerBScoreOverTime, Config.PlayerBColor);
   }

   private _drawLine(ctx: CanvasRenderingContext2D, scores: number[], color: ex.Color) {
      var brush = new ex.Point(0, this.getHeight());
      var step = 0, stepWidth = this.getWidth() / Math.max(35, GameSession.State.currentTurn);
      var yMax = _.sum(_.map(GameSession.State.fleets, x => x.numberOfShips)) +
         _.sum(_.map(GameSession.State.planets, x => x.numberOfShips));

      ctx.beginPath();
      ctx.strokeStyle = color.toString();
      ctx.lineWidth = 2;

      // draw player 1
      for (step; step < scores.length; step++) {

         // set brush pos
         brush.x = step * stepWidth;
         brush.y = this.getHeight() - ((scores[step] / yMax) * this.getHeight());

         // workaround getLeft() not using anchor
         ctx.lineTo(this.getBounds().left + brush.x, this.getBounds().top + brush.y);
      }
      
      ctx.stroke();
   }
}