import { Queue } from './BusComponents';
import './Queue.css';

type QueueProps = {
    queue: Queue
}

export const QueueComponent = (props: QueueProps) => {
    let queueColor = 'lime';
    if(props.queue.state === 'high traffic') {
        queueColor = 'red';
    } else if(props.queue.state === 'low traffic')
    {
        queueColor = 'lightblue';
    }

    return (
        <div className='container' style={{backgroundColor: queueColor}}>
            <p>{props.queue.name} | {props.queue.type} | {props.queue.state} | Messages in queue: {props.queue.size}</p>
        </div>
    )
}