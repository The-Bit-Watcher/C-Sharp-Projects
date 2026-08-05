import { Component } from '@angular/core';
import { RouterModule, Router } from '@angular/router';
import { MatCardModule} from '@angular/material/card';
import {MatIconModule} from '@angular/material/icon';
import { EventDataService } from '../../services/event-data.service';
import { Event } from '../../events-model';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { FormsModule } from '@angular/forms';
import { EventData } from '@angular/cdk/testing';

@Component({
  selector: 'app-add-event',
  imports: [RouterModule, MatCardModule,MatIconModule, CommonModule,
    MatFormFieldModule, MatInputModule, MatButtonModule, FormsModule],
  templateUrl: './add-event.html',
  styleUrl: './add-event.css',
})
export class AddEvent {

    addEv:Event = {
        Id: 0,
        EventName: '',
        TicketPrice: 0,
        Location: '',     
    };

    constructor(private eventserv:EventDataService, private router:Router){}

    addEvent(): void {
    if (!this.addEv.EventName?.trim() || 
        !this.addEv.Location?.trim() || 
        !this.addEv.TicketPrice || 
        this.addEv.TicketPrice <= 0) {
      console.error("Please fill in all required fields");
      return;
    }

    this.eventserv.addEvent(this.addEv).subscribe({
      next: (response) => {
        console.log('Event added successfully:', response);
        this.router.navigate(['/event-listing']);
      },
      error: (error) => {
        console.error('Error adding event:', error);
        alert('Failed to add event. Please try again.');
      }
    });
  }

    cancel(): void {
    this.router.navigate(["./event-listing"]);
  }

  isFormValid(): boolean {
    return !!(this.addEv.EventName?.trim() && 
              this.addEv.Location?.trim() && 
              this.addEv.TicketPrice && 
              this.addEv.TicketPrice > 0);
  }
}
