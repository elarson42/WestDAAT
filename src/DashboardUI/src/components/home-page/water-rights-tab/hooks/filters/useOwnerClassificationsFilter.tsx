import { waterRightsProperties } from '../../../../../config/constants';
import { useWaterRightsContext } from '../../Provider';
import { useInFilter } from '../../../../../hooks/filters/useInFilter';

export function useOwnerClassificationsFilter() {
  const context = useWaterRightsContext();
  const values = context.state.filters.ownerClassifications;

  const setValues = (payload: string[] | undefined) => {
    context.dispatch({ type: 'SET_OWNER_CLASSIFICATION_FILTERS', payload });
  };

  const { mapFilters } = useInFilter(
    values,
    context.state.ownerClassificationsQuery.data?.length,
    waterRightsProperties.ownerClassifications
  );

  return {
    ownerClassifications: values,
    setOwnerClassifications: setValues,
    mapFilters,
  };
}
