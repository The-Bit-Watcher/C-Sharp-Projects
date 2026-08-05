import { Component, ChangeDetectorRef  } from '@angular/core';
import { ActivatedRoute, RouterModule, Router } from '@angular/router';
import { OnInit } from '@angular/core';
import { MatCardModule} from '@angular/material/card';
import {MatIconModule} from '@angular/material/icon';
import { EventDataService } from '../../services/event-data.service';
import { Event } from '../../events-model';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-edit-event',
  imports: [RouterModule, MatCardModule,MatIconModule, CommonModule,
    MatFormFieldModule, MatInputModule, MatButtonModule, FormsModule],
  templateUrl: './edit-event.html',
  styleUrl: './edit-event.css',
})
export class EditEvent implements OnInit{
    currentEvent!: Event;

    constructor(private eventserv: EventDataService, private route:ActivatedRoute, private router:Router,
      private cdr: ChangeDetectorRef){}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    console.log('ID from route:', id);

    this.eventserv.getEvent(Number(id)).subscribe((data: any) => {
      console.log('Raw data from API:', data);
      
      this.currentEvent = {
        Id: data.id,             
        EventName: data.eventName, 
        Location: data.location,   
        TicketPrice: data.ticketPrice 
      }; 
      this.cdr.detectChanges();
      console.log('Mapped editEvent:', this.currentEvent);
    });
  }

  saveEvent() : void {
    if (this.currentEvent && this.currentEvent.Id){       
      this.eventserv.updateEvent(this.currentEvent.Id, this.currentEvent).subscribe({
        next: () => {
          console.log("Event updated successfully");
          this.router.navigate(['/event-listing']);
        },
        error: (error) => {
          console.error("Error updating Event", error);
        }
      });
    }
  }

  cancel(): void {
    this.router.navigate(['/event-listing']);
  }
}