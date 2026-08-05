import { TestBed } from '@angular/core/testing';

import { Watched } from './watched';

describe('Watched', () => {
  let service: Watched;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Watched);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
