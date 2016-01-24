

module Server {
    
    export interface Fleet {
        
        id: number;
        ownerId: number;
        numberOfShips: number;
        sourcePlanetId: number;
        destinationPlanetId: number;
    }
}