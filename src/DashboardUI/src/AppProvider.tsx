import { createContext, FC, useCallback, useEffect, useState } from "react";
import { useSearchParams } from "react-router-dom";
import * as compress from "lz-string";

export interface User {
  username: string;
}

interface AppContextState {
  user: User | null,
  setUser: (user: User | null) => void,

  setUrlParam: (key: string, value: any) => void,
  getUrlParam: <T, >(key: string) => T | undefined,
};

let defaultAppContextState = {
  user: null,
  setUser: (user: User | null) => { },

  setUrlParam: (key: string, value: any) => { },
  getUrlParam: <T,>(key: string): T | undefined => undefined,
}

export const AppContext = createContext<AppContextState>(defaultAppContextState);

const AppProvider: FC = ({ children }) => {

  let [urlParams, setUrlParams] = useSearchParams();

  const [user, setUser] = useState(null as User | null);

  const initUrlParams = () => {
    const stateStr = urlParams.get("state");
    if (stateStr) {
      const decompressed = compress.decompressFromEncodedURIComponent(stateStr);
      if (decompressed) {
        return JSON.parse(decompressed) as Record<string, any>;
      }
    }
    return {};
  }

  const [stateUrlParams, setStateUrlParams] = useState(initUrlParams());

  const setUrlParam = useCallback((key: string, value: {} | undefined): void => {
    setStateUrlParams(s => {
      const updatedValues = { ...s };
      if (value === undefined) {
        delete updatedValues[key]
      } else {
        updatedValues[key] = value;
      }
      return updatedValues;
    })
  }, [])

  const getUrlParam = useCallback(<T,>(key: string): T | undefined => {
    var param = stateUrlParams[key];
    if (param) {
      return param as T;
    }
  }, [stateUrlParams])

  useEffect(() => {
    if ((Object.keys(stateUrlParams).length ?? 0) > 0) {
      setUrlParams({ state: compress.compressToEncodedURIComponent(JSON.stringify(stateUrlParams)) })
    } else {
      setUrlParams({});
    }

  }, [stateUrlParams, setUrlParams])

  const appContextProviderValue = {
    user,
    setUser,
    setUrlParam,
    getUrlParam
  };

  return (
    <AppContext.Provider value={appContextProviderValue}>
      {children}
    </AppContext.Provider>
  );
}

export default AppProvider;
