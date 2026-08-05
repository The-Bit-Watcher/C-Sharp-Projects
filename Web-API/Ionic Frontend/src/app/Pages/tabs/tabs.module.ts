import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IonicModule } from '@ionic/angular';
import { RouterModule } from '@angular/router';
import { TabsPage } from './tabs.page';

@NgModule({
  imports: [
    CommonModule,
    IonicModule,
    RouterModule.forChild([
      {
        path: '',
        component: TabsPage,
        children: [
          {
            path: 'search',
            loadChildren: () => import('../search/search.module').then(m => m.SearchPageModule)
          },
          {
            path: 'watchlist',
            loadChildren: () => import('../watchlist/watchlist.module').then(m => m.WatchlistPageModule)
          },
          {
            path: 'watched',
            loadChildren: () => import('../watched/watched.module').then(m => m.WatchedPageModule)
          },
          {
            path: '',
            redirectTo: 'search',
            pathMatch: 'full'
          }
        ]
      }
    ])
  ],
  declarations: [TabsPage]
})
export class TabsPageModule {}