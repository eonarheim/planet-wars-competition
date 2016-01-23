

module Server {
    
    export interface Fleet {
        
        id: number;
        owner: number;
        numberOfShips: number;
        sourcePlanetId: number;
        destinationPlanetId: number;
    }
}