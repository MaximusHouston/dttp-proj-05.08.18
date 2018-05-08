import { CollectionService } from '../common/collection.service';
import { CollectionItemComponent } from '../common/collection-item.component';
import { ConfigurationService } from '../common/configuration.service';
import { Border, Margin, Padding } from '../common/property-types';
import { PanesTitle, Pane } from '../common/property-types';
/**
 * @hidden
 */
export declare abstract class PaneComponentGenerated extends CollectionItemComponent implements Pane {
    protected configurationService: ConfigurationService;
    protected collectionService: CollectionService;
    background: string;
    border: Border;
    clip: boolean;
    height: number;
    margin: Margin | number;
    name: string;
    padding: Padding | number;
    title: string | PanesTitle;
    constructor(configurationService: ConfigurationService, collectionService: CollectionService);
}
