import { IonicModule } from '@ionic/angular';
import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChartsModule } from '@progress/kendo-angular-charts';

import { LivePreviewPage } from './live-preview.page';

import { AccelerationAxisMonitorChartComponent } from 'src/app/components/axis-monitor-chart/acceleration-axis-monitor-chart.component';
import { SettingsModalComponent } from 'src/app/components/settings-modal/settings-modal.component';

@NgModule({
  imports: [
    IonicModule,
    CommonModule,
    FormsModule,
    ChartsModule,
    RouterModule.forChild([{ path: '', component: LivePreviewPage }])
  ],
  declarations: [LivePreviewPage, AccelerationAxisMonitorChartComponent, SettingsModalComponent],
  entryComponents: [
    SettingsModalComponent
  ]
})
export class LivePreviewModule { }
