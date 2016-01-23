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
    };
    GameSession.updateSessionState = function () {
        $.getJSON("/api/games/" + GameSession.SessionId).then(function (sess) {
            GameSession.SessionState = sess;
        });
    };
    return GameSession;
})();
//# sourceMappingURL=game.js.map