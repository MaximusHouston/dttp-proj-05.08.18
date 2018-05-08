import { drawing } from '@progress/kendo-drawing';
import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { ErrorBarLine, ErrorBarsVisualArgs } from '../../common/property-types';
import { SeriesErrorBars } from '../../common/property-types';
/**
 * @hidden
 */
export declare abstract class SeriesErrorBarsComponentGenerated extends SettingsComponent implements SeriesErrorBars {
    configurationService: ConfigurationService;
    color: string;
    endCaps: boolean;
    line: ErrorBarLine;
    value: string;
    visual: (e: ErrorBarsVisualArgs) => drawing.Element;
    xValue: string;
    yValue: string;
    constructor(configurationService: ConfigurationService);
}
