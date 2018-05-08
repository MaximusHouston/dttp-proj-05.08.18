import { drawing } from '@progress/kendo-drawing';
import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { AxisNoteVisualArgs, NoteLine, NotePosition } from '../../common/property-types';
import { XAxisNotesIcon, XAxisNotesLabel, XAxisNotes } from '../../common/property-types';
/**
 * @hidden
 */
export declare abstract class XAxisNotesComponentGenerated extends SettingsComponent implements XAxisNotes {
    configurationService: ConfigurationService;
    data: any[];
    line: NoteLine;
    position: NotePosition;
    visual: (e: AxisNoteVisualArgs) => drawing.Element;
    icon: XAxisNotesIcon;
    label: XAxisNotesLabel;
    constructor(configurationService: ConfigurationService);
}
