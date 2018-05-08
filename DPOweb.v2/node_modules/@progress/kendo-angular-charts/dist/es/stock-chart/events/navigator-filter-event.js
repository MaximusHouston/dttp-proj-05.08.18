import * as tslib_1 from "tslib";
import { BaseEvent } from '../../events/base-event';
/**
 * The arguments for the `navigatorFilter` event.
 */
var NavigatorFilterEvent = /** @class */ (function (_super) {
    tslib_1.__extends(NavigatorFilterEvent, _super);
    /**
     * Constructs the event arguments from a raw object.
     */
    function NavigatorFilterEvent(e, sender) {
        var _this = _super.call(this, sender) || this;
        _this.from = e.from;
        _this.to = e.to;
        return _this;
    }
    return NavigatorFilterEvent;
}(BaseEvent));
export { NavigatorFilterEvent };
