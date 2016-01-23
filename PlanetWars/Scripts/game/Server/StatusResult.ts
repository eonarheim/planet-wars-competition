

module Server {
    
    export interface StatusResult {
        
        isGameOver: boolean;
        status: boolean;
        planets: Planet[];
        fleets: Fleet[];
    }
}