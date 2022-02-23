import mapboxgl from 'mapbox-gl';
import { useEffect, useState } from 'react';
import AboutModal from '../components/AboutModal';
import ContactModal from '../components/ContactModal';
import SidePanel from '../components/SidePanel';
import SiteFooter from '../components/SiteFooter';
import SiteNavbar from '../components/SiteNavbar';
import TermsModal from '../components/TermsModal';

import '../styles/home-page.scss';

export enum HomePageTab {
  WaterRights = "Water Rights",
  Aggregations = "Aggregations",
  SiteSpecific = "Site Specific"
}

function HomePage() {

  const [currentTab, setCurrentTab] = useState(HomePageTab.WaterRights);

  const [shouldAboutShowModal, setShowAboutModal] = useState(false);
  const [showContactModal, setShowContactModal] = useState(false);
  const [showTermsModal, setShowTermsModal] = useState(false);

  useEffect(() => {
    mapboxgl.accessToken = process.env.REACT_APP_MAPBOX_ACCESSTOKEN || "";
    new mapboxgl.Map({
      container: 'map', // container ID
      style: 'mapbox://styles/mapbox/streets-v11', // style URL
      center: [-74.5, 40], // starting position [lng, lat]
      zoom: 9 // starting zoom
    });
  });

  const handleTabClick = (tab: HomePageTab) =>{
    setCurrentTab(tab);
  }

  const shouldShowAboutModal = (show: boolean) => {
    setShowAboutModal(show);
  }

  const shouldShowContactModal = (show: boolean) => {
    setShowContactModal(show);
  }

  const shouldShowTermsModal = (show: boolean) => {
    setShowTermsModal(show);
  }

  return (
    <div className="home-page d-flex flex-column">
      <SiteNavbar onTabClick={handleTabClick} currentTab={currentTab} showAboutModal={shouldShowAboutModal} showContactModal={shouldShowContactModal} showTermsModal={shouldShowTermsModal} />
      <div className="d-flex flex-grow-1">
        <SidePanel currentTab={currentTab} />

        <div id="map" className="map flex-grow-1">
          <div className="legend mapboxgl-ctrl-bottom-right m-4">Legend</div>
        </div>
      </div>
      <SiteFooter />

      <AboutModal show={shouldAboutShowModal} setShow={shouldShowAboutModal} />
      <ContactModal show={showContactModal} setShow={shouldShowContactModal} />
      <TermsModal show={showTermsModal} setShow={shouldShowTermsModal} />
    </div>
  );
}

export default HomePage;
