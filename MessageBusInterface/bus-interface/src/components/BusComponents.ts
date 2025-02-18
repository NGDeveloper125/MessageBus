export interface Queue {
    name: string
    type: string
    state: string 
    size: string
}

export interface GeneralInfo {
    name: string
    address: string
    embusPort: string
    debusPort: string
    state: string
}

export interface BusError {
    description: string
}

export interface Bus {
    busGeneralInfo: GeneralInfo
    queues: Queue[]
    errors: BusError[]
}
