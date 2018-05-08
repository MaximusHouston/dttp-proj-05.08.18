import { ConfigurationService } from '../common/configuration.service';
import { SettingsComponent } from '../common/settings.component';
import { Border, Margin, Padding } from '../common/property-types';
import { PlotArea } from '../common/property-types';
/**
 * @hidden
 */
export declare abstract class PlotAreaComponentGenerated extends SettingsComponent implements PlotArea {
    configurationService: ConfigurationService;
    background: string;
    border: Border;
    margin: Margin | number;
    opacity: number;
    padding: Padding | number;
    constructor(configurationService: ConfigurationService);
}
