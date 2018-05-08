/// <reference path="WidgetData.ts" />
/// <reference path="../typings/jquery/jquery.d.ts" />

module Overview {
    export class OverviewWidget implements IOverviewWidget {
        private data: IWidgetData;
        private templateId: string;
        private search: Object;
        private additionalSettings: Object;
        private renderer: IWidgetRenderer;
        private $widget: JQuery;

        constructor() {
        }

        public setRenderer(renderer: IWidgetRenderer) {
            this.renderer = renderer;
        }

        public setSearch(search: IWidgetSearch) {
            this.search = search;
        }

        public setTemplateId(templateId: string) {
            this.templateId = templateId;
        }

        public setWidget($widget: JQuery) {
            this.$widget = $widget;
        }

        public refresh() {
            if (typeof this.data === 'undefined' || this.data === null) {
                this.loadWidgetData();
            }

            this.render();
        }

        public loadWidgetData(): any {
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
        }

        public render() {
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
        }
    }
}