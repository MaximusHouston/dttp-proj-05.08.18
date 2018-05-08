/// <reference path="Base/BaseWidgetRenderer.ts" />
/// <reference path="WidgetData.ts" />
/// <reference path="../typings/jquery/jquery.d.ts" />

module Overview {
    export class TextElementRenderer extends BaseWidgetRenderer {

        render($widgets: JQuery, data: WidgetData) {
            super.render($widgets, data);
            if (!super.checkData(data)) {
                $widgets.each(function () {
                    $(this).find('.no-data').show();
                });

                return;
            }

            var me = this;

            $widgets.each(function () {
                $(this).find('.data').text(me.getDataValue(data));
            });
        }

        private getDataValue(data: WidgetData): number {
            if (super.checkData(data)) {
                var dataEl = data.Data[0];

                if (dataEl && dataEl.Value) {
                    return <number>dataEl.Value;
                }
            }

            return 0;
        }
    }
} 