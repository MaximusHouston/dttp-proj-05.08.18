import { EventEmitter } from '@angular/core';
/**
 * @hidden
 */
export declare class ScrollViewPagerComponent {
    activeIndex: number;
    data: any[];
    pagerIndexChange: EventEmitter<number>;
    itemClass(idx: number): any;
    indexChange(selectedIndex: number): void;
}
