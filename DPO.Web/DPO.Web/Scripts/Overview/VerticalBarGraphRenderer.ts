/// <reference path="Base/BaseWidgetRenderer.ts" />
/// <reference path="../typings/jquery/jquery.d.ts" />

module Overview {

    export class VerticalBarGraphRenderer extends BaseWidgetRenderer implements IGraphWidgetRenderer {
        constructor() {
            super();
        }

        public setXDataSetName(dataSetName: string) {
        }

        public setY1DataSetName(dataSetName: string) {
        }

        public setY2DataSetName(dataSetName: string) {
        }
        public getY2DataSetName(): string {
            return "";
        }
        public setXLabel(label: string) {
        }

        public setY1Label(label: string) {
        }

        public setY2Label(label: string) {
        }
        public getY2Label(): string {
            return "";
            }

        render($widgets: JQuery, data: WidgetData) {
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
                var dataValue: string = data.Data[i].Value.toFixed(2)
                var barStr = '<p>' + data.Data[i].Key + '</p>' +
                    '<strong class="pull-left" style="margin-left: 86%;">' + dataValue + '%</strong>' +
                    '<div style="width: 82%;"><div class="amount-bar" style="width: ' + dataValue + '%">&nbsp;</div></div> ';

                bars.push(barStr);
            }

            var allBarsStr = bars.join('');

            $widgets.each(function () {
                $(this).find('.contentbox').append($(allBarsStr));
            });
        }
    }
} 