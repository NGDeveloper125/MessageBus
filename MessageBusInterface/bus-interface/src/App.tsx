import { useState } from 'react';
import { TrackNewBus } from './backend-access/BusTracker'
import './App.css';
import { Bus } from './components/BusComponents';
import { BusComponent } from './components/Bus';

function App() {

  const [buses, setBuses] = useState<Bus[]>([]);
  const [address, setAddress] = useState('');

  function HandleOnTrackNewBusClick() {
    if(address) {
      TrackNewBus(address, AddNewBus, UpdateBus);
    } 
  }

  function AddNewBus(bus: Bus) {
    setBuses(prevBuses => [...prevBuses, bus]);
  }

  function UpdateBus(updatedBus: Bus) {
    setBuses(prevBuses => prevBuses.map(bus => 
      bus.busGeneralInfo.address === updatedBus.busGeneralInfo.address ? updatedBus : bus
    ));
  }

  return (
    <div className="App">
      <div className="title-bar">
        <h1>Message Bus Interface Manager</h1>
        <input 
          placeholder='address'
          value={address}
          onChange={(e) => setAddress(e.target.value)}
          />
        <button onClick={HandleOnTrackNewBusClick}>Track New MessageBus</button>
      </div>
      <div>
        {buses.length === 0 ? (<p>No buses are being track</p>) : (buses.map((bus, index) => (<BusComponent key={index} bus={bus}/>)))}
      </div>
    </div>
  );
}

export default App;
