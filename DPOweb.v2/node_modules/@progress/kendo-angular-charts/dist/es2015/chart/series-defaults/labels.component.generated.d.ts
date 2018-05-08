import { drawing } from '@progress/kendo-drawing';
import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { Border, Margin, Padding, SeriesLabelsVisualArgs } from '../../common/property-types';
import { SeriesDefaultsLabelsFrom, SeriesDefaultsLabelsTo, SeriesDefaultsLabels } from '../../common/property-types';
/**
 * @hidden
 */
export declare abstract class SeriesDefaultsLabelsComponentGenerated extends SettingsComponent implements SeriesDefaultsLabels {
    configurationService: ConfigurationService;
    background: string;
    border: Border;
    color: string;
    content: (e: any) => string;
    font: string;
    format: string;
    margin: Margin | number;
    padding: Padding | number;
    visible: boolean;
    visual: (e: SeriesLabelsVisualArgs) => drawing.Element;
    from: SeriesDefaultsLabelsFrom;
    to: SeriesDefaultsLabelsTo;
    constructor(configurationService: ConfigurationService);
}
