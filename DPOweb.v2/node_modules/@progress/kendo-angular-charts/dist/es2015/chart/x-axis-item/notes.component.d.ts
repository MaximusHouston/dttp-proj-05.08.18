import { ConfigurationService } from '../../common/configuration.service';
import { XAxisNotesComponentGenerated } from '../x-axis-item/notes.component.generated';
/**
 * The configuration of the X-axis notes.
 * For an example on the basic usage of the XAxisNotesComponent,
 * refer to the [demo on the XAxisComponent]({% slug api_charts_xaxiscomponent %})
 * or to the documentation about the
 * [axis notes]({% slug notes_chart_charts %}#toc-axis-notes).
 */
export declare class XAxisNotesComponent extends XAxisNotesComponentGenerated {
    configurationService: ConfigurationService;
    constructor(configurationService: ConfigurationService);
}
