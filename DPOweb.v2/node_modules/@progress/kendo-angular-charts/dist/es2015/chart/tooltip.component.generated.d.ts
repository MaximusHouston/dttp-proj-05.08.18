import { ConfigurationService } from '../common/configuration.service';
import { SettingsComponent } from '../common/settings.component';
import { Border, Padding } from '../common/property-types';
import { Tooltip } from '../common/property-types';
/**
 * @hidden
 */
export declare abstract class TooltipComponentGenerated extends SettingsComponent implements Tooltip {
    configurationService: ConfigurationService;
    background: string;
    border: Border;
    color: string;
    font: string;
    format: string;
    opacity: number;
    padding: Padding | number;
    shared: boolean;
    visible: boolean;
    constructor(configurationService: ConfigurationService);
}
