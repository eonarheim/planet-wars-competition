var Assets = {
    TextureTest: null
};
var Config = {
    MapPadding: 50,
    MapSize: 300,
    PlanetMinSize: 15,
    PlanetMaxSize: 50,
    PlanetNeutralColor: ex.Color.Gray,
    PlayerAColor: ex.Color.Red,
    PlayerBColor: ex.Color.Blue
};
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var Planet = (function (_super) {
    __extends(Planet, _super);
    function Planet(planet) {
        var p = GameSession.mapServerCoordsToWorld(planet.position);
        var s = GameSession.mapPlanetSize(planet.size);
        _super.call(this, p.x, p.y, s, s);
        this._planetColor = Config.PlanetNeutralColor;
        this._initialShips = planet.numberOfShips;
        this.updateState(planet);
    }
    Planet.prototype.onInitialize = function (engine) {
        _super.prototype.onInitialize.call(this, engine);
        this._shipLabel = new ex.Label(null, 0, 0, 'Arial');
        this._shipLabel.color = ex.Color.White;
        this._shipLabel.textAlign = ex.TextAlign.Center;
        this.add(this._shipLabel);
    };
    Planet.prototype.updateState = function (planet) {
        this._planet = planet;
    };
    Planet.prototype.update = function (engine, delta) {
        _super.prototype.update.call(this, engine, delta);
        this._shipLabel.text = "Ships: " + this._planet.numberOfShips.toString();
        if (this._planet.ownerId === GameSession.State.playerA) {
            this._planetColor = Config.PlayerAColor;
        }
        else if (this._planet.ownerId === GameSession.State.playerB) {
            this._planetColor = Config.PlayerBColor;
        }
        else {
            this._planetColor = Config.PlanetNeutralColor;
        }
    };
    Planet.prototype.draw = function (ctx, delta) {
        _super.prototype.draw.call(this, ctx, delta);
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.getWidth(), 0, Math.PI * 2);
        ctx.fillStyle = this._planetColor.toString();
        ctx.closePath();
        ctx.fill();
    };
    return Planet;
})(ex.Actor);
var GameSession = (function () {
    function GameSession() {
    }
    GameSession.create = function (gameId) {
        GameSession.Id = gameId;
        var game = new ex.Engine({
            canvasElementId: "game",
            height: 480,
            width: 720
        });
        game.backgroundColor = ex.Color.Black;
        var loader = new ex.Loader();
        _.forIn(Assets, function (a) { return loader.addResource(a); });
        game.start(loader).then(GameSession.init);
        GameSession.Game = game;
    };
    GameSession.init = function () {
        var _this = this;
        this.updateSessionState().then(function () {
            _.each(GameSession.State.planets, function (p) {
                var planet = new Planet(p);
                _this._planets[p.id] = planet;
                _this.Game.add(planet);
            });
        });
    };
    GameSession.mapPlanetSize = function (s) {
        return ((Config.PlanetMaxSize - Config.PlanetMinSize) / Config.PlanetMaxSize) * s;
    };
    GameSession.mapServerCoordsToWorld = function (p) {
        var px = _.map(GameSession.State.planets, function (k) { return k.position.x; });
        var py = _.map(GameSession.State.planets, function (k) { return k.position.y; });
        var pxMin = _.min(px);
        var pxMax = _.max(px);
        var pyMin = _.min(py);
        var pyMax = _.max(py);
        var sfx = (pxMax - pxMin) / pxMax;
        var sfy = (pyMax - pyMin) / pyMax;
        var x = p.x * (sfx * Config.MapSize);
        var y = p.y * (sfy * Config.MapSize);
        x += Config.MapPadding;
        y += Config.MapPadding;
        return new ex.Point(x, y);
    };
    GameSession.updateSessionState = function () {
        return $.post("/api/status", { gameId: this.Id }).then(function (s) {
            GameSession.State = s;
        });
    };
    GameSession._planets = [];
    return GameSession;
})();
//# sourceMappingURL=game.js.map