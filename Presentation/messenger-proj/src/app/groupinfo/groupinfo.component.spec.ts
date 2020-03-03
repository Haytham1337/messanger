import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupinfoComponent } from './groupinfo.component';

describe('GroupinfoComponent', () => {
  let component: GroupinfoComponent;
  let fixture: ComponentFixture<GroupinfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ GroupinfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GroupinfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
