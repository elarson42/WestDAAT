import { useMemo } from "react";

export function useInFilter<T>(vals: T[] | undefined, allValuesCount: number | undefined, mapField: string){
  const values = useMemo(() => {
    return [...new Set(vals)].sort() ?? [];
  }, [vals]);

  const areAllItemsSelected = useMemo(() => {
    return allValuesCount && values.length === allValuesCount;
  }, [values, allValuesCount]);

  const mapFilters = useMemo((): any[] | undefined => {
    // If there are newly selected values and not all items are selected
    // then return an array that has
    // ["any",
    // then for each selected value, ["in", the value, ["get", mapField]]
    // an example of mapFIeld would be waterRightsProperties.beneficialUses (i.e., 'bu')
    // So a full example of a result may look like
    // ["any", ["in", "Agriculture", ["get", "bu"]], ["in", "Domestic", ["get", "bu"]]]
    // then I guess if all items are selected, then return undefined?...seems odd?
    // how do you tell the difference between all items selected and no items selected?
    if (values.length > 0 && !areAllItemsSelected) {
      return ["any", ...values.map(a => ["in", a, ["get", mapField]])];
    }
  }, [values, mapField, areAllItemsSelected]);

  return { values, mapFilters };
}


