import { GeneralInfo } from './BusComponents';
import './GeneralInfo.css';

type GeneralInfoProps = {
    generalInfo: GeneralInfo
}

export const GeneralInfoComponent = (props: GeneralInfoProps) => {
    return (
        <div className='container'>
            <p>Bus Name: {props.generalInfo.name}</p>
            <p>Address: {props.generalInfo.address}</p>
            <p>Embus Port: {props.generalInfo.embusPort}</p>
            <p>Debus Port: {props.generalInfo.debusPort}</p>
            <p>State: {props.generalInfo.state}</p>
        </div>
    )  
}