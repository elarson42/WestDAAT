import { useState } from "react";
import ButtonGroup from "react-bootstrap/ButtonGroup";
import Button from "react-bootstrap/Button";
import Form from "react-bootstrap/Form";
import ToggleButton from "react-bootstrap/ToggleButton";
import { Handles, Rail, Slider, Ticks, Tracks } from "react-compound-slider";
import { Handle, SliderRail, Tick, Track } from "./SliderComponents";
import FlowRangeSlider from "./FlowRangeSlider";

function WaterRightsTab() {

  const [radioValue, setRadioValue] = useState('1');
  const radios = [
    { name: 'Both', value: '1' },
    { name: 'POD', value: '2' },
    { name: 'POU', value: '3' },
  ];

  const domain = [100, 500];
  const defaultValues = [450, 400, 300, 150];

  const onUpdate = () => {
    console.log("On slider update");
  };

  const onChange = () => {
    console.log("On slider change");
  };

  return (
    <>
      <div className="mb-3">
        <label>FILTERS</label>
        <a href="#" target="_blank">Learn about WaDE filters</a>
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
        <select className="form-select">
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
        <FlowRangeSlider />
      </div>
    </>
  );
}

export default WaterRightsTab;
