/// <reference path="WidgetData.ts" />
/// <reference path="../typings/jquery/jquery.d.ts" />
var Overview;
(function (Overview) {
    var OverviewWidget = /** @class */ (function () {
        function OverviewWidget() {
        }
        OverviewWidget.prototype.setRenderer = function (renderer) {
            this.renderer = renderer;
        };
        OverviewWidget.prototype.setSearch = function (search) {
            this.search = search;
        };
        OverviewWidget.prototype.setTemplateId = function (templateId) {
            this.templateId = templateId;
        };
        OverviewWidget.prototype.setWidget = function ($widget) {
            this.$widget = $widget;
        };
        OverviewWidget.prototype.refresh = function () {
            if (typeof this.data === 'undefined' || this.data === null) {
                this.loadWidgetData();
            }
            this.render();
        };
        OverviewWidget.prototype.loadWidgetData = function () {
            var templateId = this.templateId;
            var me = this;
            this.$widget.addClass("loading");
            $.ajax({
                type: "POST",
                url: '/Overview/OverviewTemplateData',
                dataType: 'json',
                contentType: 'application/json',
                cache: false,
                data: JSON.stringify({
                    widget: {
                        templateId: templateId
                    },
                    container: me.search
                }),
                success: function (result) {
                    me.data = result;
                    me.$widget.removeClass("loading");
                    me.render();
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    me.render();
                }
            });
        };
        OverviewWidget.prototype.render = function () {
            var $allWidgets = $('li[data-templateId="' + this.templateId + '"]');
            if (typeof this.data === 'undefined' || this.data === null) {
                $allWidgets.each(function () {
                    var $widget = $(this);
                    $widget.find('.contentbox, .contentbox-footer').hide();
                    $widget.find('.contentbox-error').show();
                    $widget.removeClass('loading');
                });
                return;
            }
            if (this.renderer != null) {
                this.renderer.render($allWidgets, this.data);
            }
            $allWidgets.removeClass('loading');
        };
        return OverviewWidget;
    }());
    Overview.OverviewWidget = OverviewWidget;
})(Overview || (Overview = {}));
//# sourceMappingURL=OverviewWidget.js.map