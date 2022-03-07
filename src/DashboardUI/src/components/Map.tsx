import mapboxgl, { NavigationControl } from "mapbox-gl";
import { useContext, useEffect, useRef, useState } from "react";

import MapboxGeocoder from '@mapbox/mapbox-gl-geocoder';
import '@mapbox/mapbox-gl-geocoder/dist/mapbox-gl-geocoder.css';

import "../styles/map.scss";
import mapConfig from "../config/maps.json";
import { AppContext, User } from "../AppProvider";
import { MapContext, MapData, MapTypes } from "./MapProvider";

// Fix transpile errors. Mapbox is working on a fix for this
// eslint-disable-next-line import/no-webpack-loader-syntax
(mapboxgl as any).workerClass = require('worker-loader!mapbox-gl/dist/mapbox-gl-csp-worker').default;


function Map() {

  const { user } = useContext(AppContext);
  const { baseMap, layers, sources, setCurrentLayers, setCurrentSources } = useContext(MapContext);

  const [mapData, setMapData] = useState((mapConfig as any)[MapTypes.WaterRights] as MapData);
  const [isMapLoaded, setIsMapLoaded] = useState(false);

  const map = useRef<mapboxgl.Map | null>(null);
  const navControl = useRef(new NavigationControl());
  let geocoderControl = useRef(new MapboxGeocoder({
    accessToken: mapboxgl.accessToken
  }));


  useEffect(() => {
    setIsMapLoaded(false);
    mapboxgl.accessToken = process.env.REACT_APP_MAPBOX_ACCESSTOKEN || "";
    map.current = new mapboxgl.Map({
      container: "map",
      style: "mapbox://styles/mapbox/light-v10",
      center: [-100, 40],
      zoom: 4,
    });

    map.current.on("load", () => setIsMapLoaded(true));

    updateMapControls(user);
  }, [mapData, user]);

  useEffect(() => {
    updateMapControls(user);
  }, [user]);

  useEffect(() => {
    setCurrentSources(mapData.sources);
    setCurrentLayers(mapData.layers);
  }, [mapData]);

  useEffect(() => {
    if (!isMapLoaded || !map.current) return;
    var myMap = map.current;

    sources.forEach(source => {
      if (myMap.getSource(source.id)) {
        myMap.removeSource(source.id);
      }
      myMap.addSource(source.id, source.source);
    });
  }, [sources, isMapLoaded]);

  useEffect(() => {
    if (!isMapLoaded || !map.current) return;
    var myMap = map.current;

    layers.forEach(layer => {
      if (myMap.getLayer(layer.id)) {
        myMap.removeLayer(layer.id);
      }
      myMap.addLayer(layer);
    });
  }, [layers, isMapLoaded]);

  useEffect(() => {
    setMapData((mapConfig as any)[baseMap]);
  }, [baseMap]);

  const updateMapControls = (user: User | null) => {
    if (!map.current) return;

    if (map.current.hasControl(geocoderControl.current)) {
      map.current.removeControl(geocoderControl.current);
    }

    if (map.current.hasControl(navControl.current)) {
      map.current.removeControl(navControl.current);
    }

    // Only allow location search for logged in users
    if (user) {
      geocoderControl.current = new MapboxGeocoder({
        accessToken: mapboxgl.accessToken,
        mapboxgl: map.current
      });
      map.current.addControl(geocoderControl.current);
    }

    map.current.addControl(navControl.current);
  }

  return (
    <div className="position-relative h-100">
      <div className="legend">
        <div>
          {
            // Sort legend items alphabetically
            mapData && mapData.layers.sort((a, b) =>
              a.friendlyName > b.friendlyName ? 1 : -1
            ).map(layer => {
              // Null check for layer paint property
              let color = layer?.paint ? layer.paint["circle-color"] as string : "#000000";
              return (
                <div key={layer.id}>
                  <span style={{ "backgroundColor": color }}></span>
                  {layer.friendlyName}
                </div>
              );
            }
            )
          }
        </div>
      </div>
      <div id="map" className="map h-100"></div>
    </div>
  );
}

export default Map;
