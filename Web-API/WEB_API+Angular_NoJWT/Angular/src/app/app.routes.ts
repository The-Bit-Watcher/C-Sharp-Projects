import { Routes } from '@angular/router';
import { EditEvent } from './pages/edit-event/edit-event';
import { AddEvent } from './pages/add-event/add-event';
import { EventListing } from './pages/event-listing/event-listing'

export const routes: Routes = [    
    {path: '', component: EventListing},
    {path: 'event-listing', component: EventListing},
    {path:'edit-event/:id', component: EditEvent},
    {path:'add-event', component: AddEvent},         
];