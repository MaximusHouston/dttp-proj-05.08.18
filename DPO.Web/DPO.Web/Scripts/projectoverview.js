///// <reference path="/Scripts/d3/d3.js" />

//// +1 to allow for 'add new tile', tile.
//var maxWidgets = 13;

//window.overviewWidgets = {
//    widgetSearch: {},
//    widgetData: {},

//    //getWidgetWidthAndHeight: function ($widgetBox) {

//    //    var $widget = $widgetBox
//    //    if ($widgetBox.is(':visible')) {
//    //        return {
//    //            width: $widgetBox.width(),
//    //            height: $widgetBox.height()
//    //        };
//    //    }

//    //    var $widget = $widgetBox.parents('li');

//    //    var previousCss = $widget.attr("style");

//    //    $widget
//    //        .css({
//    //            position: 'absolute', // Optional if #myDiv is already absolute
//    //            visibility: 'hidden',
//    //            display: 'block'
//    //        });

//    //    var wh = {
//    //        width: $widget.width(),
//    //        height: $widget.height()
//    //    }

//    //    $widget.attr("style", previousCss ? previousCss : "");

//    //    return wh;
//    //},

//    //method to save position of widgets
//    //setAllLocalStorageKeys: function () {
//    //    var tiles = $('.project-overview-inner li');
//    //    var i = 0;

//    //    for (key in window.localStorage) {
//    //        if (key.substring(0, 29) === "dpo_overview_widget_position_") {
//    //            window.localStorage.removeItem(key);
//    //        }
//    //    }
//    //    tiles.each(function () {
//    //        var thisTemplateType = $(this).attr('data-templateId');
//    //        if (thisTemplateType !== undefined) {
//    //            window.localStorage.setItem("dpo_overview_widget_position_" + i, thisTemplateType);
//    //            i++;
//    //        }
//    //    });

//    //    (tiles.length < maxWidgets) ? $('#addNewTile').show() : $('#addNewTile').hide();
//    //},

//    //buildInfoText: function ($widgets, data) {
//    //    var num = (data && data.Data && data.Data[0] && data.Data[0].Value) ? data.Data[0].Value : 0;

//    //    $widgets.each(function () {
//    //        $(this).find('.data').text(num);
//    //    });
//    //},

//    //buildInfoList: function ($widgets, data) {
//    //    if (!data || !data.Data || !data.Data.length) {
//    //        $widgets.each(function () {
//    //            $(this).find('.no-data').show();
//    //        });
//    //    }
//    //    else {
//    //        var alerts = [];
//    //        for (var i = 0; i < data.Data.length; i++) {
//    //            var alertStr = "<p>" + data.Data[i].Key + "</p>";
//    //            alerts.push(alertStr);
//    //        }

//    //        var allAlertsStr = alerts.join('');

//    //        $widgets.each(function () {

//    //            var $widget = $(this);
//    //            $widget.find('.contentbox').append($(allAlertsStr));
//    //            $widget.find('.js-count').text('- ' + alerts.length);
//    //        });
//    //    }
//    //},

//    //buildBarChart: function ($widgets, data) {
//    //    if (!data || !data.Data || !data.Data.length) {
//    //        $widgets.each(function () {
//    //            $(this).find('.no-data').show();
//    //        });
//    //    }
//    //    else {
//    //        var bars = [];

//    //        for (var i = 0; i < data.Data.length; i++) {
//    //            var barStr = '<p>' + data.Data[i].Key + '</p>' +
//    //            '<strong class="pull-left" style="margin-left: 86%;">' + data.Data[i].Value.toFixed(2) + '%</strong>' +
//    //            '<div style="width: 83%;"><div>&nbsp;</div></div>';

//    //            bars.push(barStr);
//    //        }

//    //        var allBarsStr = bars.join('');

//    //        $widgets.each(function () {
//    //            $(this).find('.contentbox').append($(allBarsStr));
//    //        });
//    //    }

//    //    setTimeout(function () {
//    //        $widgets.each(function () {
//    //            $(this).find('div > div > div').each(function () {
//    //                var $bar = $(this);
//    //                $bar.css('width', $bar.parent().prev('strong').text());
//    //            });
//    //        });

//    //    }, 250);
//    //},

//    //buildD3Bar: function ($widgets, data, tooltipText) {
//    //    if (!data || !data.Data) {
//    //        $(this).find('.no-data').show();

//    //        return;
//    //    }

//    //    var me = this;

//    //    $widgets.each(function () {
//    //        var $widget = $(this),
//    //            $displayDiv = $(this).find('.chart-content'),
//    //            $widgetBox = $widget.children('div');

//    //        $displayDiv.html("");
//    //        wh = me.getWidgetWidthAndHeight($widgetBox);

//    //        var margin = { top: 20, right: 40, bottom: 70, left: 40 },
//    //            width = wh.width
//    //                - margin.left - margin.right,
//    //            height = wh.height
//    //                - margin.top - margin.bottom,
//    //            projectCountData = data.Data["ProjectCount"],
//    //            oduCountData = data.Data["ODUCount"],
//    //            xDomain = projectCountData.map(function (d) { return d.Key; }),
//    //            y1Domain = [0, d3.max(projectCountData, function (d) { return d.Value; })],
//    //            y2Domain = [0, d3.max(oduCountData, function (d) { return d.Value })]

//    //        var x = d3.scale.ordinal()
//    //            .rangeRoundBands([0, width], .1);

//    //        var x2 = d3.scale.ordinal()
//    //          .rangeRoundBands([0, width], .1);

//    //        var y1 = d3.scale.linear()
//    //            .range([height, 0]);

//    //        var y2 = d3.scale.linear()
//    //            .range([height, 0]);

//    //        var xAxis = d3.svg.axis()
//    //            .scale(x)
//    //            .orient("bottom")
//    //            .tickValues(xDomain)
//    //            .tickFormat(function (d, i) {
//    //                return d[0];
//    //            });

//    //        var x2Axis = d3.svg.axis()
//    //                        .scale(x2)
//    //                        .orient("bottom")
//    //                        .tickValues(xDomain)
//    //                        .tickFormat(function (d, i) {
//    //                            return d[0];
//    //                        });

//    //        var y1Axis = d3.svg.axis()
//    //            .scale(y1)
//    //            .orient("left")
//    //            .tickFormat(d3.format("d"))
//    //            .tickSubdivide(0);

//    //        var y2Axis = d3.svg.axis()
//    //            .scale(y2)
//    //            .orient("right")
//    //            .tickFormat(d3.format("d"))
//    //            .tickSubdivide(0);

//    //        var svg = d3.selectAll($displayDiv.toArray()).append("svg")
//    //            .attr("width", width + margin.left + margin.right)
//    //            .attr("height", height + margin.top + margin.bottom)
//    //          .append("g")
//    //            .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

//    //        x.domain(xDomain);
//    //        x2.domain(xDomain);
//    //        y1.domain(y1Domain);
//    //        y2.domain(y2Domain);

//    //        svg.append("g")
//    //            .attr("class", "x axis")
//    //            .attr("transform", "translate(0," + height + ")")
//    //            .call(xAxis);

//    //        svg.append("g")
//    //            .attr("class", "y y1 axis")
//    //            .call(y1Axis)
//    //                .append("text")
//    //                  //.attr("transform", "rotate(-90)")
//    //                  .attr("y", -margin.top + 2)
//    //                  .attr("dy", ".71em")
//    //                .style("text-anchor", "end")
//    //                .text("Count");

//    //        svg.append("g")
//    //          .attr("class", "y y2 axis")
//    //          .attr("transform", "translate(" + width + " ,0)")
//    //          .call(y2Axis)
//    //              .append("text")
//    //                //.attr("transform", "rotate(-90)")
//    //                .attr("y", -margin.top + 2)
//    //                .attr("dy", ".71em")
//    //              .style("text-anchor", "end")
//    //              .text("ODU Count");


//    //        // y1 bar - Project Count
//    //        var bars = svg.selectAll(".bar")
//    //          .data(projectCountData);

//    //        bars
//    //          .enter().append("rect")
//    //            .attr("class", "bar")
//    //            .attr("width", x.rangeBand())
//    //            .attr("x", function (d) { return x(d.Key); })
//    //            .attr("y", height)
//    //            .attr("height", 0)
//    //            .transition()
//    //                .delay(function (d, i) { return i * 100 })
//    //                .attr("y", function (d) {
//    //                    var val = y1(d.Value);

//    //                    return val > 0 ? val : 0;
//    //                })
//    //                .attr("height", function (d) {
//    //                    var val = height - y1(d.Value);

//    //                    return val > 0 ? val : 0;
//    //                });
//    //        bars
//    //            .on("mouseover", function (d) {
//    //                tooltip.transition()
//    //                    .duration(200)
//    //                    .style("opacity", .9);
//    //                tooltip.html(d.Value)
//    //                    .style("left", (d3.event.pageX) + "px")
//    //                    .style("top", (d3.event.pageY - 25) + "px");
//    //            })
//    //            .on("mouseout", function (d) {
//    //                tooltip.transition()
//    //                        .duration(500)
//    //                        .style("opacity", 0);
//    //            });

//    //        bars
//    //            .exit().remove()

//    //        // y1 bar labels
//    //        //svg.selectAll(".bar-text")
//    //        //   .data(projectCountData)
//    //        //   .enter()
//    //        //   .append("text")
//    //        //   .attr("x", function (d) { return x(d.Key) })
//    //        //   .attr("y", function (d) { return height - y1(d.Value); })
//    //        //   .attr("dx", x.rangeBand() / 2)
//    //        //   .attr("dy", "1.2em")
//    //        //   .attr("text-anchor", "middle")
//    //        //   .text(function (d) { return d.Value; })
//    //        //   .attr("fill", "white");

//    //        // y2 lines
//    //        var countLine = d3.svg.line()
//    //                .x(function (d) { return x2(d.Key) + x.rangeBand() / 2; }) // Half range band to go to middle of bar
//    //                .y(function (d) { return y2(d.Value); })
//    //                .interpolate("monotone");

//    //        var path = svg.selectAll(".line-count")
//    //            .data(oduCountData);

//    //        path
//    //            .enter().append("path")
//    //                .attr("class", "line-count")
//    //                .attr("d", countLine(oduCountData));

//    //        var totalLength = path.node().getTotalLength();

//    //        // quick fix to animate line
//    //        path
//    //            .attr("stroke-dasharray", totalLength + " " + totalLength)
//    //            .attr("stroke-dashoffset", totalLength)
//    //            .transition()
//    //                .duration(800)
//    //                .ease("linear")
//    //                .attr("stroke-dashoffset", 0);

//    //        path
//    //         .exit().remove();


//    //        // bubbles & tooltips
//    //        var tooltip = d3.select("body").append("div")
//    //            .attr("class", "chart-tooltip")
//    //            .style("opacity", 0);

//    //        var marker = svg.selectAll('.line-count-marker')
//    //            .data(oduCountData);

//    //        marker
//    //            .enter().append("circle")
//    //                .attr("class", "line-count-marker")
//    //                .attr("r", 3)
//    //                .attr("cx", function (d) { return x2(d.Key) + x.rangeBand() / 2 })
//    //                .attr("cy", function (d) { return y2(d.Value) });

//    //        marker
//    //            .on("mouseover", function (d) {
//    //                tooltip.transition()
//    //                     .duration(200)
//    //                     .style("opacity", .9);
//    //                tooltip.html(d.Value)
//    //                     .style("left", (d3.event.pageX + 5) + "px")
//    //                     .style("top", (d3.event.pageY - 15) + "px");
//    //            })
//    //             .on("mouseout", function (d) {
//    //                 tooltip.transition()
//    //                      .duration(500)
//    //                      .style("opacity", 0);
//    //             });

//    //        marker.
//    //            exit().remove();
//    //        // y1 labels
//    //        //svg.append('g')
//    //        //    .classed('labels-group', true)
//    //        //    .selectAll('text')
//    //        //    .data(projectCountData)
//    //        //    .enter()
//    //        //    .append('text')
//    //        //    .classed('label', true)
//    //        //    .attr({
//    //        //        'x': function (d, i) {
//    //        //            return x(d.Key);
//    //        //        },
//    //        //        'y': function (d, i) {
//    //        //            return y1(d.Value);
//    //        //        }
//    //        //    })
//    //        //    .attr("text-anchor", "middle")
//    //        //    .text(function (d, i) {
//    //        //        return d.Value;
//    //        //    });
//    //    });
//    //},

//    //buildLineChart: function ($widgets, data, tooltipText) {
//    //    if (!data || !data.Data || !data.Data.length) {
//    //        $widgets.each(function () {
//    //            $(this).find('.no-data').show();
//    //        });
//    //    }
//    //    else {
//    //        //populate months axis
//    //        //and bars data
//    //        var months = [];
//    //        var columns = [];
//    //        for (var i = 0; i < data.Data.length; i++) {
//    //            var monthStr = '<div class="column"><strong>' + data.Data[i].Key + '</strong></div>';
//    //            months.push(monthStr);

//    //            var columnStr = '<div class="column"><div class="fill"><div class="marker">&nbsp;</div><div class="tooltip">' +
//    //                            '<span>' + data.Data[i].Value + '</span> ' + tooltipText + '<img src="/Images/tooltip-tail-vertical.png" />' +
//    //                            '</div></div></div>';
//    //            columns.push(columnStr);
//    //        }

//    //        var allMonthsStr = months.join('');
//    //        var allColumnsStr = columns.join('');

//    //        $widgets.each(function () {
//    //            var $widget = $(this);
//    //            $widget.find('.months').append($(allMonthsStr));
//    //            $widget.find('.bars').append($(allColumnsStr));
//    //        });
//    //    }

//    //    setTimeout(function () {
//    //        $widgets.each(function () {
//    //            var maxfillvalue = 0;
//    //            var filltextvalues = [];
//    //            var $content = $(this).find('.contentbox');

//    //            $content.find('.tooltip span').each(function () {
//    //                var textasnum = Number($(this).text());

//    //                filltextvalues.push(textasnum);
//    //                if (textasnum > maxfillvalue) maxfillvalue = textasnum;
//    //            });

//    //            $content.find('.fill').each(function (index) {
//    //                var fillvalue = 100 - ((filltextvalues[index] / maxfillvalue) * 100);
//    //                $(this).css('height', fillvalue + '%');
//    //            });

//    //        });

//    //    }, 250);
//    //},

//    //getWidgetData: function (templateIds) {
//    //    var templateId = templateIds.shift();
//    //    var me = this;

//    //    $.ajax({
//    //        type: "POST",
//    //        url: '/Overview/OverviewTemplateData',
//    //        dataType: 'json',
//    //        contentType: 'application/json',
//    //        cache: false,
//    //        data: JSON.stringify({
//    //            widget: {
//    //                templateId: templateId
//    //            },
//    //            container: me.widgetSearch
//    //        }),
//    //        success: function (result) {
//    //            me.widgetData[templateId] = result;
//    //            me.renderWidgets(templateId, result);

//    //            if (templateIds.length > 0) {
//    //                me.getWidgetData(templateIds);
//    //            }
//    //        },
//    //        error: function (XMLHttpRequest, textStatus, errorThrown) {
//    //            me.renderWidgets(templateId);

//    //            if (templateIds.length > 0) {
//    //                me.getWidgetData(templateIds);
//    //            }
//    //        }
//    //    });
//    //},

//    //renderWidgets: function (templateId, data) {
//    //    var $allWidgets = $('li[data-templateId="' + templateId + '"]');

//    //    if (typeof data === 'undefined' || data === null) {
//    //        $allWidgets.each(function () {
//    //            var $widget = $(this);

//    //            $widget.find('.contentbox, .contentbox-footer').hide();
//    //            $widget.find('.contentbox-error').show();
//    //            $widget.removeClass('loading');
//    //        });
//    //        return;
//    //    }

//    //    //add data to template and live widgets
//    //    switch (templateId) {
//    //        case "OpenProjectTypesTemplate":
//    //        case "VerticalMarketsTemplate":
//    //            this.buildBarChart($allWidgets, data);
//    //            break;
//    //        case "WonProjectsTemplate":
//    //            this.buildD3Bar($allWidgets, data, 'Projects Won');
//    //            break;
//    //        case "LostProjectsTemplate":
//    //            this.buildD3Bar($allWidgets, data, 'Projects Lost');
//    //            //this.buildLineChart($allWidgets, data, 'Projects Lost');
//    //            break;
//    //        case "NewProjectsTemplate":
//    //            this.buildD3Bar($allWidgets, data, 'Projects Lost');
//    //            //this.buildLineChart($allWidgets, data, 'New Projects');
//    //            break;
//    //        case "ProjectAlertsTemplate":
//    //            this.buildInfoList($allWidgets, data);
//    //            break;
//    //        default:
//    //            this.buildInfoText($allWidgets, data);
//    //            break;
//    //    }

//    //    $allWidgets.removeClass('loading');
//    //},

//    //createWidget: function (templateId) {
//    //    var $widget = $('#tileTemplates li[data-templateId="' + templateId + '"]').clone();

//    //    var widgetTypeClass = $widget.attr("data-widgettype");
//    //    var widgetRendererClass = $widget.attr("data-renderer");

//    //    if (widgetTypeClass == null || widgetRendererClass == null)
//    //    {
//    //        return;
//    //    }

//    //    var widgetType = eval("new " + widgetTypeClass + "()");
//    //    var widgetRenderer = eval("new " + widgetRendererClass + "()");

//    //    widgetType.setSearch(window.widgetSearch);
//    //    widgetType.setTemplateId(templateId);
//    //    widgetType.setRenderer(widgetRenderer);
//    //    widgetType.setWidget($widget);
//    //    widgetType.loadWidgetData();
//    //    $widget.data(widgetType);

//    //    $widget.insertBefore($('#addNewTile'));
//    //},

//    init: function (widgetSearch) {
//        this.widgetSearch = widgetSearch;

//        ////populate widgets
//        //var alreadyPositionedWidgets = [],
//        //    defaultWidgets = [],
//        //    $content = $('#content'),
//        //    me = this;

//        ////get list of all widget types from template HTML
//        //$('#tileTemplates li').each(function () {
//        //    if ($(this).attr("data-templateid").indexOf("New") < 0)
//        //    {
//        //        defaultWidgets.push($(this).attr('data-templateId'));
//        //    }
//        //});

//        //for (var w = 0; w < maxWidgets; w++) {
//        //    var storedPosition = window.localStorage.getItem("dpo_overview_widget_position_" + w);
//        //    if (typeof (storedPosition) === "string") alreadyPositionedWidgets.push(storedPosition);
//        //    else break;
//        //}

//        ////if not used page before, just show one of each of the defaults
//        //var widgetsToPosition = (alreadyPositionedWidgets.length === 0) ? defaultWidgets : alreadyPositionedWidgets;

//        //for (var p = 0; p < widgetsToPosition.length; p++) {
//        //    this.createWidget(widgetsToPosition[p]);
//        //}

//        ////get data for all types (at present)
//        //this.getWidgetData(defaultWidgets);
//        //this.setAllLocalStorageKeys();

//        ////event listeners:

//        ////toggle tooltips
//        ////$content.on('click', '.marker', function () {
//        ////    var tooltip = $(this).next();
//        ////    $('.tooltip').not(tooltip).hide();
//        ////    (tooltip.is(':visible')) ? tooltip.hide() : tooltip.fadeIn(200);
//        ////});

//        ////$content.on('click', function (e) {
//        ////    if ($('.tooltip').is(':visible') && !$(e.target).hasClass('marker')) $('.tooltip').fadeOut(200);
//        ////});

//        //$content.on('click', '.project-overview-inner #btn-expand', function () {
//        //    var displayDiv = $(this).parents('li > div');

//        //    if (displayDiv.hasClass("lightbox-full-screen")) {
//        //        displayDiv.removeClass("lightbox-full-screen");
//        //        $(this).find('img').attr("src", "/Images/full-screen-icon.png");
//        //    } else {
//        //        displayDiv.addClass("lightbox-full-screen");
//        //        $(this).find('img').attr("src", "/Images/full-screen-exit-icon.png");
//        //    }

//        //    var templateId = $(this).parents("li").attr("data-templateId");
//        //    me.renderWidgets(templateId, me.widgetData[templateId]);
//        //});

//        ////toggle settings
//        //$content.on('click', '.project-overview-inner #btn-settings', function () {
//        //    var settings = $(this).parents('li').find('.settings');
//        //    if (settings.length) (settings.is(':visible')) ? settings.slideUp() : settings.slideDown();
//        //});

//        //$content.on('click', '.project-overview-inner > li .settings .remove-btn', function () {
//        //    var index = $('.project-overview-inner li').index($(this).parents('li'));
//        //    $(this).parents('li').remove();
//        //    me.setAllLocalStorageKeys();
//        //});

//        ////add new tile
//        //$content.on('click', '#addNewTile', function () {
//        //    var $newWidget = $('#tileTemplates li[data-templateId="EmptyWidgetTemplate"]').clone();
//        //    $newWidget.insertBefore('#addNewTile');
//        //    if ($('.project-overview-inner li').length >= maxWidgets) $('#addNewTile').hide();
//        //});

//        ////settings change

//        ////change widget type
//        //$content.on('change', '.project-overview-inner .settings #tileType', function () {
//        //    var templateId = $(this).val();
//        //    var $currentWidget = $(this).parents('li');

//        //    var $newWidget = $('#tileTemplates li[data-templateId="' + templateId + '"]').clone();
//        //    $newWidget.insertBefore($currentWidget);
//        //    $currentWidget.remove();
//        //    me.setAllLocalStorageKeys();
//        //});
//    }

//}
