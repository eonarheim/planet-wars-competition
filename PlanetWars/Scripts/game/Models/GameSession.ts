
module Models {

    
    export interface IGameSession {
        
        gameId: number;
        players: { [key: number]: string; };
    }
}