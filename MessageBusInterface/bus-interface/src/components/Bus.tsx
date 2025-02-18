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
        <div className="bus" style={props.bus.busGeneralInfo.state === 'Running' ? {borderColor: 'lime'} : {borderColor: 'red'}}>
            <div className="bus-general-info">
                <h3>General Info</h3>
                <GeneralInfoComponent generalInfo={props.bus.busGeneralInfo} />
            </div>
            <div className="bus-queues" >
                <h3>Queues</h3>
                {props.bus.queues.map(queue => (
                    <QueueComponent queue={queue} />
                ))}
            </div>
            <div className="bus-errors">
                <h3>Errors</h3>
                {props.bus.errors.map(error => (
                    <BusErrorComponent busError={error} />
                ))}
            </div>
        </div>
    )
}