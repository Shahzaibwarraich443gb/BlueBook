import { Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';

import {
  ApexAxisChartSeries,
  ApexChart,
  ApexDataLabels,
  ApexPlotOptions,
  ApexYAxis,
  ApexLegend,
  ApexStroke,
  ApexXAxis,
  ApexFill,
  ApexTooltip,
  ApexTitleSubtitle,
  ChartComponent
} from "ng-apexcharts";

export type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  dataLabels: ApexDataLabels;
  plotOptions: ApexPlotOptions;
  yaxis: ApexYAxis;
  xaxis: ApexXAxis;
  fill: ApexFill;
  tooltip: ApexTooltip;
  stroke: ApexStroke;
  legend: ApexLegend;
  title: ApexTitleSubtitle;
  colors: string[];
};

@Component({
  selector: 'app-barchart',
  templateUrl: './barchart.component.html',
  styleUrls: ['./barchart.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class BarchartComponent implements OnInit {

  @ViewChild("chartBar") chart: ChartComponent | undefined;
  public chartOptions: Partial<ChartOptions> | any;

  constructor() {
    this.chartOptions = {
      series: [
        {
          name: "Expense",
          data: [20, 45, 85, 100, 120, 160]
        },
        {
          name: "Income",
          data: [56, 80, 95, 125, 150, 190]
        }
      ],
      chart: {
        type: "bar",
        height: 500,
        width: '100%',
      },
      title: {
        text: "Expense vs Income",
        align: "left",
      },
      dataLabels: {
        enabled: false
      },
      colors: ["#E8E4F5", "#6F4EF6"],
      stroke: {
        show: true,
        width: 2,
        colors: ["transparent"]
      },
      xaxis: {
        categories: [
          "2016",
          "2017",
          "2018",
          "2019",
          "2020",
          "2021",
        ]
      },
      yaxis: {
        min: 10,
        max: 200,
        title: {
          text: "Income Report"
        }
      },
      legend: {
        position: "top",
        horizontalAlign: "right",
        floating: true,
        offsetY: -25,
        offsetX: -5
      },
      fill: {
        opacity: 1
      },
      tooltip: {
        y: {
          formatter: function(val: string) {
            return "$ " + val + " thousands";
          }
        }
      }
    };
  }

  ngOnInit(): void {
  }

}
