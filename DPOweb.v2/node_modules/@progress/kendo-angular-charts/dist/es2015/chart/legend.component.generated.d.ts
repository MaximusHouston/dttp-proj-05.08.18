import { ConfigurationService } from '../common/configuration.service';
import { SettingsComponent } from '../common/settings.component';
import { Border, LegendLabels, Margin, Padding } from '../common/property-types';
import { LegendInactiveItems, LegendItem, Legend } from '../common/property-types';
/**
 * @hidden
 */
export declare abstract class LegendComponentGenerated extends SettingsComponent implements Legend {
    configurationService: ConfigurationService;
    align: 'start' | 'center' | 'end';
    background: string;
    border: Border;
    height: number;
    labels: LegendLabels;
    margin: Margin | number;
    offsetX: number;
    offsetY: number;
    orientation: 'vertical' | 'horizontal';
    padding: Padding | number;
    position: 'top' | 'bottom' | 'left' | 'right' | 'custom';
    reverse: boolean;
    visible: boolean;
    width: number;
    inactiveItems: LegendInactiveItems;
    item: LegendItem;
    constructor(configurationService: ConfigurationService);
}
