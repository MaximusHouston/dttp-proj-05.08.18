/// <reference path="Base/BaseWidgetRenderer.ts" />
/// <reference path="WidgetData.ts" />
/// <reference path="../typings/jquery/jquery.d.ts" />
var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var Overview;
(function (Overview) {
    var AlertElementRenderer = /** @class */ (function (_super) {
        __extends(AlertElementRenderer, _super);
        function AlertElementRenderer() {
            return _super.call(this) || this;
        }
        AlertElementRenderer.prototype.render = function ($widgets, data) {
            if (!_super.prototype.checkData.call(this, data)) {
                $widgets.each(function () {
                    $(this).find('.no-data').show();
                });
                return;
            }
            var alerts = [];
            for (var i = 0; i < data.Data.length; i++) {
                var alertStr = "<p>" + data.Data[i].Key + "</p>";
                alerts.push(alertStr);
            }
            var allAlertsStr = alerts.join('');
            $widgets.each(function () {
                var $widget = $(this);
                $widget.find('.contentbox').append($(allAlertsStr));
                $widget.find('.js-count').text('- ' + alerts.length);
            });
        };
        return AlertElementRenderer;
    }(Overview.BaseWidgetRenderer));
    Overview.AlertElementRenderer = AlertElementRenderer;
})(Overview || (Overview = {}));
//# sourceMappingURL=AlertElementRenderer.js.map