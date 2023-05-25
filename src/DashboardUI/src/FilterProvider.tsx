import { createContext, FC, useContext, useState } from "react";
import { AppContext } from "./AppProvider";
import { BeneficialUseListItem } from "./data-contracts/BeneficialUseListItem";
import { DataPoints, Directions } from "./data-contracts/nldi";

export interface WaterRightsFilters{
  beneficialUses?: BeneficialUseListItem[],
  ownerClassifications?: string[],
  waterSourceTypes?: string[],
  riverBasinNames?: string[],
  states?: string[],
  allocationOwner?: string,
  includeExempt: boolean | undefined,
  minFlow: number | undefined,
  maxFlow: number | undefined,
  minVolume: number | undefined,
  maxVolume: number | undefined,
  podPou: "POD" | "POU" | undefined,
  minPriorityDate: number | undefined,
  maxPriorityDate: number | undefined,
  polyline: { identifier: string, data: GeoJSON.Feature<GeoJSON.Geometry> }[],
  nldiFilterData: NldiFilters | null
}

export interface NldiFilters{ 
  latitude: number | null,
  longitude: number | null,
  directions: Directions,
  dataPoints: DataPoints
}

interface FilterContextState {
  filters: WaterRightsFilters;
  setFilters: React.Dispatch<React.SetStateAction<WaterRightsFilters>>;
  nldiIds: string[];
  setNldiIds: React.Dispatch<React.SetStateAction<string[]>>;
}

export const defaultFilters: WaterRightsFilters = {
  beneficialUses: undefined,
  ownerClassifications: undefined,
  allocationOwner: undefined,
  waterSourceTypes: undefined,
  states: undefined,
  riverBasinNames: undefined,
  includeExempt: undefined,
  minFlow: undefined,
  maxFlow: undefined,
  minVolume: undefined,
  maxVolume: undefined,
  podPou: undefined,
  minPriorityDate: undefined,
  maxPriorityDate: undefined,
  polyline: [],
  nldiFilterData: null
}

export const defaultNldiFilters = {
  latitude: null as number | null,
  longitude: null as number | null,
  directions: Directions.Upsteam | Directions.Downsteam as Directions,
  dataPoints: DataPoints.Usgs | DataPoints.Epa | DataPoints.Wade as DataPoints
}

const defaultState: FilterContextState = {
  filters: defaultFilters,
  setFilters: () => {},
  nldiIds: [],
  setNldiIds: () => {},
}

export const FilterContext = createContext<FilterContextState>(defaultState);

export const FilterProvider: FC = ({ children }) => {
  const { getUrlParam } = useContext(AppContext);

  const [ filters, setFilters ] = useState<WaterRightsFilters>(getUrlParam<WaterRightsFilters>("wr") ?? defaultFilters);
  const [ nldiIds, setNldiIds ] = useState<string[]>([]);

  const filterContextProviderValue = {
    filters,
    setFilters,
    nldiIds,
    setNldiIds
  }

  return (
    <FilterContext.Provider value={filterContextProviderValue}>
      {children}
    </FilterContext.Provider>
  );
}