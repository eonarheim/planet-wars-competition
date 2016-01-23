var Assets = {
    TextureTest: null
};
var Config = {
    MapPadding: 50,
    MapSize: 300,
    FleetWidth: 6,
    FleetHeight: 7,
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
            _this._turnTimer = new ex.Timer(function () { return _this.updateSessionState(); }, _this.getTurnDuration(), true);
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
        var _this = this;
        return $.post("/api/status", { gameId: this.Id }).then(function (s) {
            GameSession.State = s;
            _.each(GameSession.State.planets, function (p) {
                var planet = new Planet(p);
                if (!_this._planets[p.id]) {
                    _this.Game.add(planet);
                }
                _this._planets[p.id] = planet;
            });
            _.each(GameSession.State.fleets, function (f) {
                var fleet = Fleet.create(f);
                if (!_this._fleets[f.id]) {
                    _this.Game.add(fleet);
                }
                _this._fleets[f.id] = fleet;
            });
        });
    };
    GameSession.getPlanet = function (planetId) {
        if (!this._planets[planetId]) {
            throw "Planet does not exist";
        }
        return this._planets[planetId];
    };
    GameSession.getOwnerColor = function (ownerId) {
        return this.State.playerA === ownerId ? Config.PlayerAColor : Config.PlayerBColor;
    };
    GameSession.getTurnDuration = function () {
        return this.State.playerTurnLength + this.State.serverTurnLength;
    };
    GameSession._planets = [];
    GameSession._fleets = [];
    return GameSession;
})();
var Fleet = (function (_super) {
    __extends(Fleet, _super);
    function Fleet(dp, sp, color) {
        _super.call(this, sp.x, sp.y, Config.FleetWidth, Config.FleetHeight, color);
        this._v1 = new ex.Vector(0, 0);
        this._v2 = new ex.Vector(0, 0);
        this._dest = dp;
        this._turns = Math.ceil(Math.sqrt(Math.pow(dp.x - sp.x, 2) +
            Math.pow(dp.y - sp.y, 2)));
    }
    Fleet.create = function (fleet) {
        var sp = GameSession.getPlanet(fleet.sourcePlanetId);
        var dp = GameSession.getPlanet(fleet.destinationPlanetId);
        var co = GameSession.getOwnerColor(fleet.owner);
        return new Fleet(sp, dp, co);
    };
    Fleet.prototype.onInitialize = function () {
        var _this = this;
        this.moveBy(this._dest.x, this._dest.y, GameSession.getTurnDuration() * this._turns).asPromise().then(function () { return _this.kill(); });
    };
    Fleet.prototype.update = function (engine, delta) {
        this._v1.x = this._dest.x;
        this._v1.y = this._dest.y;
        this._v2.x = this.x;
        this._v2.y = this.y;
        this.rotation = this._v1.subtract(this._v2).toAngle();
    };
    return Fleet;
})(ex.Actor);
//# sourceMappingURL=game.js.map