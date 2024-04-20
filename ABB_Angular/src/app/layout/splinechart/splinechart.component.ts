import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { ViewChild } from "@angular/core";

import {
  ApexAxisChartSeries,
  ApexChart,
  ApexXAxis,
  ApexDataLabels,
  ApexTooltip,
  ApexStroke,
  ApexLegend,
  ApexYAxis,
  ApexTitleSubtitle,
  ChartComponent
} from "ng-apexcharts";

export type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  stroke: ApexStroke;
  tooltip: ApexTooltip;
  dataLabels: ApexDataLabels;
  yaxis: ApexYAxis;
  legend: ApexLegend;
  title: ApexTitleSubtitle;
  colors: string[];

};

@Component({
  selector: 'app-splinechart',
  templateUrl: './splinechart.component.html',
  styleUrls: ['./splinechart.component.scss'],
  encapsulation: ViewEncapsulation.None
})

export class SplinechartComponent implements OnInit {

  @ViewChild("chart") chart: ChartComponent | undefined;
  public chartOptions: Partial<ChartOptions> | any;
  

  constructor() {
    this.chartOptions = {
      series: [
        {
          name: "Expense",
          data: [15, 65, 85, 45, 100, 75, 100, 55, 120, 140, 100, 165]
        },
        {
          name: "Income",
          data: [35, 95, 45, 32, 85, 120, 41, 140, 160, 100, 180, 195]
        }
      ],
      chart: {
        height: 445,
        width: '100%',
        type: "area",
      },
      dataLabels: {
        enabled: true
      },
      title: {
        text: "Cashflow",
        align: "left",
      },
      colors: ["#E95582", "#6F4EF6"],
      stroke: {
        curve: "smooth"
      },
      xaxis: {
        categories: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
      },
      yaxis: {
        min: 10,
        max: 200
      },
      legend: {
        position: "top",
        horizontalAlign: "right",
        floating: true,
        offsetY: -25,
        offsetX: -5
      }
    };
  }
  public generateData(baseval: number, count: number, yrange: { max: number; min: number; }) {
    var i = 0;
    var series = [];
    while (i < count) {
      var x = Math.floor(Math.random() * (750 - 1 + 1)) + 1;
      var y =
        Math.floor(Math.random() * (yrange.max - yrange.min + 1)) + yrange.min;
      var z = Math.floor(Math.random() * (75 - 15 + 1)) + 15;

      series.push([x, y, z]);
      baseval += 86400000;
      i++;
    }
    return series;
  }
  ngOnInit(): void {}

}
