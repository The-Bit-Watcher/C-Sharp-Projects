export interface User {
    email: string;
    password: string;
    username: string;
    age?: number;
};


export interface Movie {
    imbd_id: string;
    title: string;
    year: number;
    rank: number;
    actors: string[]; 
    aka: string;
    imbd_url: string;
    imbd_iv: string;
    img: string;
    width: number;
    height: number;
};

export interface MovieSearchResponse {
    ok: boolean;
    description: Movie[];   
};

export interface WatchlistItem {
    movie: Movie;
    dateAdded: string;
};

export interface WatchedItem {
    movie: Movie;
    timesWatched: number;
    dateWatched: string;
};