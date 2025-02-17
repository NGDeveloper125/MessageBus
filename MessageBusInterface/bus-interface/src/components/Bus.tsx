import { Bus } from "./BusComponents";
import { BusErrorComponent } from "./BusError";
import { GeneralInfoComponent } from "./GeneralInfo";
import { QueueComponent } from "./Queue";
import './Bus.css';

type BusProps = {
    bus: Bus
}

export const BusComponent = (props: BusProps) => {
    return (
        <div className="bus">
            <div className="bus-general-info">
                <GeneralInfoComponent generalInfo={props.bus.busGeneralInfo} />
            </div>
            <div className="bus-queues">
                {props.bus.queues.map(queue => (
                    <QueueComponent queue={queue} />
                ))}
            </div>
            <div className="bus-errors">
                {props.bus.errors.map(error => (
                    <BusErrorComponent busError={error} />
                ))}
            </div>
        </div>
    )
}