import { ConfigurationService } from '../../../common/configuration.service';
import { PanesTitleComponent } from '../../../chart/pane/title.component';
/**
 * The title configuration of the StockChart navigator pane.
 */
export declare class NavigatorPaneTitleComponent extends PanesTitleComponent {
    configurationService: ConfigurationService;
    constructor(configurationService: ConfigurationService);
}
