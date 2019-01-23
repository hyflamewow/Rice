import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TotemComponent } from './totem.component';
import { ValuesComponent } from './values/values.component';

const routes = [
  {
    path: '', component: TotemComponent, children: [
      { path: '', component: ValuesComponent },
      { path: 'values', component: ValuesComponent },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TotemRoutingModule { }
