import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DeleteCheckComponent } from './delete-check.component';

describe('DeleteCheckComponent', () => {
  let component: DeleteCheckComponent;
  let fixture: ComponentFixture<DeleteCheckComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DeleteCheckComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DeleteCheckComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
