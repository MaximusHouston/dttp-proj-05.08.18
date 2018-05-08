/// <reference path="../WidgetData.ts" />
/// <reference path="../../typings/jquery/jquery.d.ts" />

module Overview {
    export interface IWidgetRenderer {
        render($allWidgets: JQuery, data: WidgetData): void
        checkData(data: WidgetData): Boolean
    }
} 