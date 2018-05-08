import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { Border, Margin, Padding } from '../../common/property-types';
import { SeriesDefaultsLabelsFrom } from '../../common/property-types';
/**
 * @hidden
 */
export declare abstract class SeriesDefaultsLabelsFromComponentGenerated extends SettingsComponent implements SeriesDefaultsLabelsFrom {
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
