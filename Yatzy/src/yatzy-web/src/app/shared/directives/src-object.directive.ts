import { Directive, ElementRef, Input, OnChanges } from '@angular/core';

/** Sets the non-reflectable `srcObject` property on a <video> element. */
@Directive({ selector: '[srcObject]', standalone: true })
export class SrcObjectDirective implements OnChanges {
  @Input() srcObject: MediaStream | null = null;

  constructor(private el: ElementRef<HTMLVideoElement>) {}

  ngOnChanges(): void {
    this.el.nativeElement.srcObject = this.srcObject;
  }
}
