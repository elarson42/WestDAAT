import { HomePageTab } from '../pages/HomePage';
import WaterRightsTab from './WaterRightsTab';
import AggregationsTab from './AggregationsTab';
import SiteSpecificTab from './SiteSpecificTab';
import NldiTab from './NldiTab';

import './../styles/side-panel.scss';

interface SidePanelProps {
  currentTab: HomePageTab;
}

function SidePanel(props: SidePanelProps) {
  const tabs: Record<HomePageTab, JSX.Element> = {
    [HomePageTab.WaterRights]: <WaterRightsTab />,
    [HomePageTab.Aggregations]: <AggregationsTab />,
    [HomePageTab.SiteSpecific]: <SiteSpecificTab />,
    [HomePageTab.TempNldi]: <NldiTab />,
  }
  
  var tabComponent = tabs[props.currentTab];

  return (
    <div className="side-panel">
      <div className="map-info text-center p-2">
        19,241 Points of Diversions Displayed
      </div>
      <div className="p-3">
        {tabComponent}
      </div>
    </div>
  );
}

export default SidePanel;
