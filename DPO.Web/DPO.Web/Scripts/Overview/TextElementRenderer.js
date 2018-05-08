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
    var TextElementRenderer = /** @class */ (function (_super) {
        __extends(TextElementRenderer, _super);
        function TextElementRenderer() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        TextElementRenderer.prototype.render = function ($widgets, data) {
            _super.prototype.render.call(this, $widgets, data);
            if (!_super.prototype.checkData.call(this, data)) {
                $widgets.each(function () {
                    $(this).find('.no-data').show();
                });
                return;
            }
            var me = this;
            $widgets.each(function () {
                $(this).find('.data').text(me.getDataValue(data));
            });
        };
        TextElementRenderer.prototype.getDataValue = function (data) {
            if (_super.prototype.checkData.call(this, data)) {
                var dataEl = data.Data[0];
                if (dataEl && dataEl.Value) {
                    return dataEl.Value;
                }
            }
            return 0;
        };
        return TextElementRenderer;
    }(Overview.BaseWidgetRenderer));
    Overview.TextElementRenderer = TextElementRenderer;
})(Overview || (Overview = {}));
//# sourceMappingURL=TextElementRenderer.js.map