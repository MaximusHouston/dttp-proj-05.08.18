import { InstanceEventService } from '../../events/instance-event.service';
import { StockChartComponent } from '../../stock-chart.component';
/**
 * @hidden
 */
export declare class StockInstanceEventService extends InstanceEventService {
    create(name: string, args: any, sender: StockChartComponent): any;
}
