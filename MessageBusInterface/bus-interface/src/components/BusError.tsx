import { BusError } from "./BusComponents";

type BusErrorProps = {
    busError: BusError
}

export const BusErrorComponent = (props: BusErrorProps) => {
    return (
        <div>
            <p>{props.busError.description}</p>
        </div>
    )
}