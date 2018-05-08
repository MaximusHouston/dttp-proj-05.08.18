/// <reference path="Base/BaseWidgetRenderer.ts" />
/// <reference path="WidgetData.ts" />
/// <reference path="../typings/jquery/jquery.d.ts" />

module Overview {
    export class AlertElementRenderer extends BaseWidgetRenderer {

        constructor() {
            super();
        }

        render($widgets: JQuery, data: WidgetData) {
            if (!super.checkData(data)) {
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
        }
    }
} 