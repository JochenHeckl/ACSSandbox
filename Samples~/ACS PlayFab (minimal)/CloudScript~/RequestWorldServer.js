"use strict";
handlers.requestWorldServer = function (sessionId, context) {
    var request = {
        "AliasId": AliasId,
        "SessionId": sessionId,
        "SessionCookie": "",
        "InitialPlayers": [],
        "PreferredRegions": [
            "NorthEurope"
        ]
    };
    return multiplayer.RequestMultiplayerServer(request);
};
