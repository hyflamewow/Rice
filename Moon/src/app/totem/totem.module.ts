import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';

import { TotemRoutingModule } from './totem-routing.module';
import { TotemComponent } from './totem.component';
import { ValuesComponent } from './values/values.component';

@NgModule({
  declarations: [TotemComponent, ValuesComponent],
  imports: [
    CommonModule,
    TotemRoutingModule,
    HttpClientModule
  ]
})
export class TotemModule { }
