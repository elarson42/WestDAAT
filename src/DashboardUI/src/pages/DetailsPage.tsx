import { useParams } from 'react-router-dom';
import Button from 'react-bootstrap/Button';
import '../styles/detail-page.scss';
import WaterRightMap from '../components/WaterRightMap';
import WaterRightProperties from '../components/WaterRightProperties';
import WaterRightTabs from '../components/WaterRightTabs';
import SiteMap from '../components/SiteMap';
import SiteProperties from '../components/SiteProperties';
import SiteTabs from '../components/SiteTabs';
import useProgressIndicator from '../hooks/useProgressIndicator';
import { useMemo } from 'react';
import { useWaterRightDetails, useWaterRightSiteInfoList, useWaterRightSiteLocations, useWaterRightSourceInfoList } from '../hooks/useWaterRightQuery';

interface detailPageProps {
  detailType: "site" | "right";
}

function DetailsPage(props: detailPageProps) {

  let { id: idParam } = useParams();

  const id = useMemo(() => idParam ? idParam : "", [idParam]);
  const isSiteDetail = useMemo(() => props.detailType === "site", [props.detailType]);

  const detailsComponent = isSiteDetail ? <SiteProperties /> : <WaterRightProperties waterRightId={id} />;
  const mapComponent = isSiteDetail ? <SiteMap /> : <WaterRightMap waterRightId={id} />;
  const tabComponent = isSiteDetail ? <SiteTabs /> : <WaterRightTabs waterRightId={id} />;

  const { isFetching: isFetchingDetails } = useWaterRightDetails(+id);
  const { isFetching: isFetchingSources } = useWaterRightSourceInfoList(+id);
  const { isFetching: isFetchingSites } = useWaterRightSiteInfoList(+id);
  const { isFetching: isFetchingMap } = useWaterRightSiteLocations(+id);
  useProgressIndicator([!isFetchingDetails, !isFetchingSources, !isFetchingSites, !isFetchingMap], "Retrieving Water Right Data");

  return (
    <>
      <div className="detail-page d-flex flex-column">
        <div className='d-flex flex-row align-items-center title-header'>
          <h3 className='d-flex me-auto'>WaDE {isSiteDetail ? "Site" : "Right"} ID: {id} </h3>
          <div className='p-2'>
            <Button variant="secondary" size="lg">Return to Map</Button>
          </div>
          <div className='p-2'>
            <Button size="lg">Download Data</Button>
          </div>
        </div>
        <div className='row properties-row'>
          <div className='col-7'>
            {detailsComponent}
          </div>
          <div className='col-5'>
            {mapComponent}
          </div>
        </div>
        <div className='flex-fill'>
          {tabComponent}
        </div>
      </div>
    </>
  );
}

export default DetailsPage;
