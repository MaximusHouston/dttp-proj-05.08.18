// <reference path="../typings/jquery/jquery.d.ts" />
var Overview;
(function (Overview) {
    var ProjectOverview = /** @class */ (function () {
        function ProjectOverview(search) {
            this.search = search;
            //populate widgets
            var alreadyPositionedWidgets = [], defaultWidgets = [], $content = $('#content'), me = this;
            //get list of all widget types from template HTML
            $('#tileTemplates li').each(function () {
                defaultWidgets.push({
                    templateId: $(this).attr('data-templateId'),
                    storedY2Label: null,
                    storedY2DataSetName: null
                });
            });
            for (var w = 0; w < ProjectOverview.MAXWIDGETS; w++) {
                var storedPosition = window.localStorage.getItem("dpo_overview_widget_position_" + w);
                if (typeof (storedPosition) === "string") {
                    var storedY2Label = window.localStorage.getItem("dpo_overview_widget_y2_label_" + w);
                    var storedY2DataSetName = window.localStorage.getItem("dpo_overview_widget_y2_datasetname_" + w);
                    alreadyPositionedWidgets.push({
                        templateId: storedPosition,
                        storedY2Label: storedY2Label,
                        storedY2DataSetName: storedY2DataSetName
                    });
                }
                else {
                    break;
                }
            }
            //if not used page before, just show one of each of the defaults
            var widgetsToPosition = (alreadyPositionedWidgets.length === 0) ? defaultWidgets : alreadyPositionedWidgets;
            for (var p = 0; p < widgetsToPosition.length; p++) {
                this.createWidget(widgetsToPosition[p]);
            }
            this.setAllLocalStorageKeys();
            //event listeners:
            $content.on('change', '.project-overview-inner #AvailableCharts', function () {
                var $template = $(this).parents('li');
                var $selectedEl = $(this).find(":selected");
                var settings = $(this).parents('li').find('.settings');
                if (settings.length)
                    (settings.is(':visible')) ? settings.slideUp() : settings.slideDown();
                var widgetClass = $template.data();
                var label = $selectedEl.text();
                var value = $selectedEl.val();
                widgetClass.setY2Label(label);
                widgetClass.setY2DataSetName(value);
                widgetClass.loadWidgetData();
                me.setAllLocalStorageKeys();
            });
            $content.on('click', '.project-overview-inner #btn-expand', function () {
                var displayDiv = $(this).parents('li .widget-body');
                if (displayDiv.hasClass("lightbox-full-screen")) {
                    displayDiv.removeClass("lightbox-full-screen");
                    $(this).find('img').attr("src", "/Images/full-screen-icon.png");
                }
                else {
                    displayDiv.addClass("lightbox-full-screen");
                    $(this).find('img').attr("src", "/Images/full-screen-exit-icon.png");
                }
                var templateId = $(this).parents("li").attr("data-templateId");
                var widgetClass = $(this).parents("li").data();
                widgetClass.refresh();
            });
            //toggle settings
            $content.on('click', '.project-overview-inner #btn-settings', function () {
                var settings = $(this).parents('li').find('.settings');
                if (settings.length)
                    (settings.is(':visible')) ? settings.slideUp() : settings.slideDown();
            });
            $content.on('click', '.project-overview-inner > li .settings .remove-btn', function () {
                var index = $('.project-overview-inner li').index($(this).parents('li'));
                $(this).parents('li').remove();
                me.setAllLocalStorageKeys();
            });
            //add new tile
            $content.on('click', '#addNewTile', function () {
                var $newWidget = $('#tileTemplates li[data-templateId="EmptyWidgetTemplate"]').clone();
                $newWidget.insertBefore('#addNewTile');
                if ($('.project-overview-inner li').length >= ProjectOverview.MAXWIDGETS)
                    $('#addNewTile').hide();
            });
            //settings change
            //change widget type
            $content.on('change', '.project-overview-inner .settings #tileType', function () {
                var templateId = $(this).val();
                var $currentWidget = $(this).parents('li');
                var $newWidget = me.createWidget({ templateId: templateId }, $currentWidget);
                $currentWidget.remove();
                me.setAllLocalStorageKeys();
            });
        }
        ProjectOverview.prototype.createWidget = function (storedData, insertBeforeElement) {
            var templateId = storedData.templateId;
            var $widget = $('#tileTemplates li[data-templateId="' + templateId + '"]').clone();
            var widgetTypeClass = $widget.attr("data-widgettype");
            var widgetRendererClass = $widget.attr("data-renderer");
            if (widgetTypeClass == null || widgetRendererClass == null) {
                return;
            }
            var widgetType = eval("new " + widgetTypeClass + "()");
            var widgetRenderer = eval("new " + widgetRendererClass + "()");
            widgetType.setSearch(this.search);
            widgetType.setTemplateId(templateId);
            widgetType.setRenderer(widgetRenderer);
            widgetType.setWidget($widget);
            if (storedData.storedY2Label) {
                if (widgetType.setY2Label) {
                    widgetType.setY2Label(storedData.storedY2Label);
                }
            }
            if (storedData.storedY2DataSetName) {
                if (widgetType.setY2Label) {
                    widgetType.setY2DataSetName(storedData.storedY2DataSetName);
                }
            }
            widgetType.loadWidgetData();
            $widget.data(widgetType);
            if (insertBeforeElement == null) {
                insertBeforeElement = $('#addNewTile');
            }
            $widget.insertBefore(insertBeforeElement);
        };
        ProjectOverview.prototype.setAllLocalStorageKeys = function () {
            var tiles = $('.project-overview-inner li');
            var i = 0;
            for (var x = 0; x < window.localStorage.length; x++) {
                var key = window.localStorage.key(x);
                if (key.substring(0, 29) === "dpo_overview_widget_position_") {
                    window.localStorage.removeItem(key);
                }
            }
            tiles.each(function () {
                var thisTemplateType = $(this).attr('data-templateId');
                if (thisTemplateType !== undefined) {
                    window.localStorage.setItem("dpo_overview_widget_position_" + i, thisTemplateType);
                    var widgetClass = $(this).data();
                    // HACK:  Use newer stuff
                    if (widgetClass.getY2Label) {
                        window.localStorage.setItem("dpo_overview_widget_y2_label_" + i, widgetClass.getY2Label());
                    }
                    if (widgetClass.getY2DataSetName) {
                        window.localStorage.setItem("dpo_overview_widget_y2_datasetname_" + i, widgetClass.getY2DataSetName());
                    }
                    i++;
                }
            });
            (tiles.length < ProjectOverview.MAXWIDGETS) ? $('#addNewTile').show() : $('#addNewTile').hide();
        };
        ProjectOverview.MAXWIDGETS = 13;
        return ProjectOverview;
    }());
    Overview.ProjectOverview = ProjectOverview;
})(Overview || (Overview = {}));
//# sourceMappingURL=ProjectOverview.js.map