import { ConfigurationService } from './configuration.service';
/**
 * @hidden
 */
export declare class ThemeService extends ConfigurationService {
    private loaded;
    private element;
    loadTheme(): void;
    private setStyle(key, value);
    private setColors();
    private setFonts();
    private setSeriesColors();
    private mapColor(key, varName);
    private queryStyle(varName);
}
