/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { RefreshTokenInrceptorService } from './RefreshTokenInrceptor.service';

describe('Service: RefreshTokenInrceptor', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [RefreshTokenInrceptorService]
    });
  });

  it('should ...', inject([RefreshTokenInrceptorService], (service: RefreshTokenInrceptorService) => {
    expect(service).toBeTruthy();
  }));
});
