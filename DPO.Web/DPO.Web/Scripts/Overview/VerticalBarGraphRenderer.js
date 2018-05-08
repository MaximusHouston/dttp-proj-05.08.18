/// <reference path="Base/BaseWidgetRenderer.ts" />
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
    var VerticalBarGraphRenderer = /** @class */ (function (_super) {
        __extends(VerticalBarGraphRenderer, _super);
        function VerticalBarGraphRenderer() {
            return _super.call(this) || this;
        }
        VerticalBarGraphRenderer.prototype.setXDataSetName = function (dataSetName) {
        };
        VerticalBarGraphRenderer.prototype.setY1DataSetName = function (dataSetName) {
        };
        VerticalBarGraphRenderer.prototype.setY2DataSetName = function (dataSetName) {
        };
        VerticalBarGraphRenderer.prototype.getY2DataSetName = function () {
            return "";
        };
        VerticalBarGraphRenderer.prototype.setXLabel = function (label) {
        };
        VerticalBarGraphRenderer.prototype.setY1Label = function (label) {
        };
        VerticalBarGraphRenderer.prototype.setY2Label = function (label) {
        };
        VerticalBarGraphRenderer.prototype.getY2Label = function () {
            return "";
        };
        VerticalBarGraphRenderer.prototype.render = function ($widgets, data) {
            if (!this.checkData(data)) {
                $widgets.each(function () {
                    $(this).find('.no-data').show();
                });
                return;
            }
            $widgets.each(function () {
                $(this).find('.contentbox').empty();
            });
            var bars = [];
            for (var i = 0; i < data.Data.length; i++) {
                var dataValue = data.Data[i].Value.toFixed(2);
                var barStr = '<p>' + data.Data[i].Key + '</p>' +
                    '<strong class="pull-left" style="margin-left: 86%;">' + dataValue + '%</strong>' +
                    '<div style="width: 82%;"><div class="amount-bar" style="width: ' + dataValue + '%">&nbsp;</div></div> ';
                bars.push(barStr);
            }
            var allBarsStr = bars.join('');
            $widgets.each(function () {
                $(this).find('.contentbox').append($(allBarsStr));
            });
        };
        return VerticalBarGraphRenderer;
    }(Overview.BaseWidgetRenderer));
    Overview.VerticalBarGraphRenderer = VerticalBarGraphRenderer;
})(Overview || (Overview = {}));
//# sourceMappingURL=VerticalBarGraphRenderer.js.map