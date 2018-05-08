import LineChart from '../line-chart/line-chart';
import SplineSegment from '../line-chart/spline-segment';
import LineSegment from '../line-chart/line-segment';

import { Point, Box } from '../../core';

import { SMOOTH } from '../constants';

import { setDefaultOptions } from '../../common';

class RadarLineChart extends LineChart {

    pointSlot(categorySlot, valueSlot) {
        const valueRadius = categorySlot.center.y - valueSlot.y1;
        const slot = Point.onCircle(categorySlot.center, categorySlot.middle(), valueRadius);

        return new Box(slot.x, slot.y, slot.x, slot.y);
    }

    createSegment(linePoints, currentSeries, seriesIx) {
        const style = currentSeries.style;
        let pointType;

        if (style === SMOOTH) {
            pointType = SplineSegment;
        } else {
            pointType = LineSegment;
        }

        const segment = new pointType(linePoints, currentSeries, seriesIx);

        if (linePoints.length === currentSeries.data.length) {
            segment.options.closed = true;
        }

        return segment;
    }
}

setDefaultOptions(RadarLineChart, {
    clip: false,
    limitPoints: false
});

export default RadarLineChart;