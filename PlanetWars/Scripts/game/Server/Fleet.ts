

module Server {
    
    export interface Fleet {
        
        id: number;
        ownerId: number;
        numberOfShips: number;
        numberOfTurnsToDestination: number;
        sourcePlanetId: number;
        destinationPlanetId: number;
    }
}