

module Server {
    
    export interface Fleet {
        
        id: number;
        numberOfShips: number;
        sourcePlanetId: number;
        destinationPlanetId: number;
    }
}