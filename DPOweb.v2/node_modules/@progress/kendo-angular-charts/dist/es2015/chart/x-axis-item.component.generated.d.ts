import { CollectionService } from '../common/collection.service';
import { CollectionItemComponent } from '../common/collection-item.component';
import { ConfigurationService } from '../common/configuration.service';
import { AxisLine, AxisTicks, BaseUnit, GridLines, PlotBand } from '../common/property-types';
import { XAxisCrosshair, XAxisLabels, XAxisNotes, XAxisTitle, XAxis } from '../common/property-types';
/**
 * @hidden
 */
export declare abstract class XAxisItemComponentGenerated extends CollectionItemComponent implements XAxis {
    protected configurationService: ConfigurationService;
    protected collectionService: CollectionService;
    axisCrossingValue: any | any[];
    background: string;
    baseUnit: BaseUnit;
    color: string;
    line: AxisLine;
    majorGridLines: GridLines;
    majorTicks: AxisTicks;
    majorUnit: number;
    max: any;
    min: any;
    minorGridLines: GridLines;
    minorTicks: AxisTicks;
    minorUnit: number;
    name: string;
    narrowRange: boolean;
    pane: string;
    plotBands: PlotBand[];
    reverse: boolean;
    startAngle: number;
    type: 'numeric' | 'log' | 'date';
    visible: boolean;
    crosshair: XAxisCrosshair;
    labels: XAxisLabels;
    notes: XAxisNotes;
    title: XAxisTitle;
    constructor(configurationService: ConfigurationService, collectionService: CollectionService);
}
