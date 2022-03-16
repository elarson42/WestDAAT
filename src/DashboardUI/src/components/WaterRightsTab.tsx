import { ChangeEvent, useCallback, useContext, useEffect, useMemo, useState } from "react";
import DropdownMultiselect from "react-multiselect-dropdown-bootstrap";
import Button from "react-bootstrap/Button";
import ButtonGroup from "react-bootstrap/ButtonGroup";
import ToggleButton from "react-bootstrap/ToggleButton";
import FlowRangeSlider from "./FlowRangeSlider";
import { MapContext } from "./MapProvider";
import VolumeRangeSlider from "./VolumeRangeSlider";
import { AppContext } from "../AppProvider";
import { MapThemeSelector } from "./MapThemeSelector";
import deepEqual from 'fast-deep-equal/es6';
import { useQuery } from "react-query";
import { getBeneficialUses, getOwnerClassifications, getWaterSourceTypes } from "../accessors/systemAccessor";
import useProgressIndicator from "../hooks/useProgressIndicator";
import { useDebounceCallback } from "@react-hook/debounce";

enum MapGrouping {
  BeneficialUse = "beneficialUseCV",
  CustomerType = "2",
  SiteType = "3",
  WaterSourceType = "waterSourceType"
}

interface WaterRightsFilters {
  beneficialUses?: string[],
  ownerClassifications?: string[],
  waterSourceTypes?: string[],
  allocationOwner?: string,
  mapGrouping: MapGrouping
}

const colors = [
  '#006400',
  '#9ACD32',
  '#FF00E6',
  '#0000FF',
  '#32CD32',
  '#FF4500',
  '#9370DB',
  '#00FFFF',
  '#FF69B4',
  '#800080',
  '#00BFFF',
  '#FFD700',
  '#A52A2A',
  '#4B0082',
  '#808080',
  '#FFA500',
  '#D2691E',
  '#FFC0CB',
  '#F0FFF0',
  '#F5DEB3',
  '#FF0000'
]

const allWaterRightsLayers = [
  'agricultural',
  'aquaculture',
  'commercial',
  'domestic',
  'environmental',
  'fire',
  'fish',
  'flood',
  'heating',
  'industrial',
  'instream',
  'livestock',
  'mining',
  'municipal',
  'other',
  'power',
  'recharge',
  'recreation',
  'snow',
  'storage',
  'wildlife'
]

const defaultFilters = {
  beneficialUses: undefined,
  ownerClassifications: undefined,
  allocationOwner: undefined,
  waterSourceTypes: undefined,
  mapGrouping: MapGrouping.BeneficialUse
}

function WaterRightsTab() {
  const { data: allBeneficialUses, isFetching: isAllBeneficialUsesLoading } = useQuery(
    ['beneficialUses'],
    getBeneficialUses,
    {
      refetchOnWindowFocus: false,
      refetchOnMount: false,
      refetchOnReconnect: false,
      cacheTime: 8600000,
      staleTime: Infinity
    })

  const { data: allWaterSourceTypes, isFetching: isAllWaterSourceTypesLoading } = useQuery(
    ['waterSourceTypes'],
    getWaterSourceTypes,
    {
      refetchOnWindowFocus: false,
      refetchOnMount: false,
      refetchOnReconnect: false,
      cacheTime: 8600000,
      staleTime: Infinity
    })

  const { data: allOwnerClassifications, isFetching: isAllOwnerClassificationsLoading } = useQuery(
    ['ownerClassifications'],
    getOwnerClassifications,
    {
      refetchOnWindowFocus: false,
      refetchOnMount: false,
      refetchOnReconnect: false,
      cacheTime: 8600000,
      staleTime: Infinity
    })

  useProgressIndicator([!isAllBeneficialUsesLoading, !isAllWaterSourceTypesLoading, !isAllOwnerClassificationsLoading], "Loading Filter Data");

  const [radioValue, setRadioValue] = useState('1');
  const { setUrlParam, getUrlParam } = useContext(AppContext);

  const [filters, setFilters] = useState<WaterRightsFilters>(getUrlParam<WaterRightsFilters>("wr") ?? defaultFilters);

  const mapGrouping = useMemo(() => {
    let colorIndex = 0;
    let colorMapping: { key: string, color: string }[];
    switch (filters.mapGrouping) {
      case MapGrouping.BeneficialUse:
        colorMapping = allBeneficialUses?.map(a => ({ key: a, color: colors[colorIndex++ % colors.length] })) ?? []
        break;
      case MapGrouping.CustomerType:
        colorMapping = []
        break;
      case MapGrouping.SiteType:
        colorMapping = []
        break;
      case MapGrouping.WaterSourceType:
        colorMapping = allWaterSourceTypes?.map(a => ({ key: a, color: colors[colorIndex++ % colors.length] })) ?? []
        break;
    }
    return { property: filters.mapGrouping as string, colorMapping }
  }, [filters.mapGrouping, allBeneficialUses, allWaterSourceTypes])

  const radios = [
    { name: 'Both', value: '1' },
    { name: 'POD', value: '2' },
    { name: 'POU', value: '3' },
  ];

  const {
    setLegend,
    setLayerFilters: setMapLayerFilters,
    setVisibleLayers,
    renderedFeatures,
    setLayerCircleColors
  } = useContext(MapContext);

  useEffect(() => {
    let circleColorArray = ["case",
      ...mapGrouping.colorMapping
        .map(b => [["==", ["get", mapGrouping.property], b.key], b.color]).reduce((b, c) => b.concat(c), []),
      "#000000"
    ]
    setLayerCircleColors(allWaterRightsLayers.map(a => {
      return {
        layer: a,
        circleColor: circleColorArray
      }
    }))
  }, [setLayerCircleColors, mapGrouping])

  useEffect(() => {
    const legendItems = mapGrouping.colorMapping
      .filter(a => renderedFeatures.some(b => b.properties && a.key === b.properties[mapGrouping.property]));
    if (legendItems.length === 0) {
      setLegend(null);
    } else {
      setLegend(
        <>
          {

            legendItems.map(layer => {
              return (
                <div key={layer.key} className="legend-item">
                  <span className="legend-circle" style={{ "backgroundColor": layer.color }}></span>
                  {layer.key}
                </div>
              )
            })
          }
        </>);
    }
  }, [setLegend, mapGrouping, renderedFeatures])

  const [allocationOwnerValue, setAllocationOwnerValue] = useState(filters.allocationOwner ?? "")

  useEffect(() => {
    setVisibleLayers(allWaterRightsLayers)
  }, [setVisibleLayers])

  useEffect(() => {
    if (deepEqual(filters, defaultFilters)) {
      setUrlParam("wr", undefined);
    } else {
      setUrlParam("wr", filters);
    }
  }, [filters, setUrlParam])

  const handleMapGroupingChange = useCallback((e: ChangeEvent<HTMLSelectElement>) => {
    setFilters(s => ({
      ...s,
      mapGrouping: e.target.value as MapGrouping
    }));
  }, [setFilters]);

  const handleBeneficialUseChange = useCallback((selectedOptions: string[]) => {
    setFilters(s => ({
      ...s,
      beneficialUses: selectedOptions.length > 0 ? selectedOptions : undefined
    }));
  }, [setFilters]);

  const handleOwnerClassificationChange = useCallback((selectedOptions: string[]) => {
    setFilters(s => ({
      ...s,
      ownerClassifications: selectedOptions.length > 0 ? selectedOptions : undefined
    }));
  }, [setFilters]);

  const setAllocationOwner = useDebounceCallback((allocationOwnerValue: string) => {
    setFilters(s => ({
      ...s,
      allocationOwner: allocationOwnerValue.length > 0 ? allocationOwnerValue : undefined
    }));
  }, 400)

  const handleAllocationOwnerChange = (e: ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value ?? "";
    setAllocationOwnerValue(value);
    setAllocationOwner(value)
  }

  const handleWaterSourceTypeChange = useCallback((selectedOptions: string[]) => {
    setFilters(s => ({
      ...s,
      waterSourceTypes: selectedOptions.length > 0 ? selectedOptions : undefined
    }));
  }, [setFilters]);

  useEffect(() => {
    const filterSet = ["all"] as any[];
    if (filters.beneficialUses && filters.beneficialUses.length > 0) {
      filterSet.push(["in", ["get", "beneficialUseCV"], ["literal", filters.beneficialUses]]);
    }
    if (filters.ownerClassifications && filters.ownerClassifications.length > 0) {
      filterSet.push(["in", ["get", "ownerClassification"], ["literal", filters.ownerClassifications]]);
    }
    if (filters.waterSourceTypes && filters.waterSourceTypes.length > 0) {
      filterSet.push(["in", ["get", "waterSourceType"], ["literal", filters.waterSourceTypes]]);
    }
    if (filters.allocationOwner && filters.allocationOwner.length > 0) {
      filterSet.push(["in", filters.allocationOwner.toUpperCase(), ["upcase", ["get", "allocationOwner"]]])
    }
    setMapLayerFilters(allWaterRightsLayers.map(a => {
      return { layer: a, filter: filterSet }
    }))
  }, [filters, setMapLayerFilters])

  const clearMapFilters = () => {
    setFilters(defaultFilters);
    setAllocationOwnerValue("");
  }

  if (isAllBeneficialUsesLoading || isAllWaterSourceTypesLoading || isAllOwnerClassificationsLoading) return null;

  return (
    <>
      <div className="map-info text-center p-2">
        {renderedFeatures.length} Points of Diversions Displayed
      </div>
      <div className="position-relative flex-grow-1">
        <div className="panel-content p-3">
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
            <select className="form-select" onChange={handleMapGroupingChange} value={filters.mapGrouping}>
              <option value={MapGrouping.BeneficialUse}>Beneficial Use</option>
              <option value={MapGrouping.CustomerType}>Customer Type</option>
              <option value={MapGrouping.SiteType}>Site Type</option>
              <option value={MapGrouping.WaterSourceType}>Water Source Type</option>
            </select>
          </div>

          <div className="mb-3">
            <label>Search Allocation Owner</label>
            <input type="text" className="form-control" onChange={handleAllocationOwnerChange} value={allocationOwnerValue} />
          </div>

          <div className="mb-3">
            <label>Owner Classification</label>
            <DropdownMultiselect
              className="form-control"
              options={allOwnerClassifications}
              selected={filters.ownerClassifications ?? []}
              handleOnChange={handleOwnerClassificationChange}
              name="ownerClassification"
            />
          </div>

          <div className="mb-3">
            <label>Beneficial Use</label>
            <DropdownMultiselect
              className="form-control"
              options={allBeneficialUses}
              selected={filters.beneficialUses ?? []}
              handleOnChange={handleBeneficialUseChange}
              name="beneficialUses"
            />
          </div>

          <div className="mb-3">
            <label>Water Source Type</label>
            <DropdownMultiselect
              className="form-control"
              options={allWaterSourceTypes}
              selected={filters.waterSourceTypes ?? []}
              handleOnChange={handleWaterSourceTypeChange}
              name="beneficialUses"
            />
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

          <div className="mb-3">
            <label>Volume Range</label>
            <span>- AF to - AF</span>
            <VolumeRangeSlider handleChange={(values) => console.log(values)} />
          </div>

          {/* <div className="mb-3">
        <label>Allocation Priority Date</label>
        <span>{allocationDates[0]} to {allocationDates[1]}</span>
        <AllocationDateSlider handleChange={handleAllocationDateChange} dates={allocationDates} />
      </div> */}

          <div className="mb-3">
            <label>MAP THEME</label>
            <MapThemeSelector />
          </div>

          <div className="mt-4">
            <Button className="w-100" onClick={clearMapFilters}>
              Reset All Filters
            </Button>
          </div>
        </div>
      </div>

    </>
  );
}

export default WaterRightsTab;