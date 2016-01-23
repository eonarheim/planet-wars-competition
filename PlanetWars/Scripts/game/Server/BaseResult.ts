

module Server {
    
    export interface BaseResult<T> {
        
        success: boolean;
        message: string;
        errors: string[];
    }
}