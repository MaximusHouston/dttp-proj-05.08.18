import { drawing } from '@progress/kendo-drawing';
import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { LegendItemVisualArgs } from '../../common/property-types';
import { LegendItem } from '../../common/property-types';
/**
 * @hidden
 */
export declare abstract class LegendItemComponentGenerated extends SettingsComponent implements LegendItem {
    configurationService: ConfigurationService;
    cursor: string;
    visual: (e: LegendItemVisualArgs) => drawing.Element;
    constructor(configurationService: ConfigurationService);
}
