import { ConfigurationService } from '../../../common/configuration.service';
import { SeriesTooltipComponent } from '../../../chart/series-item/tooltip.component';
/**
 * The tooltip configuration of the StockChart navigator series.
 * The StockChart navigator series tooltip is displayed when the `navigator.series.tooltip.visible` option is set to `true`.
 */
export declare class NavigatorSeriesTooltipComponent extends SeriesTooltipComponent {
    configurationService: ConfigurationService;
    constructor(configurationService: ConfigurationService);
}
