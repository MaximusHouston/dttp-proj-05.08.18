/// <reference path="Base/BaseWidgetRenderer.ts" />
/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/d3/d3.d.ts" />
var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var Overview;
(function (Overview) {
    var BarAndLineWidgetRenderer = /** @class */ (function (_super) {
        __extends(BarAndLineWidgetRenderer, _super);
        function BarAndLineWidgetRenderer(data) {
            var _this = _super.call(this) || this;
            _this.xDataSetName = "ProjectCount";
            _this.y1DataSetName = "ProjectCount";
            _this.y2DataSetName = "ODUCount";
            _this.xLabel = null;
            _this.y1Label = "Project Count";
            _this.y2Label = "Outdoor Unit Count";
            _this.data = data;
            return _this;
        }
        BarAndLineWidgetRenderer.prototype.setXDataSetName = function (dataSetName) {
            this.xDataSetName = dataSetName;
        };
        BarAndLineWidgetRenderer.prototype.setY1DataSetName = function (dataSetName) {
            this.y1DataSetName = dataSetName;
        };
        BarAndLineWidgetRenderer.prototype.setY2DataSetName = function (dataSetName) {
            this.y2DataSetName = dataSetName;
        };
        BarAndLineWidgetRenderer.prototype.getY2DataSetName = function () {
            return this.y2DataSetName;
        };
        BarAndLineWidgetRenderer.prototype.setXLabel = function (label) {
            this.xLabel = label;
        };
        BarAndLineWidgetRenderer.prototype.setY1Label = function (label) {
            this.y1Label = label;
        };
        BarAndLineWidgetRenderer.prototype.setY2Label = function (label) {
            this.y2Label = label;
        };
        BarAndLineWidgetRenderer.prototype.getY2Label = function () {
            return this.y2Label;
        };
        BarAndLineWidgetRenderer.prototype.getDataSet = function (dataSetName) {
            return this.data.Data[dataSetName];
        };
        BarAndLineWidgetRenderer.prototype.getDimensionsWithMargins = function ($widget) {
            var wh = this.getWidgetDimensions($widget);
            var margins = this.getMargins();
            return {
                width: wh.width - margins.left - margins.right,
                height: wh.height - margins.top - margins.bottom
            };
        };
        BarAndLineWidgetRenderer.prototype.getXScale = function (domain) {
            return d3.scale.ordinal()
                .rangeRoundBands([0, this.dimensions.width], .1)
                .domain(domain);
        };
        BarAndLineWidgetRenderer.prototype.getYScale = function (domain) {
            return d3.scale.linear()
                .range([this.dimensions.height, 0])
                .domain(domain);
        };
        BarAndLineWidgetRenderer.prototype.getXDomain = function (dataSetName) {
            if (this.data.Data[dataSetName] != null) {
                return this.data.Data[dataSetName].map(function (d) { return d.Key; });
            }
            return new Array();
        };
        BarAndLineWidgetRenderer.prototype.getYDomain = function (dataSetName) {
            var dataSet = this.data.Data[dataSetName];
            if (dataSet != null) {
                return [0, d3.max(dataSet, function (d) { return d.Value; })];
            }
            return Array();
        };
        BarAndLineWidgetRenderer.prototype.getXAxis = function (x) {
            return d3.svg.axis()
                .scale(x)
                .orient("bottom")
                .tickValues(this.getXDomain(this.xDataSetName))
                .tickFormat(function (t) { return t[0]; });
        };
        BarAndLineWidgetRenderer.prototype.getY1Axis = function (scale, orientation) {
            var axis = d3.svg.axis()
                .scale(scale)
                .orient(orientation)
                .tickSubdivide(0)
                .tickFormat(d3.format(",.0f"));
            return axis;
        };
        BarAndLineWidgetRenderer.prototype.getY2Axis = function (scale, orientation) {
            var axis = d3.svg.axis()
                .scale(scale)
                .orient(orientation)
                .tickSubdivide(0);
            // HACK:  This could be done better
            if (this.y2DataSetName.toLowerCase() == "totalnetvalue") {
                axis.tickFormat(function (d) {
                    var divValue = d / 1000;
                    var sep = "";
                    if (divValue >= 1) {
                        sep = "K";
                        d = divValue;
                        divValue = divValue / 1000;
                        if (divValue >= 1) {
                            sep = "M";
                            d = divValue;
                        }
                    }
                    return d3.format(",.0f")(d) + sep;
                });
            }
            else {
                axis.tickFormat(d3.format(",.0f"));
            }
            return axis;
        };
        BarAndLineWidgetRenderer.prototype.getMargins = function () {
            return { top: 20, right: 40, bottom: 70, left: 40 };
        };
        BarAndLineWidgetRenderer.prototype.getWidgetDimensions = function ($widgetBox) {
            var $widget = $widgetBox;
            if ($widgetBox.is(':visible')) {
                return {
                    width: $widgetBox.width(),
                    height: $widgetBox.height()
                };
            }
            var $widget = $widgetBox.parents('li');
            var previousCss = $widget.attr("style");
            $widget
                .css({
                position: 'absolute',
                visibility: 'hidden',
                display: 'block'
            });
            var wh = {
                width: $widget.width(),
                height: $widget.height()
            };
            $widget.attr("style", previousCss ? previousCss : "");
            return wh;
        };
        BarAndLineWidgetRenderer.prototype.renderInitialSvgSelection = function ($displayDiv) {
            return d3.selectAll($displayDiv.toArray()).append("svg")
                .attr("width", this.dimensions.width + this.margin.left + this.margin.right)
                .attr("height", this.dimensions.height + this.margin.top + this.margin.bottom)
                .append("g")
                .attr("transform", "translate(" + this.margin.left + "," + this.margin.top + ")");
        };
        BarAndLineWidgetRenderer.prototype.renderXAxis = function () {
            this.svg.append("g")
                .attr("class", "x axis")
                .attr("transform", "translate(0," + this.dimensions.height + ")")
                .call(this.xAxis);
        };
        BarAndLineWidgetRenderer.prototype.renderY1Axis = function () {
            this.svg.append("g")
                .attr("class", "y y1 axis")
                .call(this.y1Axis)
                .append("text")
                .attr("class", "y1-axis-label")
                .attr("y", -this.margin.top + 2)
                .attr("dy", ".71em")
                .style("text-anchor", "start")
                .text(this.y1Label);
        };
        BarAndLineWidgetRenderer.prototype.renderY2Axis = function () {
            this.svg.append("g")
                .attr("class", "y y2 axis")
                .attr("transform", "translate(" + this.dimensions.width + " ,0)")
                .call(this.y2Axis)
                .append("text")
                .attr("class", "y2-axis-label")
                .attr("y", -this.margin.top + 2)
                .attr("dy", ".71em")
                .style("text-anchor", "end")
                .text(this.y2Label);
        };
        BarAndLineWidgetRenderer.prototype.renderBars = function (dataSetName) {
            var _this = this;
            var bars = this.svg.selectAll(".bar")
                .data(this.getDataSet(dataSetName));
            var me = this;
            bars
                .enter().append("rect")
                .attr("class", "bar")
                .attr("width", this.xScale.rangeBand())
                .attr("x", function (d) { return _this.xScale(d.Key); })
                .attr("y", this.dimensions.height)
                .attr("height", 0)
                .transition()
                .delay(function (d, i) { return i * 100; })
                .attr("y", function (d) {
                var val = _this.y1Scale(d.Value);
                return val > 0 ? val : 0;
            })
                .attr("height", function (d) {
                var val = _this.dimensions.height - _this.y1Scale(d.Value);
                return val > 0 ? val : 0;
            });
            bars
                .on("mouseover", function (d) {
                me.tooltip.transition()
                    .duration(200)
                    .style("opacity", .9);
                me.tooltip.html(d3.format(",.0f")(d.Value))
                    .style("left", (d3.event.pageX + 5) + "px")
                    .style("top", (d3.event.pageY - 15) + "px");
            })
                .on("mouseout", function (d) {
                me.tooltip.transition()
                    .duration(500)
                    .style("opacity", 0);
            });
            bars
                .exit().remove();
        };
        BarAndLineWidgetRenderer.prototype.renderLine = function (dataSetName) {
            var _this = this;
            var countLine = d3.svg.line()
                .x(function (d) { return _this.xScale(d.Key) + _this.xScale.rangeBand() / 2; }) // Half range band to go to middle of bar
                .y(function (d) { return _this.y2Scale(d.Value); })
                .interpolate("monotone");
            var dataSet = this.getDataSet(dataSetName);
            var path = this.svg.selectAll(".line-count")
                .data(dataSet);
            path
                .enter().append("path")
                .attr("class", "line-count")
                .attr("d", countLine(dataSet));
            var totalLength = path.node().getTotalLength();
            // quick fix to animate line
            path
                .attr("stroke-dasharray", totalLength + " " + totalLength)
                .attr("stroke-dashoffset", totalLength)
                .transition()
                .duration(800)
                .ease("linear")
                .attr("stroke-dashoffset", 0);
            path
                .exit().remove();
            this.renderMarkers(dataSetName);
        };
        BarAndLineWidgetRenderer.prototype.renderMarkers = function (dataSetName) {
            var _this = this;
            var dataSet = this.getDataSet(dataSetName);
            var me = this;
            var marker = this.svg.selectAll('.line-count-marker')
                .data(dataSet);
            marker
                .enter().append("circle")
                .attr("class", "line-count-marker")
                .attr("r", 3)
                .attr("cx", function (d) { return _this.xScale(d.Key) + _this.xScale.rangeBand() / 2; })
                .attr("cy", function (d) { return _this.y2Scale(d.Value); });
            marker
                .on("mouseover", function (d) {
                me.tooltip.transition()
                    .duration(200)
                    .style("opacity", .9);
                me.tooltip.html(d3.format(",.0f")(d.Value))
                    .style("left", (d3.event.pageX + 5) + "px")
                    .style("top", (d3.event.pageY - 15) + "px");
            })
                .on("mouseout", function (d) {
                me.tooltip.transition()
                    .duration(500)
                    .style("opacity", 0);
            });
            marker.
                exit().remove();
        };
        BarAndLineWidgetRenderer.prototype.renderTooltip = function () {
            return d3.select("body").append("div")
                .attr("class", "chart-tooltip")
                .style("opacity", 0);
        };
        BarAndLineWidgetRenderer.prototype.renderWidget = function ($widget, $displayDiv, $widgetBox) {
            $displayDiv.html("");
            this.margin = this.getMargins();
            this.dimensions = this.getDimensionsWithMargins($widgetBox);
            this.xScale = this.getXScale(this.getXDomain(this.xDataSetName));
            this.y1Scale = this.getYScale(this.getYDomain(this.y1DataSetName));
            this.y2Scale = this.getYScale(this.getYDomain(this.y2DataSetName));
            this.xAxis = this.getXAxis(this.xScale);
            this.y1Axis = this.getY1Axis(this.y1Scale, "left");
            this.y2Axis = this.getY2Axis(this.y2Scale, "right");
            this.svg = this.renderInitialSvgSelection($displayDiv);
            this.renderXAxis();
            this.renderY1Axis();
            this.renderY2Axis();
            this.tooltip = this.renderTooltip();
            this.renderBars(this.y1DataSetName);
            this.renderLine(this.y2DataSetName);
        };
        BarAndLineWidgetRenderer.prototype.render = function ($widgets, data) {
            _super.prototype.render.call(this, $widgets, data);
            if (_super.prototype.checkData.call(this, data)) {
                $(this).find('.no-data').show();
                return;
            }
            this.data = data;
            var me = this;
            $widgets.each(function () {
                var $widget = $(this), $displayDiv = $(this).find('.chart-content'), $widgetBox = $widget.children('div');
                me.renderWidget($widget, $displayDiv, $widgetBox);
            });
        };
        return BarAndLineWidgetRenderer;
    }(Overview.BaseWidgetRenderer));
    Overview.BarAndLineWidgetRenderer = BarAndLineWidgetRenderer;
})(Overview || (Overview = {}));
//# sourceMappingURL=BarAndLineWidgetRenderer.js.map