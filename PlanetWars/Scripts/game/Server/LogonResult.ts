

module Server {
    
    export interface LogonResult {
        
        authToken: string;
        id: number;
        gameId: number;
        gameStart: Date;
    }
}