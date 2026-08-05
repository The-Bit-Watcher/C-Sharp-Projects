import { Component, ChangeDetectorRef  } from '@angular/core';
import { RouterModule, Router } from '@angular/router';
import { OnInit } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { CommonModule } from '@angular/common';
import { EventDataService } from '../../services/event-data.service';
import { Event } from '../../events-model';


@Component({
  selector: 'app-event-listing',
  imports: [RouterModule, MatCardModule, CommonModule],
  templateUrl: './event-listing.html',
  styleUrl: './event-listing.css',
})
export class EventListing implements OnInit {
  events: Event[] = [];

  constructor(private eventserv: EventDataService, private router:Router,
    private cdr: ChangeDetectorRef
  ){}

  ngOnInit(): void {
  this.loadEvents();
  }

loadEvents(): void {
  this.eventserv.getEvents().subscribe({
    next: (data) => {
      this.events = data.map((item: any) => ({
        Id: item.id,
        EventName: item.eventName,
        Location: item.location,
        TicketPrice: item.ticketPrice,
      })).sort((a, b) => b.Id - a.Id);
      this.cdr.detectChanges()
    },
    error: (error) => {
      console.error('Error fetching Events', error);
    }
  });
  }

  deleteEvent(eventId: number, eventName: string): void {
    if (confirm(`Are you sure you want to delete ${eventName}?`)) {
      this.eventserv.deleteEvent(eventId).subscribe({
        next: () => {
          console.log('Event deleted successfully');
          this.events = this.events.filter(event => event.Id !== eventId);
          this.cdr.detectChanges();
          this.router.navigate(['/event-listing', eventId]);
        },
        error: (error) => {
          console.error('Error deleting event', error);
          alert('Failed to delete event. Please try again.');
        }
      });
    }
  }

  navigateToEdit(eventId: number): void {
    console.log('Editing event with ID:', eventId);
    if (eventId) {
      this.router.navigate(['/edit-event', eventId]);
    } else {
      console.error('Invalid event ID');
    }
  }
}