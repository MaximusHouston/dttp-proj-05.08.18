import { ConfigurationService } from '../../common/configuration.service';
import { YAxisCrosshairTooltipComponentGenerated } from '../y-axis-item/crosshair.tooltip.component.generated';
/**
 * The configuration options of the crosshair tooltip.
 * The crosshair tooltip is displayed when the `visible` option is set to `true`.
 */
export declare class YAxisCrosshairTooltipComponent extends YAxisCrosshairTooltipComponentGenerated {
    configurationService: ConfigurationService;
    constructor(configurationService: ConfigurationService);
}
