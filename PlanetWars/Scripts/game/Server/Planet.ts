

module Server {
    
    export interface Planet {
        
        id: number;
        numberOfShips: number;
        size: number;
        position: Point;
        ownerId: number;
    }
}