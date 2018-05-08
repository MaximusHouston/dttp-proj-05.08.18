/// <reference path="Base/BaseWidgetRenderer.ts" />
/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/d3/d3.d.ts" />

module Overview {
    export class BarAndLineWidgetRenderer extends BaseWidgetRenderer implements IGraphWidgetRenderer {
        private data: GraphData;
        private xDataSetName: string = "ProjectCount";
        private y1DataSetName: string = "ProjectCount";
        private y2DataSetName: string = "ODUCount";
        private xLabel: string = null;
        private y1Label: string = "Project Count";
        private y2Label: string = "Outdoor Unit Count";
        private dimensions: IDimensions;
        private margin: IMargins;
        private svg: d3.Selection<any>;
        private xAxis: d3.svg.Axis;
        private y1Axis: d3.svg.Axis;
        private y2Axis: d3.svg.Axis;
        private xScale: d3.scale.Ordinal<string, number>;
        private y1Scale: d3.scale.Linear<number, number>;
        private y2Scale: d3.scale.Linear<number, number>;
        private tooltip: d3.Selection<any>;

        constructor(data: GraphData) {
            super();
            this.data = data;
        }

        public setXDataSetName(dataSetName: string) {
            this.xDataSetName = dataSetName;
        }

        public setY1DataSetName(dataSetName: string) {
            this.y1DataSetName = dataSetName;
        }

        public setY2DataSetName(dataSetName: string) {
            this.y2DataSetName = dataSetName;
        }
        public getY2DataSetName(): string {
            return this.y2DataSetName;
        }

        public setXLabel(label: string) {
            this.xLabel = label;
        }

        public setY1Label(label: string) {
            this.y1Label = label;
        }

        public setY2Label(label: string) {
            this.y2Label = label;
        }
        public getY2Label(): string {
            return this.y2Label;
        }

        private getDataSet(dataSetName: string): Array<WidgetDataElement> {
            return this.data.Data[dataSetName];
        }

        private getDimensionsWithMargins($widget: JQuery): IDimensions {
            var wh: IDimensions = this.getWidgetDimensions($widget);
            var margins: IMargins = this.getMargins();

            return {
                width: wh.width - margins.left - margins.right,
                height: wh.height - margins.top - margins.bottom
            }
        }

        private getXScale(domain: Array<string>): d3.scale.Ordinal<string, number> {
            return d3.scale.ordinal()
                .rangeRoundBands([0, this.dimensions.width], .1)
                .domain(domain);
        }

        private getYScale(domain: Array<number>): d3.scale.Linear<number, number> {
            return d3.scale.linear()
                .range([this.dimensions.height, 0])
                .domain(domain);
        }

        private getXDomain(dataSetName: string): Array<string> {
            if (this.data.Data[dataSetName] != null) {
                return this.data.Data[dataSetName].map((d) => d.Key);
            }

            return new Array<string>();
        }

        private getYDomain(dataSetName: string): Array<number> {
            var dataSet = this.data.Data[dataSetName];

            if (dataSet != null) {
                return [0, d3.max(dataSet, (d) => d.Value)];
            }

            return Array<number>();
        }

        private getXAxis(x: d3.scale.Ordinal<string, number>): d3.svg.Axis {
            return d3.svg.axis()
                .scale(x)
                .orient("bottom")
                .tickValues(this.getXDomain(this.xDataSetName))
                .tickFormat((t: any): string => { return (<string>t)[0] });
        }

        private getY1Axis(scale: d3.scale.Linear<number, number>, orientation: string): d3.svg.Axis {
            var axis: d3.svg.Axis = d3.svg.axis()
                .scale(scale)
                .orient(orientation)
                .tickSubdivide(0)
                .tickFormat(d3.format(",.0f"));

            return axis;
        }

        private getY2Axis(scale: d3.scale.Linear<number, number>, orientation: string): d3.svg.Axis {
            var axis: d3.svg.Axis = d3.svg.axis()
                .scale(scale)
                .orient(orientation)
                .tickSubdivide(0);

            // HACK:  This could be done better
            if (this.y2DataSetName.toLowerCase() == "totalnetvalue") {
                axis.tickFormat((d) => {
                    var divValue: number = d / 1000;
                    var sep: string = "";

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
            } else {
                axis.tickFormat(d3.format(",.0f"));
            }

            return axis;
        }

        private getMargins(): IMargins {
            return { top: 20, right: 40, bottom: 70, left: 40 };
        }

        private getWidgetDimensions($widgetBox: JQuery): IDimensions {
            var $widget = $widgetBox
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
                    position: 'absolute', // Optional if #myDiv is already absolute
                    visibility: 'hidden',
                    display: 'block'
                });

            var wh = {
                width: $widget.width(),
                height: $widget.height()
            }

            $widget.attr("style", previousCss ? previousCss : "");

            return wh;
        }

        private renderInitialSvgSelection($displayDiv: JQuery): d3.Selection<any> {
            return d3.selectAll($displayDiv.toArray()).append("svg")
                .attr("width", this.dimensions.width + this.margin.left + this.margin.right)
                .attr("height", this.dimensions.height + this.margin.top + this.margin.bottom)
                .append("g")
                .attr("transform", "translate(" + this.margin.left + "," + this.margin.top + ")");
        }

        private renderXAxis(): void {
            this.svg.append("g")
                .attr("class", "x axis")
                .attr("transform", "translate(0," + this.dimensions.height + ")")
                .call(this.xAxis);
        }

        private renderY1Axis(): void {
            this.svg.append("g")
                .attr("class", "y y1 axis")
                .call(this.y1Axis)
                .append("text")
                .attr("class", "y1-axis-label")
            //.attr("transform", "rotate(-90)")
                .attr("y", -this.margin.top + 2)
                .attr("dy", ".71em")
                .style("text-anchor", "start")
                .text(this.y1Label);
        }

        private renderY2Axis(): void {
            this.svg.append("g")
                .attr("class", "y y2 axis")
                .attr("transform", "translate(" + this.dimensions.width + " ,0)")
                .call(this.y2Axis)
                .append("text")
            //.attr("transform", "rotate(-90)")
                .attr("class", "y2-axis-label")
                .attr("y", -this.margin.top + 2)
                .attr("dy", ".71em")
                .style("text-anchor", "end")
                .text(this.y2Label);
        }

        private renderBars(dataSetName: string): void {
            var bars = this.svg.selectAll(".bar")
                .data(this.getDataSet(dataSetName));

            var me = this;

            bars
                .enter().append("rect")
                .attr("class", "bar")
                .attr("width", this.xScale.rangeBand())
                .attr("x", (d) => { return this.xScale(d.Key) })
                .attr("y", this.dimensions.height)
                .attr("height", 0)
                .transition()
                .delay((d, i) => { return i * 100 })
                .attr("y", (d) => {
                    var val = this.y1Scale(d.Value);

                    return val > 0 ? val : 0;
                })
                .attr("height", (d) => {
                    var val = this.dimensions.height - this.y1Scale(d.Value);

                    return val > 0 ? val : 0;
                });
            bars
                .on("mouseover", (d) => {
                    me.tooltip.transition()
                        .duration(200)
                        .style("opacity", .9);
                    me.tooltip.html(d3.format(",.0f")(d.Value))
                        .style("left", (d3.event.pageX + 5) + "px")
                        .style("top", (d3.event.pageY - 15) + "px");
                })
                .on("mouseout", (d) => {
                    me.tooltip.transition()
                        .duration(500)
                        .style("opacity", 0);
                });

            bars
                .exit().remove();
        }

        private renderLine(dataSetName: string) {
            var countLine = d3.svg.line<WidgetDataElement>()
                .x((d) => { return this.xScale(d.Key) + this.xScale.rangeBand() / 2; }) // Half range band to go to middle of bar
                .y((d) => { return this.y2Scale(d.Value); })
                .interpolate("monotone");

            var dataSet = this.getDataSet(dataSetName);

            var path = this.svg.selectAll(".line-count")
                .data(dataSet);

            path
                .enter().append("path")
                .attr("class", "line-count")
                .attr("d", countLine(dataSet));

            var totalLength = (<SVGPathElement>path.node()).getTotalLength();

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

        }

        private renderMarkers(dataSetName: string) {
            var dataSet = this.getDataSet(dataSetName);
            var me = this;

            var marker = this.svg.selectAll('.line-count-marker')
                .data(dataSet);

            marker
                .enter().append("circle")
                .attr("class", "line-count-marker")
                .attr("r", 3)
                .attr("cx", (d) => { return this.xScale(d.Key) + this.xScale.rangeBand() / 2 })
                .attr("cy", (d) => { return this.y2Scale(d.Value) });

            marker
                .on("mouseover", (d) => {
                    me.tooltip.transition()
                        .duration(200)
                        .style("opacity", .9);
                    me.tooltip.html(d3.format(",.0f")(d.Value))
                        .style("left", (d3.event.pageX + 5) + "px")
                        .style("top", (d3.event.pageY - 15) + "px");
                })
                .on("mouseout", (d) => {
                    me.tooltip.transition()
                        .duration(500)
                        .style("opacity", 0);
                });

            marker.
                exit().remove();
        }

        private renderTooltip() {
            return d3.select("body").append("div")
                .attr("class", "chart-tooltip")
                .style("opacity", 0);
        }

        private renderWidget($widget: JQuery, $displayDiv: JQuery, $widgetBox: JQuery) {
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
        }

        public render($widgets: JQuery, data: IWidgetData) {
            super.render($widgets, data);

            if (super.checkData(data)) {
                $(this).find('.no-data').show();
                return;
            }

            this.data = data;

            var me = this;
            $widgets.each(function () {
                var $widget = $(this),
                    $displayDiv = $(this).find('.chart-content'),
                    $widgetBox = $widget.children('div');

                me.renderWidget($widget, $displayDiv, $widgetBox);
            });
        }
    }
} 