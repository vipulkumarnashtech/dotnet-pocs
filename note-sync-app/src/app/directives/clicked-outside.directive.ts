import {
  Directive,
  ElementRef,
  EventEmitter,
  HostListener,
  Output,
} from '@angular/core';

@Directive({
  selector: '[appClickedOutside]',
})
export class ClickedOutsideDirective {
  constructor(private el: ElementRef) {}

  @Output() public clickedOutside = new EventEmitter();
  @Output() public clickedInside = new EventEmitter();

  @HostListener('document:click', ['$event.target'])
  public onClick(target: any) {
    const isClickedInside = this.el.nativeElement.contains(target);
    if (!isClickedInside) {
      this.clickedOutside.emit(target);
    } else {
      this.clickedInside.emit(target);
    }
  }
}
