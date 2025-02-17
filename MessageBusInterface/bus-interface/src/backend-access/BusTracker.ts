import { Bus } from "../components/BusComponents";

export const TrackNewBus = async (address: string, addNewBus: (newBus: Bus) => void, updateBus: (updatedBus: Bus) => void) => {
    const bus = await GetBusData(address);
    if (!bus) {
        return null;
    }

    addNewBus(bus);
    TrackBus(address, updateBus);
}

function TrackBus(address: string, updateBus: (bus: Bus) => void) {
    setInterval(async () => {
        const updatedBus = await GetBusData(address);
        if (updatedBus) {
            updateBus(updatedBus);
        }
    }, 10000)
}



async function GetBusData(address: string): Promise<Bus | null> {
    // for test case
    try {
        const response = await fetch('/bus_list.json');
        const data = await response.json();
        console.log('data: ', data);
        console.log('address: ' + address);
        const bus = data.Buses.find((b: any) => b.GeneralInfo.Address === address);

        if(!bus) return null;

        return {
            busGeneralInfo: {
                name: bus.GeneralInfo.Name,
                address: bus.GeneralInfo.Address,
                embusPort: bus.GeneralInfo.EmbusPort,
                debusPort: bus.GeneralInfo.DebusPort,
                state: bus.GeneralInfo.State,
            },
            queues: bus.Queues.map((q: any) => ({
                name: q.Name,
                type: q.Type,
                state: q.State,
                size: q.Size
            })),
            errors: bus.Errors.map((e: any) => ({
                description: e.Description
            }))
        };
    } catch (error) {
        console.error('Error fetching bus data:', error);
        return null;
    }
}