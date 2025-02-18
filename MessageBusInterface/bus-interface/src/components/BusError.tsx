import { BusError } from "./BusComponents";
import './BusError.css';

type BusErrorProps = {
    busError: BusError
}

export const BusErrorComponent = (props: BusErrorProps) => {
    return (
        <div className="container">
            <p>{props.busError.description}</p>
        </div>
    )
}