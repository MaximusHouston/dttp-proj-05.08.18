/// <reference path="../Interfaces/IWidgetRenderer.ts" />

module Overview {

    export class BaseWidgetRenderer implements IWidgetRenderer {

        public checkData(data: WidgetData): Boolean {
            return data != null && data.Data != null && data.Data.length != null;
        }

        public render($widgets: JQuery, data: WidgetData) : void {
            return;
        }
    }
}