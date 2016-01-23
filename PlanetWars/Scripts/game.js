var Assets = {
    TextureTest: null
};
var GameSession = (function () {
    function GameSession() {
    }
    GameSession.create = function (sessionId) {
        GameSession.SessionId = sessionId;
        var game = new ex.Engine({
            canvasElementId: "game",
            height: 480,
            width: 720
        });
        var loader = new ex.Loader();
        _.forIn(Assets, function (a) { return loader.addResource(a); });
        game.start(loader).then(GameSession.init);
        GameSession.Game = game;
    };
    GameSession.init = function () {
        GameSession.Game.on('update', function (e) {
        });
    };
    GameSession.updateSessionState = function () {
        $.getJSON("/api/games/" + GameSession.SessionId).then(function (sess) {
            GameSession.SessionState = sess;
        });
    };
    return GameSession;
})();
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var Planet = (function (_super) {
    __extends(Planet, _super);
    function Planet(planetId, x, y, size) {
        _super.call(this, x, y, size * 10, size * 10);
        this.planetId = planetId;
        this.size = size;
    }
    Planet.prototype.draw = function (ctx, delta) {
        _super.prototype.draw.call(this, ctx, delta);
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.size, 0, Math.PI * 2);
        ctx.fillStyle = ex.Color.Gray.toString();
        ctx.closePath();
        ctx.fill();
    };
    return Planet;
})(ex.Actor);
//# sourceMappingURL=game.js.map