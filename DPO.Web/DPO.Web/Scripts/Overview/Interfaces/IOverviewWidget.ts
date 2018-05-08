/// <reference path="IWidgetRenderer.ts" />
/// <reference path="../../typings/jquery/jquery.d.ts" />

module Overview {
    export interface IOverviewWidget {
        setSearch(search: IWidgetSearch);
        setRenderer(renderer: IWidgetRenderer);
        setTemplateId(templateId: string);
        setWidget($widget: JQuery);
        loadWidgetData(): any;
        render(): void;
        refresh(): void;
    }
} 