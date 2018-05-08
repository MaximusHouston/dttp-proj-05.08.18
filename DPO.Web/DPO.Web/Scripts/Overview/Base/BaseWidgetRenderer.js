/// <reference path="../Interfaces/IWidgetRenderer.ts" />
var Overview;
(function (Overview) {
    var BaseWidgetRenderer = /** @class */ (function () {
        function BaseWidgetRenderer() {
        }
        BaseWidgetRenderer.prototype.checkData = function (data) {
            return data != null && data.Data != null && data.Data.length != null;
        };
        BaseWidgetRenderer.prototype.render = function ($widgets, data) {
            return;
        };
        return BaseWidgetRenderer;
    }());
    Overview.BaseWidgetRenderer = BaseWidgetRenderer;
})(Overview || (Overview = {}));
//# sourceMappingURL=BaseWidgetRenderer.js.map