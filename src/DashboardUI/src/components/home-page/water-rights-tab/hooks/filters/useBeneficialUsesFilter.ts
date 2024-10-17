import { waterRightsProperties } from '../../../../../config/constants';
import { useInFilter } from '../../../../../hooks/filters/useInFilter';
import { useWaterRightsContext } from '../../Provider';

export function useBeneficialUsesFilter() {
  const context = useWaterRightsContext();
  const values = context.state.filters.beneficialUseNames;

  const setValues = (payload: string[] | undefined) => {
    context.dispatch({ type: 'SET_BENEFICIAL_USE_NAME_FILTERS', payload });
  };

  const { mapFilters } = useInFilter(
    values,
    context.state.beneficialUsesQuery.data?.length,
    waterRightsProperties.beneficialUses
  );

  return {
    beneficialUseNames: values,
    setBeneficialUseNames: setValues,
    mapFilters,
  };
}
