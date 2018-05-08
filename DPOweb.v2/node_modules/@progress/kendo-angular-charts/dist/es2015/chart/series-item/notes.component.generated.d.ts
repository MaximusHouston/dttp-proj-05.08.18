import { drawing } from '@progress/kendo-drawing';
import { ConfigurationService } from '../../common/configuration.service';
import { SettingsComponent } from '../../common/settings.component';
import { NoteLine, NotePosition, SeriesNoteVisualArgs } from '../../common/property-types';
import { SeriesNotesIcon, SeriesNotesLabel, SeriesNotes } from '../../common/property-types';
/**
 * @hidden
 */
export declare abstract class SeriesNotesComponentGenerated extends SettingsComponent implements SeriesNotes {
    configurationService: ConfigurationService;
    line: NoteLine;
    position: NotePosition;
    visual: (e: SeriesNoteVisualArgs) => drawing.Element;
    icon: SeriesNotesIcon;
    label: SeriesNotesLabel;
    constructor(configurationService: ConfigurationService);
}
