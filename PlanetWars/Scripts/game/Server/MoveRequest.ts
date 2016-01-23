

module Server {
    
    export interface MoveRequest {
        
        authonToken: string;
        destinationPlanetId: number;
        gameId: number;
        numberOfShips: number;
        sourcePlanetId: number;
    }
}