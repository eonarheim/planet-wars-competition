

module Server {
    
    export interface Planet {
        
        id: number;
        numberOfShips: number;
        growthRate: number;
        size: number;
        position: Point;
        ownerId: number;
    }
}