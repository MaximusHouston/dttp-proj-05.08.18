import { drawing } from '@progress/kendo-drawing';
import { ConfigurationService } from '../common/configuration.service';
import { SettingsComponent } from '../common/settings.component';
import { Border, Overlay, SeriesStack, SeriesType, SeriesVisualArgs } from '../common/property-types';
import { SeriesDefaultsLabels, SeriesDefaultsNotes, SeriesDefaultsTooltip, SeriesDefaults } from '../common/property-types';
/**
 * @hidden
 */
export declare abstract class SeriesDefaultsComponentGenerated extends SettingsComponent implements SeriesDefaults {
    configurationService: ConfigurationService;
    border: Border;
    gap: number;
    overlay: Overlay;
    spacing: number;
    stack: boolean | string | SeriesStack;
    type: SeriesType;
    visual: (e: SeriesVisualArgs) => drawing.Element;
    labels: SeriesDefaultsLabels;
    notes: SeriesDefaultsNotes;
    tooltip: SeriesDefaultsTooltip;
    constructor(configurationService: ConfigurationService);
}
