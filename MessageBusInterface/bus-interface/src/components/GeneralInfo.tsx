import { GeneralInfo } from './BusComponents';

type GeneralInfoProps = {
    generalInfo: GeneralInfo
}

export const GeneralInfoComponent = (props: GeneralInfoProps) => {
    return (
        <div>
            <p>Name: {props.generalInfo.name}</p>
            <p>Address: {props.generalInfo.address}</p>
            <p>Embus Port: {props.generalInfo.embusPort}</p>
            <p>Debus Port: {props.generalInfo.debusPort}</p>
            <p>State: {props.generalInfo.state}</p>
        </div>
    )  
}