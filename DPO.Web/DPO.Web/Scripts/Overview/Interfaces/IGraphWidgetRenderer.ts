/// <reference path="../WidgetData.ts" />
/// <reference path="../../typings/jquery/jquery.d.ts" />

module Overview {
    export interface IGraphWidgetRenderer extends IWidgetRenderer {
        setXDataSetName(dataSetName: string);
        setY1DataSetName(dataSetName: string);
        setY2DataSetName(dataSetName: string);
        getY2Label(): string;
        getY2DataSetName(): string;
        setXLabel(label: string);
        setY1Label(label: string);
        setY2Label(label: string);
    }
} 