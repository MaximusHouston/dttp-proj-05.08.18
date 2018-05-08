var Overview;
(function (Overview) {
    var GraphOverviewWidget = /** @class */ (function () {
        function GraphOverviewWidget() {
        }
        GraphOverviewWidget.prototype.setRenderer = function (renderer) {
            this.renderer = renderer;
        };
        GraphOverviewWidget.prototype.setSearch = function (search) {
            this.search = search;
        };
        GraphOverviewWidget.prototype.setTemplateId = function (templateId) {
            this.templateId = templateId;
        };
        GraphOverviewWidget.prototype.setWidget = function ($widget) {
            this.$widget = $widget;
        };
        GraphOverviewWidget.prototype.setY2DataSetName = function (dataSetName) {
            this.renderer.setY2DataSetName(dataSetName);
        };
        GraphOverviewWidget.prototype.getY2DataSetName = function () {
            return this.renderer.getY2DataSetName();
        };
        GraphOverviewWidget.prototype.getY2Label = function () {
            return this.renderer.getY2Label();
        };
        GraphOverviewWidget.prototype.setY2Label = function (label) {
            this.renderer.setY2Label(label);
        };
        GraphOverviewWidget.prototype.loadWidgetData = function () {
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
                    me.data = null;
                    me.render();
                }
            });
        };
        GraphOverviewWidget.prototype.refresh = function () {
            if (typeof this.data === 'undefined' || this.data === null) {
                this.loadWidgetData();
            }
            else {
                this.render();
            }
        };
        GraphOverviewWidget.prototype.render = function () {
            if (typeof this.data === 'undefined' || this.data === null) {
                this.$widget.find('.contentbox, .contentbox-footer').hide();
                this.$widget.find('.contentbox-error').show();
                this.$widget.removeClass('loading');
                return;
            }
            if (this.renderer != null) {
                this.renderer.render(this.$widget, this.data);
            }
            this.$widget.removeClass('loading');
        };
        return GraphOverviewWidget;
    }());
    Overview.GraphOverviewWidget = GraphOverviewWidget;
})(Overview || (Overview = {}));
//# sourceMappingURL=GraphOverviewWidget.js.map