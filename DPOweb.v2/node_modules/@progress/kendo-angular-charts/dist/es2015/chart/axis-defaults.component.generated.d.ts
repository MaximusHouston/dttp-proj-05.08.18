import { ConfigurationService } from '../common/configuration.service';
import { SettingsComponent } from '../common/settings.component';
import { AxisLine, AxisTicks, GridLines, PlotBand } from '../common/property-types';
import { AxisDefaultsCrosshair, AxisDefaultsLabels, AxisDefaultsTitle, AxisDefaults } from '../common/property-types';
/**
 * @hidden
 */
export declare abstract class AxisDefaultsComponentGenerated extends SettingsComponent implements AxisDefaults {
    configurationService: ConfigurationService;
    background: string;
    color: string;
    line: AxisLine;
    majorGridLines: GridLines;
    majorTicks: AxisTicks;
    minorGridLines: GridLines;
    minorTicks: AxisTicks;
    narrowRange: boolean;
    pane: string;
    plotBands: PlotBand[];
    reverse: boolean;
    startAngle: number;
    visible: boolean;
    crosshair: AxisDefaultsCrosshair;
    labels: AxisDefaultsLabels;
    title: AxisDefaultsTitle;
    constructor(configurationService: ConfigurationService);
}
