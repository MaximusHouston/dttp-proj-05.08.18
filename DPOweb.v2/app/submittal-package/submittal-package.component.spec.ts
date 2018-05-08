import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SubmittalPackageComponent } from './submittal-package.component';

describe('SubmittalPackageComponent', () => {
    let component: SubmittalPackageComponent;
    let fixture: ComponentFixture<SubmittalPackageComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [SubmittalPackageComponent]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(SubmittalPackageComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
