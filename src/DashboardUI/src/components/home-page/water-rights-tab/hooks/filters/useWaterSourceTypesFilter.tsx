import { waterRightsProperties } from '../../../../../config/constants';
import { useInFilter } from '../../../../../hooks/filters/useInFilter';
import { useWaterRightsContext } from '../../Provider';

export function useWaterSourceTypesFilter() {
  const context = useWaterRightsContext();
  const values = context.state.filters.waterSourceTypes;

  const setValues = (payload: string[] | undefined) => {
    context.dispatch({ type: 'SET_WATER_SOURCE_TYPE_FILTERS', payload });
  };

  const { mapFilters } = useInFilter(
    values,
    context.state.waterSourcesQuery.data?.length,
    waterRightsProperties.waterSourceTypes
  );

  return {
    waterSourceTypes: values,
    setWaterSourceTypes: setValues,
    mapFilters,
  };
}
