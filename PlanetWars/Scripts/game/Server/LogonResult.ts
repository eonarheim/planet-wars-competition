

module Server {
    
    export interface LogonResult {
        
        authToken: string;
        gameId: number;
        gameStart: Date;
    }
}