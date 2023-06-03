import { useCallback, useContext } from "react";
import { WaterRightsContext, WaterRightsFilters } from "../../Provider";
import { waterRightsProperties } from "../../../../../config/constants";
import { useEqualsFilter as useEqualsFilterBase } from "../../../../../hooks/filters/useEqualsFilter";

type ValidEqualsFilters = 'podPou' | 'includeExempt';
export function useEqualsFilter<K1 extends keyof Pick<WaterRightsFilters, ValidEqualsFilters>>(field: K1, mapField: waterRightsProperties) {
  const { filters: { [field]: value }, setFilters } = useContext(WaterRightsContext);

  const { mapFilters } = useEqualsFilterBase(value, mapField);

  const setValue = useCallback((val: typeof value) => {
    setFilters(s => ({
      ...s,
      [field]: val
    }));
  }, [field, setFilters]);

  return { value, setValue, mapFilters };
}

