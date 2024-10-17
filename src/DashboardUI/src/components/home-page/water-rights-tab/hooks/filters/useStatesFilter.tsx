import { waterRightsProperties } from '../../../../../config/constants';
import { useInFilter } from '../../../../../hooks/filters/useInFilter';
import { useWaterRightsContext } from '../../Provider';

export function useStatesFilter() {
  const context = useWaterRightsContext();
  const values = context.state.filters.states;

  const setValues = (payload: string[] | undefined) => {
    context.dispatch({ type: 'SET_STATE_FILTERS', payload });
  };

  const { mapFilters } = useInFilter(values, context.state.statesQuery.data?.length, waterRightsProperties.states);

  return {
    states: values,
    setStates: setValues,
    mapFilters,
  };
}
