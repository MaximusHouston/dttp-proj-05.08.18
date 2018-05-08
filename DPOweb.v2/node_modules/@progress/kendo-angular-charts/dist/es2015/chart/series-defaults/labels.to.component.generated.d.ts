import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { Border, Margin, Padding } from '../../common/property-types';
import { SeriesDefaultsLabelsTo } from '../../common/property-types';
/**
 * @hidden
 */
export declare abstract class SeriesDefaultsLabelsToComponentGenerated extends SettingsComponent implements SeriesDefaultsLabelsTo {
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
    constructor(configurationService: ConfigurationService);
}
