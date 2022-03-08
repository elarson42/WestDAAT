import { useContext, useEffect, useState } from "react";
import ButtonGroup from "react-bootstrap/ButtonGroup";
import ToggleButton from "react-bootstrap/ToggleButton";
import FlowRangeSlider from "./FlowRangeSlider";
import { MapContext } from "./MapProvider";
import mapConfig from "../config/maps.json";

function WaterRightsTab() {

  const [radioValue, setRadioValue] = useState('1');
  const radios = [
    { name: 'Both', value: '1' },
    { name: 'POD', value: '2' },
    { name: 'POU', value: '3' },
  ];

  const { layers, setLayerVisibility, setCurrentSources, setCurrentLayers, setLegend } = useContext(MapContext);

  useEffect(() => {
    setCurrentSources((mapConfig as any).waterRights.sources);
    setCurrentLayers((mapConfig as any).waterRights.layers);
  }, [setCurrentSources, setCurrentLayers])

  useEffect(() => {
    const mapData = (mapConfig as any).waterRights.layers;
    if (mapData) {
      setLegend(<div className="legend">
        <div>
          {
            //Sort legend items alphabetically
            mapData.sort((a: any, b: any) =>
              a.friendlyName > b.friendlyName ? 1 : -1
            ).map((layer: any) => {
              // Null check for layer paint property
              let color = layer?.paint ? layer.paint["circle-color"] as string : "#000000";
              return (
                <div key={layer.id}>
                  <span style={{ "backgroundColor": color }}></span>
                  {layer.friendlyName}
                </div>
              );
            }
            )
          }
        </div>
      </div>);
    }

  })

  const handleBenefitUseChange = (layerId: string) => {
    // Filter to current layer only (will be multi-select eventually)
    layers.forEach(layer => {
      if (layer.layout) {
        setLayerVisibility(layer.id, layer.id === layerId);
      }
    })
  };

  return (
    <>
      <div className="mb-3">
        <label>FILTERS</label>
        <a href="/filters" target="_blank">Learn about WaDE filters</a>
      </div>

      <div className="mb-3">
        <label>TOGGLE VIEW</label>
        <ButtonGroup className="w-100">
          {radios.map((radio, idx) => (
            <ToggleButton
              key={idx}
              id={`radio-${idx}`}
              type="radio"
              variant="outline-primary"
              name="radio"
              value={radio.value}
              checked={radioValue === radio.value}
              onChange={(e) => setRadioValue(e.currentTarget.value)}
            >
              {radio.name}
            </ToggleButton>
          ))}
        </ButtonGroup>
      </div>

      <div className="mb-3">
        <label>Change Map Legend</label>
        <select className="form-select">
          <option>Beneficial Use</option>
        </select>
      </div>

      <div className="mb-3">
        <label>Search Location</label>
        <input type="text" className="form-control" />
      </div>

      <div className="mb-3">
        <label>Search Allocation Owner</label>
        <input type="text" className="form-control" />
      </div>

      <div className="mb-3">
        <label>Owner Classification</label>
        <select className="form-select">
        </select>
      </div>

      <div className="mb-3">
        <label>Beneficial Use</label>
        <select className="form-select" onChange={(event) => handleBenefitUseChange(event.target.value)}>
          {
            layers.map(layer =>
              <option key={layer.id} value={layer.id}>{layer.friendlyName}</option>
            )
          }
        </select>
      </div>

      <div className="mb-3">
        <label>Water Source Type</label>
        <select className="form-select">
        </select>
      </div>

      <div className="mb-3 form-check form-switch">
        <input className="form-check-input" type="checkbox" id="flexSwitchCheckDefault" />
        <label className="form-check-label">Include Empty Amount and Priority Date Value</label>
      </div>

      <div className="mb-3">
        <label>Flow Range</label>
        <span>- CFS to - CFS</span>
        <FlowRangeSlider handleChange={(values) => console.log(values)} />
      </div>
    </>
  );
}

export default WaterRightsTab;
