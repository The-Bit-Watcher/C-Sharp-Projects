export interface Movie {
  id?: number;
  imdbId: string;
  title: string;
  year: number;
  img: string;           
  poster?: string;       
  actors: string[];
  genre: string;
  plot: string;
  director: string;
  writer: string;
  rated: string;
  runtime: string;
  imdbRating: string;
  rank: number;        
  aka: string;          
  imbd_url: string;      
  imbd_iv?: string;
  width?: number;
  height?: number;
}

export interface WatchlistItem {
  id: number;
  userId: number;
  movieId: number;
  dateAdded: string;
  movie: Movie;
}

export interface WatchedItem {
  id: number;
  userId: number;
  movieId: number;
  timesWatched: number;
  firstWatchedAt: string;
  lastWatchedAt: string;
  movie: Movie;
}

export interface GenreStat {
  genre: string;
  count: number;
}

export interface YearlyStat {
  year: number;
  count: number;
}

export interface SummaryStats {
  totalWatched: number;
  uniqueMovies: number;
  mostWatchedMovie: string;
  mostWatchedCount: number;
  averageImdbRating: number;
}