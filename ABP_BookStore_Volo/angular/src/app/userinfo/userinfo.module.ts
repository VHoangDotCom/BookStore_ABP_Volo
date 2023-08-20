import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';

import { UserinfoRoutingModule } from './userinfo-routing.module';
import { UserinfoComponent } from './userinfo.component';
import { NgbDatepickerModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [UserinfoComponent],
  imports: [SharedModule, UserinfoRoutingModule, NgbDatepickerModule],
})
export class UserinfoModule {}
