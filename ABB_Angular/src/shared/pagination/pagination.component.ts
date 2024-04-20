import { Component, Output, EventEmitter, Input, OnInit, SimpleChanges, ViewEncapsulation, OnChanges, DoCheck, ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'app-pagination',
  templateUrl: './pagination.component.html',
  styleUrls: ['./pagination.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  encapsulation: ViewEncapsulation.None
})
export class PaginationComponent implements OnInit {

  constructor() { }

  @Output() notifyParent: EventEmitter<any> = new EventEmitter();

  private selectedCollectionSize: number;


  /** The total number of records */
  @Input()
  set collectionSize(value: number) {
    this.selectedCollectionSize = value;
    this.totalPages = new Array(Math.ceil(this.collectionSize / this.pageSize));
  }

  get collectionSize() {
    return this.selectedCollectionSize;
  }

  ngOnChanges(changes: SimpleChanges): void {
    console.log(changes)
  }


  /** Current page */
  @Input()
  currentPage = 1;

  /** The number of records to display */
  // @Input()
  pageSize = 10;

  /** The number of buttons to show either side of the current page */
  @Input()
  maxSize = 2;

  /** Display the First/Last buttons */
  @Input()
  firstLastButtons = false;

  /** Display the Next/Previous buttons */
  @Input()
  nextPreviousButtons = true;

  totalPages: any[] = [];


  ngOnInit(): void {
    abp.event.on('record-updated', (event) => {
      console.log('event', event);
      this.collectionSize = event;
      this.totalPages = new Array(Math.ceil(this.collectionSize / this.pageSize));
    })
  }

  // ngOnChanges(changes: SimpleChanges) {
  //   this.totalPages = new Array(Math.ceil(this.collectionSize / this.pageSize));
  // }

  updateRows(event) {
    this.notifyParent.emit(event);
  }

  updatePageSize(pageSize) {
    console.log(pageSize);
    this.pageSize = pageSize;
    let event = { maxResultCount: pageSize, skipCount: 0 };
    this.currentPage = 1;
    this.updateRows(event);
  }

  /** Set page number */
  selectPageNumber(pageNumber: number) {
    this.currentPage = pageNumber;
    let skipCount = (this.currentPage - 1) * this.pageSize;
    let event = { maxResultCount: this.pageSize, skipCount: skipCount };
    this.updateRows(event);
  }

  /** Set next page number */
  next() {
    const nextPage = this.currentPage + 1;
    nextPage <= this.totalPages.length && this.selectPageNumber(nextPage);
  }

  /** Set previous page number */
  previous() {
    const previousPage = this.currentPage - 1;
    previousPage >= 1 && this.selectPageNumber(previousPage);
  }

}
