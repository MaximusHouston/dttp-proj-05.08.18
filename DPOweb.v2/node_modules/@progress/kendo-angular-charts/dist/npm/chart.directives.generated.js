"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var axis_defaults_component_1 = require("./chart/axis-defaults.component");
var crosshair_component_1 = require("./chart/axis-defaults/crosshair.component");
var crosshair_tooltip_component_1 = require("./chart/axis-defaults/crosshair.tooltip.component");
var labels_component_1 = require("./chart/axis-defaults/labels.component");
var title_component_1 = require("./chart/axis-defaults/title.component");
var category_axis_component_1 = require("./chart/category-axis.component");
var crosshair_component_2 = require("./chart/category-axis-item/crosshair.component");
var crosshair_tooltip_component_2 = require("./chart/category-axis-item/crosshair.tooltip.component");
var category_axis_item_component_1 = require("./chart/category-axis-item.component");
var labels_component_2 = require("./chart/category-axis-item/labels.component");
var notes_component_1 = require("./chart/category-axis-item/notes.component");
var notes_icon_component_1 = require("./chart/category-axis-item/notes.icon.component");
var notes_label_component_1 = require("./chart/category-axis-item/notes.label.component");
var select_component_1 = require("./chart/category-axis-item/select.component");
var title_component_2 = require("./chart/category-axis-item/title.component");
var chart_area_component_1 = require("./chart/chart-area.component");
var legend_component_1 = require("./chart/legend.component");
var inactive_items_component_1 = require("./chart/legend/inactive-items.component");
var item_component_1 = require("./chart/legend/item.component");
var pane_component_1 = require("./chart/pane.component");
var panes_component_1 = require("./chart/panes.component");
var title_component_3 = require("./chart/pane/title.component");
var plot_area_component_1 = require("./chart/plot-area.component");
var series_component_1 = require("./chart/series.component");
var series_defaults_component_1 = require("./chart/series-defaults.component");
var labels_component_3 = require("./chart/series-defaults/labels.component");
var labels_from_component_1 = require("./chart/series-defaults/labels.from.component");
var labels_to_component_1 = require("./chart/series-defaults/labels.to.component");
var notes_component_2 = require("./chart/series-defaults/notes.component");
var notes_icon_component_2 = require("./chart/series-defaults/notes.icon.component");
var notes_label_component_2 = require("./chart/series-defaults/notes.label.component");
var tooltip_component_1 = require("./chart/series-defaults/tooltip.component");
var error_bars_component_1 = require("./chart/series-item/error-bars.component");
var extremes_component_1 = require("./chart/series-item/extremes.component");
var highlight_component_1 = require("./chart/series-item/highlight.component");
var series_item_component_1 = require("./chart/series-item.component");
var labels_component_4 = require("./chart/series-item/labels.component");
var labels_from_component_2 = require("./chart/series-item/labels.from.component");
var labels_to_component_2 = require("./chart/series-item/labels.to.component");
var markers_component_1 = require("./chart/series-item/markers.component");
var notes_component_3 = require("./chart/series-item/notes.component");
var notes_icon_component_3 = require("./chart/series-item/notes.icon.component");
var notes_label_component_3 = require("./chart/series-item/notes.label.component");
var outliers_component_1 = require("./chart/series-item/outliers.component");
var tooltip_component_2 = require("./chart/series-item/tooltip.component");
var title_component_4 = require("./chart/title.component");
var tooltip_component_3 = require("./chart/tooltip.component");
var value_axis_component_1 = require("./chart/value-axis.component");
var crosshair_component_3 = require("./chart/value-axis-item/crosshair.component");
var crosshair_tooltip_component_3 = require("./chart/value-axis-item/crosshair.tooltip.component");
var value_axis_item_component_1 = require("./chart/value-axis-item.component");
var labels_component_5 = require("./chart/value-axis-item/labels.component");
var notes_component_4 = require("./chart/value-axis-item/notes.component");
var notes_icon_component_4 = require("./chart/value-axis-item/notes.icon.component");
var notes_label_component_4 = require("./chart/value-axis-item/notes.label.component");
var title_component_5 = require("./chart/value-axis-item/title.component");
var x_axis_component_1 = require("./chart/x-axis.component");
var crosshair_component_4 = require("./chart/x-axis-item/crosshair.component");
var crosshair_tooltip_component_4 = require("./chart/x-axis-item/crosshair.tooltip.component");
var x_axis_item_component_1 = require("./chart/x-axis-item.component");
var labels_component_6 = require("./chart/x-axis-item/labels.component");
var notes_component_5 = require("./chart/x-axis-item/notes.component");
var notes_icon_component_5 = require("./chart/x-axis-item/notes.icon.component");
var notes_label_component_5 = require("./chart/x-axis-item/notes.label.component");
var title_component_6 = require("./chart/x-axis-item/title.component");
var y_axis_component_1 = require("./chart/y-axis.component");
var crosshair_component_5 = require("./chart/y-axis-item/crosshair.component");
var crosshair_tooltip_component_5 = require("./chart/y-axis-item/crosshair.tooltip.component");
var y_axis_item_component_1 = require("./chart/y-axis-item.component");
var labels_component_7 = require("./chart/y-axis-item/labels.component");
var notes_component_6 = require("./chart/y-axis-item/notes.component");
var notes_icon_component_6 = require("./chart/y-axis-item/notes.icon.component");
var notes_label_component_6 = require("./chart/y-axis-item/notes.label.component");
var title_component_7 = require("./chart/y-axis-item/title.component");
var zoomable_component_1 = require("./chart/zoomable.component");
// Re-exports
var axis_defaults_component_2 = require("./chart/axis-defaults.component");
exports.AxisDefaultsComponent = axis_defaults_component_2.AxisDefaultsComponent;
var crosshair_component_6 = require("./chart/axis-defaults/crosshair.component");
exports.AxisDefaultsCrosshairComponent = crosshair_component_6.AxisDefaultsCrosshairComponent;
var crosshair_tooltip_component_6 = require("./chart/axis-defaults/crosshair.tooltip.component");
exports.AxisDefaultsCrosshairTooltipComponent = crosshair_tooltip_component_6.AxisDefaultsCrosshairTooltipComponent;
var labels_component_8 = require("./chart/axis-defaults/labels.component");
exports.AxisDefaultsLabelsComponent = labels_component_8.AxisDefaultsLabelsComponent;
var title_component_8 = require("./chart/axis-defaults/title.component");
exports.AxisDefaultsTitleComponent = title_component_8.AxisDefaultsTitleComponent;
var category_axis_component_2 = require("./chart/category-axis.component");
exports.CategoryAxisComponent = category_axis_component_2.CategoryAxisComponent;
var crosshair_component_7 = require("./chart/category-axis-item/crosshair.component");
exports.CategoryAxisCrosshairComponent = crosshair_component_7.CategoryAxisCrosshairComponent;
var crosshair_tooltip_component_7 = require("./chart/category-axis-item/crosshair.tooltip.component");
exports.CategoryAxisCrosshairTooltipComponent = crosshair_tooltip_component_7.CategoryAxisCrosshairTooltipComponent;
var category_axis_item_component_2 = require("./chart/category-axis-item.component");
exports.CategoryAxisItemComponent = category_axis_item_component_2.CategoryAxisItemComponent;
var labels_component_9 = require("./chart/category-axis-item/labels.component");
exports.CategoryAxisLabelsComponent = labels_component_9.CategoryAxisLabelsComponent;
var notes_component_7 = require("./chart/category-axis-item/notes.component");
exports.CategoryAxisNotesComponent = notes_component_7.CategoryAxisNotesComponent;
var notes_icon_component_7 = require("./chart/category-axis-item/notes.icon.component");
exports.CategoryAxisNotesIconComponent = notes_icon_component_7.CategoryAxisNotesIconComponent;
var notes_label_component_7 = require("./chart/category-axis-item/notes.label.component");
exports.CategoryAxisNotesLabelComponent = notes_label_component_7.CategoryAxisNotesLabelComponent;
var select_component_2 = require("./chart/category-axis-item/select.component");
exports.CategoryAxisSelectComponent = select_component_2.CategoryAxisSelectComponent;
var title_component_9 = require("./chart/category-axis-item/title.component");
exports.CategoryAxisTitleComponent = title_component_9.CategoryAxisTitleComponent;
var chart_area_component_2 = require("./chart/chart-area.component");
exports.ChartAreaComponent = chart_area_component_2.ChartAreaComponent;
var legend_component_2 = require("./chart/legend.component");
exports.LegendComponent = legend_component_2.LegendComponent;
var inactive_items_component_2 = require("./chart/legend/inactive-items.component");
exports.LegendInactiveItemsComponent = inactive_items_component_2.LegendInactiveItemsComponent;
var item_component_2 = require("./chart/legend/item.component");
exports.LegendItemComponent = item_component_2.LegendItemComponent;
var pane_component_2 = require("./chart/pane.component");
exports.PaneComponent = pane_component_2.PaneComponent;
var panes_component_2 = require("./chart/panes.component");
exports.PanesComponent = panes_component_2.PanesComponent;
var title_component_10 = require("./chart/pane/title.component");
exports.PanesTitleComponent = title_component_10.PanesTitleComponent;
var plot_area_component_2 = require("./chart/plot-area.component");
exports.PlotAreaComponent = plot_area_component_2.PlotAreaComponent;
var series_component_2 = require("./chart/series.component");
exports.SeriesComponent = series_component_2.SeriesComponent;
var series_defaults_component_2 = require("./chart/series-defaults.component");
exports.SeriesDefaultsComponent = series_defaults_component_2.SeriesDefaultsComponent;
var labels_component_10 = require("./chart/series-defaults/labels.component");
exports.SeriesDefaultsLabelsComponent = labels_component_10.SeriesDefaultsLabelsComponent;
var labels_from_component_3 = require("./chart/series-defaults/labels.from.component");
exports.SeriesDefaultsLabelsFromComponent = labels_from_component_3.SeriesDefaultsLabelsFromComponent;
var labels_to_component_3 = require("./chart/series-defaults/labels.to.component");
exports.SeriesDefaultsLabelsToComponent = labels_to_component_3.SeriesDefaultsLabelsToComponent;
var notes_component_8 = require("./chart/series-defaults/notes.component");
exports.SeriesDefaultsNotesComponent = notes_component_8.SeriesDefaultsNotesComponent;
var notes_icon_component_8 = require("./chart/series-defaults/notes.icon.component");
exports.SeriesDefaultsNotesIconComponent = notes_icon_component_8.SeriesDefaultsNotesIconComponent;
var notes_label_component_8 = require("./chart/series-defaults/notes.label.component");
exports.SeriesDefaultsNotesLabelComponent = notes_label_component_8.SeriesDefaultsNotesLabelComponent;
var tooltip_component_4 = require("./chart/series-defaults/tooltip.component");
exports.SeriesDefaultsTooltipComponent = tooltip_component_4.SeriesDefaultsTooltipComponent;
var error_bars_component_2 = require("./chart/series-item/error-bars.component");
exports.SeriesErrorBarsComponent = error_bars_component_2.SeriesErrorBarsComponent;
var extremes_component_2 = require("./chart/series-item/extremes.component");
exports.SeriesExtremesComponent = extremes_component_2.SeriesExtremesComponent;
var highlight_component_2 = require("./chart/series-item/highlight.component");
exports.SeriesHighlightComponent = highlight_component_2.SeriesHighlightComponent;
var series_item_component_2 = require("./chart/series-item.component");
exports.SeriesItemComponent = series_item_component_2.SeriesItemComponent;
var labels_component_11 = require("./chart/series-item/labels.component");
exports.SeriesLabelsComponent = labels_component_11.SeriesLabelsComponent;
var labels_from_component_4 = require("./chart/series-item/labels.from.component");
exports.SeriesLabelsFromComponent = labels_from_component_4.SeriesLabelsFromComponent;
var labels_to_component_4 = require("./chart/series-item/labels.to.component");
exports.SeriesLabelsToComponent = labels_to_component_4.SeriesLabelsToComponent;
var markers_component_2 = require("./chart/series-item/markers.component");
exports.SeriesMarkersComponent = markers_component_2.SeriesMarkersComponent;
var notes_component_9 = require("./chart/series-item/notes.component");
exports.SeriesNotesComponent = notes_component_9.SeriesNotesComponent;
var notes_icon_component_9 = require("./chart/series-item/notes.icon.component");
exports.SeriesNotesIconComponent = notes_icon_component_9.SeriesNotesIconComponent;
var notes_label_component_9 = require("./chart/series-item/notes.label.component");
exports.SeriesNotesLabelComponent = notes_label_component_9.SeriesNotesLabelComponent;
var outliers_component_2 = require("./chart/series-item/outliers.component");
exports.SeriesOutliersComponent = outliers_component_2.SeriesOutliersComponent;
var tooltip_component_5 = require("./chart/series-item/tooltip.component");
exports.SeriesTooltipComponent = tooltip_component_5.SeriesTooltipComponent;
var title_component_11 = require("./chart/title.component");
exports.TitleComponent = title_component_11.TitleComponent;
var tooltip_component_6 = require("./chart/tooltip.component");
exports.TooltipComponent = tooltip_component_6.TooltipComponent;
var value_axis_component_2 = require("./chart/value-axis.component");
exports.ValueAxisComponent = value_axis_component_2.ValueAxisComponent;
var crosshair_component_8 = require("./chart/value-axis-item/crosshair.component");
exports.ValueAxisCrosshairComponent = crosshair_component_8.ValueAxisCrosshairComponent;
var crosshair_tooltip_component_8 = require("./chart/value-axis-item/crosshair.tooltip.component");
exports.ValueAxisCrosshairTooltipComponent = crosshair_tooltip_component_8.ValueAxisCrosshairTooltipComponent;
var value_axis_item_component_2 = require("./chart/value-axis-item.component");
exports.ValueAxisItemComponent = value_axis_item_component_2.ValueAxisItemComponent;
var labels_component_12 = require("./chart/value-axis-item/labels.component");
exports.ValueAxisLabelsComponent = labels_component_12.ValueAxisLabelsComponent;
var notes_component_10 = require("./chart/value-axis-item/notes.component");
exports.ValueAxisNotesComponent = notes_component_10.ValueAxisNotesComponent;
var notes_icon_component_10 = require("./chart/value-axis-item/notes.icon.component");
exports.ValueAxisNotesIconComponent = notes_icon_component_10.ValueAxisNotesIconComponent;
var notes_label_component_10 = require("./chart/value-axis-item/notes.label.component");
exports.ValueAxisNotesLabelComponent = notes_label_component_10.ValueAxisNotesLabelComponent;
var title_component_12 = require("./chart/value-axis-item/title.component");
exports.ValueAxisTitleComponent = title_component_12.ValueAxisTitleComponent;
var x_axis_component_2 = require("./chart/x-axis.component");
exports.XAxisComponent = x_axis_component_2.XAxisComponent;
var crosshair_component_9 = require("./chart/x-axis-item/crosshair.component");
exports.XAxisCrosshairComponent = crosshair_component_9.XAxisCrosshairComponent;
var crosshair_tooltip_component_9 = require("./chart/x-axis-item/crosshair.tooltip.component");
exports.XAxisCrosshairTooltipComponent = crosshair_tooltip_component_9.XAxisCrosshairTooltipComponent;
var x_axis_item_component_2 = require("./chart/x-axis-item.component");
exports.XAxisItemComponent = x_axis_item_component_2.XAxisItemComponent;
var labels_component_13 = require("./chart/x-axis-item/labels.component");
exports.XAxisLabelsComponent = labels_component_13.XAxisLabelsComponent;
var notes_component_11 = require("./chart/x-axis-item/notes.component");
exports.XAxisNotesComponent = notes_component_11.XAxisNotesComponent;
var notes_icon_component_11 = require("./chart/x-axis-item/notes.icon.component");
exports.XAxisNotesIconComponent = notes_icon_component_11.XAxisNotesIconComponent;
var notes_label_component_11 = require("./chart/x-axis-item/notes.label.component");
exports.XAxisNotesLabelComponent = notes_label_component_11.XAxisNotesLabelComponent;
var title_component_13 = require("./chart/x-axis-item/title.component");
exports.XAxisTitleComponent = title_component_13.XAxisTitleComponent;
var y_axis_component_2 = require("./chart/y-axis.component");
exports.YAxisComponent = y_axis_component_2.YAxisComponent;
var crosshair_component_10 = require("./chart/y-axis-item/crosshair.component");
exports.YAxisCrosshairComponent = crosshair_component_10.YAxisCrosshairComponent;
var crosshair_tooltip_component_10 = require("./chart/y-axis-item/crosshair.tooltip.component");
exports.YAxisCrosshairTooltipComponent = crosshair_tooltip_component_10.YAxisCrosshairTooltipComponent;
var y_axis_item_component_2 = require("./chart/y-axis-item.component");
exports.YAxisItemComponent = y_axis_item_component_2.YAxisItemComponent;
var labels_component_14 = require("./chart/y-axis-item/labels.component");
exports.YAxisLabelsComponent = labels_component_14.YAxisLabelsComponent;
var notes_component_12 = require("./chart/y-axis-item/notes.component");
exports.YAxisNotesComponent = notes_component_12.YAxisNotesComponent;
var notes_icon_component_12 = require("./chart/y-axis-item/notes.icon.component");
exports.YAxisNotesIconComponent = notes_icon_component_12.YAxisNotesIconComponent;
var notes_label_component_12 = require("./chart/y-axis-item/notes.label.component");
exports.YAxisNotesLabelComponent = notes_label_component_12.YAxisNotesLabelComponent;
var title_component_14 = require("./chart/y-axis-item/title.component");
exports.YAxisTitleComponent = title_component_14.YAxisTitleComponent;
var zoomable_component_2 = require("./chart/zoomable.component");
exports.ZoomableComponent = zoomable_component_2.ZoomableComponent;
/**
 * @hidden
 */
exports.CHART_DIRECTIVES_GENERATED = [
    axis_defaults_component_1.AxisDefaultsComponent,
    crosshair_component_1.AxisDefaultsCrosshairComponent,
    crosshair_tooltip_component_1.AxisDefaultsCrosshairTooltipComponent,
    labels_component_1.AxisDefaultsLabelsComponent,
    title_component_1.AxisDefaultsTitleComponent,
    category_axis_component_1.CategoryAxisComponent,
    crosshair_component_2.CategoryAxisCrosshairComponent,
    crosshair_tooltip_component_2.CategoryAxisCrosshairTooltipComponent,
    category_axis_item_component_1.CategoryAxisItemComponent,
    labels_component_2.CategoryAxisLabelsComponent,
    notes_component_1.CategoryAxisNotesComponent,
    notes_icon_component_1.CategoryAxisNotesIconComponent,
    notes_label_component_1.CategoryAxisNotesLabelComponent,
    select_component_1.CategoryAxisSelectComponent,
    title_component_2.CategoryAxisTitleComponent,
    chart_area_component_1.ChartAreaComponent,
    legend_component_1.LegendComponent,
    inactive_items_component_1.LegendInactiveItemsComponent,
    item_component_1.LegendItemComponent,
    pane_component_1.PaneComponent,
    panes_component_1.PanesComponent,
    title_component_3.PanesTitleComponent,
    plot_area_component_1.PlotAreaComponent,
    series_component_1.SeriesComponent,
    series_defaults_component_1.SeriesDefaultsComponent,
    labels_component_3.SeriesDefaultsLabelsComponent,
    labels_from_component_1.SeriesDefaultsLabelsFromComponent,
    labels_to_component_1.SeriesDefaultsLabelsToComponent,
    notes_component_2.SeriesDefaultsNotesComponent,
    notes_icon_component_2.SeriesDefaultsNotesIconComponent,
    notes_label_component_2.SeriesDefaultsNotesLabelComponent,
    tooltip_component_1.SeriesDefaultsTooltipComponent,
    error_bars_component_1.SeriesErrorBarsComponent,
    extremes_component_1.SeriesExtremesComponent,
    highlight_component_1.SeriesHighlightComponent,
    series_item_component_1.SeriesItemComponent,
    labels_component_4.SeriesLabelsComponent,
    labels_from_component_2.SeriesLabelsFromComponent,
    labels_to_component_2.SeriesLabelsToComponent,
    markers_component_1.SeriesMarkersComponent,
    notes_component_3.SeriesNotesComponent,
    notes_icon_component_3.SeriesNotesIconComponent,
    notes_label_component_3.SeriesNotesLabelComponent,
    outliers_component_1.SeriesOutliersComponent,
    tooltip_component_2.SeriesTooltipComponent,
    title_component_4.TitleComponent,
    tooltip_component_3.TooltipComponent,
    value_axis_component_1.ValueAxisComponent,
    crosshair_component_3.ValueAxisCrosshairComponent,
    crosshair_tooltip_component_3.ValueAxisCrosshairTooltipComponent,
    value_axis_item_component_1.ValueAxisItemComponent,
    labels_component_5.ValueAxisLabelsComponent,
    notes_component_4.ValueAxisNotesComponent,
    notes_icon_component_4.ValueAxisNotesIconComponent,
    notes_label_component_4.ValueAxisNotesLabelComponent,
    title_component_5.ValueAxisTitleComponent,
    x_axis_component_1.XAxisComponent,
    crosshair_component_4.XAxisCrosshairComponent,
    crosshair_tooltip_component_4.XAxisCrosshairTooltipComponent,
    x_axis_item_component_1.XAxisItemComponent,
    labels_component_6.XAxisLabelsComponent,
    notes_component_5.XAxisNotesComponent,
    notes_icon_component_5.XAxisNotesIconComponent,
    notes_label_component_5.XAxisNotesLabelComponent,
    title_component_6.XAxisTitleComponent,
    y_axis_component_1.YAxisComponent,
    crosshair_component_5.YAxisCrosshairComponent,
    crosshair_tooltip_component_5.YAxisCrosshairTooltipComponent,
    y_axis_item_component_1.YAxisItemComponent,
    labels_component_7.YAxisLabelsComponent,
    notes_component_6.YAxisNotesComponent,
    notes_icon_component_6.YAxisNotesIconComponent,
    notes_label_component_6.YAxisNotesLabelComponent,
    title_component_7.YAxisTitleComponent,
    zoomable_component_1.ZoomableComponent
];
