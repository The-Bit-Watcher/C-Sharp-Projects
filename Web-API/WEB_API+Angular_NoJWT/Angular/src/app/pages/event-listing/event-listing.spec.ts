import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EventListing } from './event-listing';

describe('EventListing', () => {
  let component: EventListing;
  let fixture: ComponentFixture<EventListing>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EventListing],
    }).compileComponents();

    fixture = TestBed.createComponent(EventListing);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
