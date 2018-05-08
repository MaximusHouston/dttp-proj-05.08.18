/// <reference path="WidgetDataElement.ts" />

module Overview {
    export class GraphData implements IWidgetData {
        public Data: { [key: string]: Array<WidgetDataElement> }
    }
} 