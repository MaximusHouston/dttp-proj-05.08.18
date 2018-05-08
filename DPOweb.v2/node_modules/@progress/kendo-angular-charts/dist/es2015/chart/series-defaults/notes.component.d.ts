import { ConfigurationService } from '../../common/configuration.service';
import { SeriesDefaultsNotesComponentGenerated } from '../series-defaults/notes.component.generated';
/**
 * The configuration of the [`seriesDefaults`]({% slug api_charts_seriesdefaultscomponent %}) notes.
 */
export declare class SeriesDefaultsNotesComponent extends SeriesDefaultsNotesComponentGenerated {
    configurationService: ConfigurationService;
    constructor(configurationService: ConfigurationService);
}
