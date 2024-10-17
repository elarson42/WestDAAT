import React, { useMemo, useReducer } from 'react';
import { createContext, FC, useCallback, useContext, useEffect, useState } from 'react';
import { BeneficialUseListItem } from '../../../data-contracts/BeneficialUseListItem';
import { DataPoints, Directions } from '../../../data-contracts/nldi';
import { useDisplayOptionsUrlParameters } from './hooks/url-parameters/useDisplayOptionsUrlParameters';
import { defaultDisplayOptions, DisplayOptions } from './DisplayOptions';
import { useFiltersUrlParameters } from './hooks/url-parameters/useFiltersUrlParameters';
import {
  useBeneficialUses,
  useOwnerClassifications,
  useStates,
  useWaterSourceTypes,
} from '../../../hooks/queries/useSystemQuery';
import { useRiverBasinOptions } from '../../../hooks/queries/useRiverBasinOptions';
import { UseQueryResult } from 'react-query';
import { EmptyPropsWithChildren } from '../../../HelperTypes';
import { produce } from 'immer';

// TODO unit tests wouldn't kill you

interface State {
  beneficialUsesQuery: Query<BeneficialUseListItem[]>;
  waterSourcesQuery: Query<string[]>;
  ownerClassificationsQuery: Query<string[]>;
  statesQuery: Query<string[]>;
  riverBasinsQuery: Query<string[]>;

  displayOptions: DisplayOptions;
  filters: WaterRightsFilters;
  nldiIds: string[];
}

type Action =
  | { type: 'RESET_USER_OPTIONS' }
  | { type: 'SET_BENEFICIAL_USE_NAME_FILTERS'; payload: string[] | undefined }
  | { type: 'SET_OWNER_CLASSIFICATION_FILTERS'; payload: string[] | undefined }
  | { type: 'SET_STATE_FILTERS'; payload: string[] | undefined }
  | { type: 'SET_WATER_SOURCE_TYPE_FILTERS'; payload: string[] | undefined };

interface WaterRightsContextProps {
  state: State;
  dispatch: React.Dispatch<Action>;
}

const WaterRightsContext = createContext<WaterRightsContextProps | undefined>(undefined);

type WaterRightsProviderProps = EmptyPropsWithChildren;

export const WaterRightsProvider: FC<WaterRightsProviderProps> = ({ children }) => {
  // Create initial state with defaults and queries.
  const defaultState = createDefaultState();
  defaultState.beneficialUsesQuery = useBeneficialUses();
  defaultState.waterSourcesQuery = useWaterSourceTypes();
  defaultState.ownerClassificationsQuery = useOwnerClassifications();
  defaultState.statesQuery = useStates();
  defaultState.riverBasinsQuery = useRiverBasinOptions();

  const [state, dispatch] = useReducer(reducer, defaultState);

  // This will reduce unnecessary re-renders when the state has not changed.
  const value = useMemo(() => ({ state, dispatch }), [state, dispatch]);

  return <WaterRightsContext.Provider value={value}>{children}</WaterRightsContext.Provider>;
};

const reducer = (state: State, action: Action): State => {
  // "produce" will deep clone and freeze state, enforcing immutability.
  return produce(state, (draft) => {
    switch (action.type) {
      case 'RESET_USER_OPTIONS':
        draft.filters = defaultFilters;
        draft.displayOptions = defaultDisplayOptions;
        draft.nldiIds = [];
        break;
      case 'SET_BENEFICIAL_USE_NAME_FILTERS':
        draft.filters.beneficialUseNames = distinctSortedOrUndefined(action.payload);
        break;
      case 'SET_OWNER_CLASSIFICATION_FILTERS':
        draft.filters.ownerClassifications = distinctSortedOrUndefined(action.payload);
        break;
      case 'SET_STATE_FILTERS':
        draft.filters.states = distinctSortedOrUndefined(action.payload);
        break;
      case 'SET_WATER_SOURCE_TYPE_FILTERS':
        draft.filters.waterSourceTypes = distinctSortedOrUndefined(action.payload);
        break;
      default:
        break;
    }

    return draft;
  });
};

const distinctSortedOrUndefined = (array: string[] | undefined): string[] | undefined => {
  return array ? [...new Set(array)].sort((a, b) => a.localeCompare(b)) : undefined;
};

export interface WaterRightsFilters {
  beneficialUseNames?: string[];
  ownerClassifications?: string[];
  waterSourceTypes?: string[];
  riverBasinNames?: string[];
  states?: string[];
  allocationOwner?: string;
  includeExempt: boolean | undefined;
  minFlow: number | undefined;
  maxFlow: number | undefined;
  minVolume: number | undefined;
  maxVolume: number | undefined;
  podPou: 'POD' | 'POU' | undefined;
  minPriorityDate: number | undefined;
  maxPriorityDate: number | undefined;
  polylines?: GeoJSON.Feature<GeoJSON.Geometry>[];
  isNldiFilterActive: boolean;
  nldiFilterData?: NldiFilters;
}

export interface NldiFilters {
  latitude: number | null;
  longitude: number | null;
  directions: Directions;
  dataPoints: DataPoints;
}

type Query<T> = Pick<UseQueryResult<T, unknown>, 'data' | 'isError' | 'isLoading'>;

const defaultQuery = { data: undefined, isError: false, isLoading: false };

export const defaultFilters: WaterRightsFilters = {
  beneficialUseNames: undefined,
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
  polylines: undefined,
  isNldiFilterActive: false,
  nldiFilterData: undefined,
};

const createDefaultState = (): State => {
  const state = {
    beneficialUsesQuery: defaultQuery,
    waterSourcesQuery: defaultQuery,
    ownerClassificationsQuery: defaultQuery,
    statesQuery: defaultQuery,
    riverBasinsQuery: defaultQuery,

    displayOptions: defaultDisplayOptions,

    filters: {
      beneficialUseNames: undefined,
      ownerClassifications: undefined,
      waterSourceTypes: undefined,
      riverBasinNames: undefined,
      states: undefined,
      allocationOwner: undefined,
      includeExempt: undefined,
      minFlow: undefined,
      maxFlow: undefined,
      minVolume: undefined,
      maxVolume: undefined,
      podPou: undefined,
      minPriorityDate: undefined,
      maxPriorityDate: undefined,
      polylines: undefined,
      isNldiFilterActive: false,
      nldiFilterData: undefined,
    },

    nldiIds: [],
  };

  return produce(state, (draft) => draft);
};

// interface WaterRightsContextState {
//   // filters: WaterRightsFilters;
//   // setFilters: React.Dispatch<React.SetStateAction<WaterRightsFilters>>;
//   // nldiIds: string[];
//   // setNldiIds: React.Dispatch<React.SetStateAction<string[]>>;
//   // displayOptions: DisplayOptions;
//   // setDisplayOptions: React.Dispatch<React.SetStateAction<DisplayOptions>>;
//   // resetUserOptions: () => void;
//   // hostData: HostData;

//   beneficialUsesQuery: Query<BeneficialUseListItem[]>;
//   waterSourcesQuery: Query<string[]>;
//   ownerClassificationsQuery: Query<string[]>;
//   statesQuery: Query<string[]>;
//   riverBasinsQuery: Query<string[]>;
// }

// interface Action {
//   type: string;
//   payload: any;
// }

// const createDefaultState = (): WaterRightsContextState => ({
//   // filters: defaultFilters,
//   // setFilters: () => {},
//   // nldiIds: [],
//   // setNldiIds: () => {},
//   // displayOptions: defaultDisplayOptions,
//   // setDisplayOptions: () => {},
//   // resetUserOptions: () => {},
//   // hostData: {
//   //   beneficialUsesQuery: defaultQuery,
//   //   waterSourcesQuery: defaultQuery,
//   //   ownerClassificationsQuery: defaultQuery,
//   //   statesQuery: defaultQuery,
//   //   riverBasinsQuery: defaultQuery,
//   // },

//   beneficialUsesQuery: defaultQuery,
//   waterSourcesQuery: defaultQuery,
//   ownerClassificationsQuery: defaultQuery,
//   statesQuery: defaultQuery,
//   riverBasinsQuery: defaultQuery,
// });

// export const WaterRightsProvider: FC = ({ children }) => {
//   const { getParameter: getDisplayOptionsParameter, setParameter: setDisplayOptionsParameter } =
//     useDisplayOptionsUrlParameters();
//   const { getParameter: getFiltersParameter, setParameter: setFiltersParameter } = useFiltersUrlParameters();

//   const [filters, setFilters] = useState<WaterRightsFilters>(getFiltersParameter() ?? defaultFilters);
//   const [displayOptions, setDisplayOptions] = useState<DisplayOptions>(
//     getDisplayOptionsParameter() ?? defaultDisplayOptions
//   );

//   useEffect(() => {
//     setDisplayOptionsParameter(displayOptions);
//   }, [displayOptions, setDisplayOptionsParameter]);

//   useEffect(() => {
//     setFiltersParameter(filters);
//   }, [filters, setFiltersParameter]);

//   const resetUserOptions = useCallback(() => {
//     setFilters(defaultFilters);
//     setDisplayOptions(defaultDisplayOptions);
//     setNldiIds([]);
//   }, [setFilters, setDisplayOptions]);

//   const filterContextProviderValue = {
//     filters,
//     setFilters,
//     nldiIds,
//     setNldiIds,
//     displayOptions,
//     setDisplayOptions,
//     resetUserOptions,
//     hostData: {
//       beneficialUsesQuery,
//       waterSourcesQuery,
//       ownerClassificationsQuery,
//       statesQuery,
//       riverBasinsQuery,
//     },
//   };

//   return <WaterRightsContext.Provider value={filterContextProviderValue}>{children}</WaterRightsContext.Provider>;
// };

export const defaultNldiFilters = {
  latitude: null as number | null,
  longitude: null as number | null,
  directions: Directions.Upsteam | (Directions.Downsteam as Directions),
  dataPoints: DataPoints.Usgs | DataPoints.Epa | (DataPoints.Wade as DataPoints),
};

export const useWaterRightsContext = (): WaterRightsContextProps => {
  const context = useContext(WaterRightsContext);

  if (!context) {
    throw new Error('useWaterRightsContext must be used within a WaterRightsProvider');
  }

  return context;
};
