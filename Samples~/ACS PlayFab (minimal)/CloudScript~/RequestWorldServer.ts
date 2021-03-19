handlers.requestWorldServer = function( sessionId?:any, context?: IPlayFabContext) : any {
    var request = {
        "AliasId": AliasId,
        "SessionId": sessionId,
        "SessionCookie": "",
        "InitialPlayers": [
        ],
        "PreferredRegions": [
            "NorthEurope"
        ]
    };

    return multiplayer.RequestMultiplayerServer(request);
}