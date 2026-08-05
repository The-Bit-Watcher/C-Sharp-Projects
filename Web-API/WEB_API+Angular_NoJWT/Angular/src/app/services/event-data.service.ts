import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Event } from '../events-model';

@Injectable({
    providedIn: 'root',
})


export class EventDataService{

        private apiUrl:string = "http://localhost:5205/api/Event";//url of web API
        
        constructor(private http:HttpClient){};

        getEvents():Observable<Event[]>
        {
            return this.http.get<Event[]>(`${this.apiUrl}`);
        }

        getEvent(Id:number):Observable<Event>{
            return this.http.get<Event>(`${this.apiUrl}/${Id}`);
        }

        deleteEvent(Id:number): Observable<any>{
            return this.http.delete(`${this.apiUrl}/${Id}`);
        }

        addEvent(event: Event): Observable<Event> {
            const payload = {
                eventName: event.EventName,
                location: event.Location,
                ticketPrice: event.TicketPrice
            };
            return this.http.post<Event>(`${this.apiUrl}`, payload);
        }

        updateEvent(Id: number, updatedEvent: Event): Observable<Event> {
            const payload = {
                id: updatedEvent.Id,
                eventName: updatedEvent.EventName,
                location: updatedEvent.Location,
                ticketPrice: updatedEvent.TicketPrice
            };
            return this.http.put<Event>(`${this.apiUrl}/${Id}`, payload);
        }      
};
