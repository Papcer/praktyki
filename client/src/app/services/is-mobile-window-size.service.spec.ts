import { TestBed } from '@angular/core/testing';

import { IsMobileWindowSizeService } from './is-mobile-window-size.service';

describe('IsMobileWindowSizeService', () => {
  let service: IsMobileWindowSizeService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(IsMobileWindowSizeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
