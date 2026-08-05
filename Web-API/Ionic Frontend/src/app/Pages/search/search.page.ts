import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IonicModule, ToastController } from '@ionic/angular';
import { Router } from '@angular/router';
import { Movie as MovieService } from 'src/services/movie';
import { Movie } from 'src/Models/model';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-search',
  templateUrl: './search.page.html',
  styleUrls: ['./search.page.scss'],
  standalone: false,
})
export class SearchPage {

  movies$: Observable<Movie[]> | undefined;
   searchItem:string = '';

  constructor(private router: Router, private toastController: ToastController, 
    private movieService: MovieService) { }

  ngOnInit(){
    this.searchItem = '';
  }


  async onSearch(item: string){
    this.searchItem = item;
    if (!this.searchItem){
      await this.showToast("Please fill in search for a movie", 'danger');
      return;
    }//if search is empty and press search

    this.movies$ = this.movieService.searcMovie(this.searchItem);

    return this.movies$;
  }

  goToDetail(movie: Movie){
    this.router.navigate(['/movie-detail'], {
      state: { movie: movie}
    });
  }

  async showToast(message: string, color: 'success' | 'danger' | 'warning'){
    const toast = await this.toastController.create({
      message,
      duration: 2500,
      color,
      position: "top"
    });
    await toast.present();
  }


}