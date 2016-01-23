

module Server {
    
    export interface MoveRequest {
        
        authToken: string;
        destinationPlanetId: number;
        gameId: number;
        numberOfShips: number;
        sourcePlanetId: number;
    }
}