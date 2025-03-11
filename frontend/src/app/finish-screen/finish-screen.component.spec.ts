import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FinishScreenComponent } from './finish-screen.component';

describe('FinishScreenComponent', () => {
  let component: FinishScreenComponent;
  let fixture: ComponentFixture<FinishScreenComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FinishScreenComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FinishScreenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
