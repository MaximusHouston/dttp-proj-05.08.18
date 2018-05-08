import LineChart from '../line-chart/line-chart';
import SplineSegment from '../line-chart/spline-segment';
import LineSegment from '../line-chart/line-segment';

import { Point, Box } from '../../core';

import { SMOOTH } from '../constants';

import { setDefaultOptions } from '../../common';

var RadarLineChart = (function (LineChart) {
    function RadarLineChart () {
        LineChart.apply(this, arguments);
    }

    if ( LineChart ) RadarLineChart.__proto__ = LineChart;
    RadarLineChart.prototype = Object.create( LineChart && LineChart.prototype );
    RadarLineChart.prototype.constructor = RadarLineChart;

    RadarLineChart.prototype.pointSlot = function pointSlot (categorySlot, valueSlot) {
        var valueRadius = categorySlot.center.y - valueSlot.y1;
        var slot = Point.onCircle(categorySlot.center, categorySlot.middle(), valueRadius);

        return new Box(slot.x, slot.y, slot.x, slot.y);
    };

    RadarLineChart.prototype.createSegment = function createSegment (linePoints, currentSeries, seriesIx) {
        var style = currentSeries.style;
        var pointType;

        if (style === SMOOTH) {
            pointType = SplineSegment;
        } else {
            pointType = LineSegment;
        }

        var segment = new pointType(linePoints, currentSeries, seriesIx);

        if (linePoints.length === currentSeries.data.length) {
            segment.options.closed = true;
        }

        return segment;
    };

    return RadarLineChart;
}(LineChart));

setDefaultOptions(RadarLineChart, {
    clip: false,
    limitPoints: false
});

export default RadarLineChart;