import { Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';

import {
  ApexNonAxisChartSeries,
  ApexResponsive,
  ApexChart,
  ApexFill,
  ApexDataLabels,
  ApexLegend,
  ApexTitleSubtitle,
  ChartComponent
} from "ng-apexcharts";

export type ChartOptions = {
  series: ApexNonAxisChartSeries;
  chart: ApexChart;
  responsive: ApexResponsive[];
  labels: any;
  fill: ApexFill;
  legend: ApexLegend;
  dataLabels: ApexDataLabels;
  title: ApexTitleSubtitle;
  colors: string[];
};

@Component({
  selector: 'app-piechart',
  templateUrl: './piechart.component.html',
  styleUrls: ['./piechart.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class PiechartComponent implements OnInit {

  @ViewChild("chartPie") chart: ChartComponent | undefined;
  public chartOptions: Partial<ChartOptions> | any;

  constructor() {
    this.chartOptions = {
      series: [45, 30, 25],
      chart: {
        width: 380,
        type: "donut"
      },
      labels: ["Income", "Expense", "Balance"],
      dataLabels: {
        enabled: false
      },
      fill: {
        type: "gradient"
      },
      title: {
        text: "Piechart Report",
        align: "left",
      },
      colors: ["#6F4EF6", "#5BC5A8", "#FF9720"],
      legend: {
        formatter: function(val: string, opts: { w: { globals: { series: { [x: string]: string; }; }; }; seriesIndex: string | number; }) {
          return val + " - " + opts.w.globals.series[opts.seriesIndex];
        }
      },
      responsive: [
        {
          breakpoint: 1600,
          options: {
            chart: {
              width: 315
            }
          }
        },
        {
          breakpoint: 1200,
          options: {
            chart: {
              width: 400
            }
          }
        },
        {
          breakpoint: 575,
          options: {
            chart: {
              width: 300,
              height: 250
            },
            legend: {
              position: "bottom"
            }
          }
        }
      ]
    };
  }

  ngOnInit(): void {}

}
