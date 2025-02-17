import { Queue } from './BusComponents';

type QueueProps = {
    queue: Queue
}

export const QueueComponent = (props: QueueProps) => {
    return (
        <div>
            <p>Name: {props.queue.name} | Type: {props.queue.type} | State: {props.queue.state} | Size: {props.queue.size}</p>
        </div>
    )
}