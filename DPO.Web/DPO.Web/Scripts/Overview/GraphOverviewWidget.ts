
module Overview {
    export class GraphOverviewWidget implements IOverviewWidget {
        private data: IWidgetData;
        private $widget: JQuery;
        private templateId: string;
        private search: Object;
        private additionalSettings: Object;
        private renderer: IGraphWidgetRenderer;

        constructor() {
           
        }
        
        public setRenderer(renderer: IGraphWidgetRenderer) {
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

        public setY2DataSetName(dataSetName: string) {
            this.renderer.setY2DataSetName(dataSetName);
        }

        public getY2DataSetName(): string {
            return this.renderer.getY2DataSetName();
        }


        public getY2Label(): string {
            return this.renderer.getY2Label();
        }
        public setY2Label(label: string) {
            this.renderer.setY2Label(label);
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
                    me.data = null;
                    me.render();
                }
            });
        }

        public refresh() {
            if (typeof this.data === 'undefined' || this.data === null) {
                this.loadWidgetData();
            }
            else {
                this.render();
            }
        }

        public render() {
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
        }
    }
} 