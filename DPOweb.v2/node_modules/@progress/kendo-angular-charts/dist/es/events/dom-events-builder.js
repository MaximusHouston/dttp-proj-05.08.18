import { isDevMode } from '@angular/core';
import { DomEvents } from './dom-events';
var MISSING_HAMMER_MESSAGE = 'Hammerjs is not loaded.' +
    'Solution: http://www.telerik.com/kendo-angular-ui/components/charts/troubleshooting/#toc-hammerjs-is-not-loaded';
/**
 * @hidden
 */
var DomEventsBuilder = /** @class */ (function () {
    function DomEventsBuilder() {
    }
    DomEventsBuilder.create = function (element, events) {
        if (typeof window !== 'undefined') {
            var HAMMER = window.Hammer;
            if (!HAMMER) {
                if (isDevMode()) {
                    throw new Error(MISSING_HAMMER_MESSAGE);
                }
                return;
            }
            var hammerInstance = new HAMMER(element, {
                recognizers: [
                    [HAMMER.Tap],
                    [HAMMER.Pan],
                    [HAMMER.Pinch],
                    [HAMMER.Press, { time: 0 }]
                ]
            });
            return new DomEvents(hammerInstance, events);
        }
    };
    return DomEventsBuilder;
}());
export { DomEventsBuilder };
