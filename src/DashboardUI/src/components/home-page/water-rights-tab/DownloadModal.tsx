import { useEffect, useState } from 'react';
import Button from 'react-bootstrap/Button';
import moment from 'moment';
import Modal, { ModalProps } from 'react-bootstrap/Modal';
import { useAppContext } from '../../../contexts/AppProvider';
import { SignIn } from "../../SignIn";
import { WaterRightsSearchCriteria } from '../../../data-contracts/WaterRightsSearchCriteria';
import { WaterRightsFilters } from './Provider';
import { ProgressBar } from 'react-bootstrap';
import { useWaterRightsDownload } from '../../../hooks/queries';
import { useHomePageContext } from '../Provider';

function DownloadWaterRights(props: {
  searchCriteria: WaterRightsSearchCriteria | null,
  setTitle:(title: JSX.Element | null) => void,
  setError:(error: JSX.Element | null) => void,
  setIsFetching: (isFetching: boolean) => void,
  setIsFetched: (isFetched: boolean) => void}){

  const {isFetching, isError, isFetched, error} =  useWaterRightsDownload(props.searchCriteria);
  const { setTitle, setError, setIsFetching, setIsFetched } = props;

  useEffect(() => {
    if (isError) {
      if(error instanceof Error && error.message === 'Download limit exceeded.') {
        setError(<ErrorMessageTooMuchData />);
      } else{
        setError(<ErrorMessageGeneric />);
      }
    }else {
      setError(null);
    }
  }, [isError, error, setError]);

  useEffect(() => {
    if (isError && error instanceof Error && error.message === 'Download limit exceeded.') {
      setTitle(<ModalTitleDownloadLimit/>)
    }
  }, [isError, error, setTitle]);

  useEffect(() => {
    if (!isFetching){
      setIsFetching(isFetching);
    }
  }, [isFetching, setIsFetching]);

  useEffect(() => {
    if (isFetched){
      setIsFetched(isFetched);
    }
  }, [isFetched, setIsFetched]);

  return (<div><p>We are preparing your download, this might take some time
                </p>
                <ProgressBar animated now={100} /></div>
                );
}

interface DownloadModalProps extends ModalProps {
  filters: WaterRightsFilters,
  nldiIds: string[]
}
function DownloadModal(props: DownloadModalProps) {
  const {filters, nldiIds} = props;
  const { authenticationContext: {isAuthenticated} } = useAppContext();
  const { showDownloadModal, setShowDownloadModal } = useHomePageContext();
  
  const [ searchCriteria, setSearchCriteria] = useState<WaterRightsSearchCriteria | null>(null);
  const [ isFetching, setIsFetching ] = useState<boolean>(false);
  const [ isFetched, setIsFetched ] = useState<boolean>(false);
  const [ downloadError, setDownloadError ] = useState<JSX.Element | null>(null);
  const [ modalTitle, setModalTitle ] = useState<JSX.Element | null>(<ModalTitleGeneric/>);

  const close = () => {
    setShowDownloadModal(false);
    setModalTitle(<ModalTitleGeneric/>);
    setDownloadError(null);
  }

  const download = async () => 
  {
    setSearchFilterValues();
    setIsFetching(true);
  }

  useEffect(() => {
    if (isFetched && !downloadError){
      setShowDownloadModal(false);
      setIsFetched(false);
    }
  }, [isFetched, setIsFetched, downloadError, setShowDownloadModal])

  // technical debt, move this to a shared space and also update TableView to use this share space. to clean copy pasted code
  const setSearchFilterValues = () => {
    setSearchCriteria({
      beneficialUses: filters.beneficialUseNames,
      filterGeometry: filters.polylines?.map(p => JSON.stringify(p.geometry)),
      exemptofVolumeFlowPriority: filters.includeExempt,
      minimumFlow: filters.minFlow,
      maximumFlow: filters.maxFlow,
      minimumVolume: filters.minVolume,
      maximumVolume: filters.maxVolume,
      podOrPou: filters.podPou,
      minimumPriorityDate: filters.minPriorityDate ? moment.unix(filters.minPriorityDate).toDate() : undefined,
      maximumPriorityDate: filters.maxPriorityDate ? moment.unix(filters.maxPriorityDate).toDate() : undefined,
      ownerClassifications: filters.ownerClassifications,
      waterSourceTypes: filters.waterSourceTypes,
      riverBasinNames: filters.riverBasinNames,
      allocationOwner: filters.allocationOwner,
      states: filters.states,
      filterUrl: filters !== null ? window.location.href : undefined,
      wadeSitesUuids: nldiIds
    });
  }

  return (
    <Modal show={showDownloadModal} aria-labelledby="contained-modal-title-vcenter" centered>
      <Modal.Header closeButton onClick={close}>
        <Modal.Title id="contained-modal-title-vcenter">
        { !isAuthenticated && <label>Login for Download Access</label> }
        { isAuthenticated && modalTitle }
        </Modal.Title>
      </Modal.Header>
      <Modal.Body>
      { !isAuthenticated && <p>
          Sign up <strong>completely free</strong> or login for download access of up to 100,000
          water rights points.
        </p>}
        { downloadError }
        {/* display donwload message to continue with download if user is authenticated */}
        { isAuthenticated && !isFetching && !downloadError &&
          <p>Download access limited up to a maximum of 100,000 water rights points. Click Download to continue</p>}
        {/* display modal with is fetching info and progress bar with it */}
        { isAuthenticated && isFetching && !downloadError &&
        <DownloadWaterRights
          searchCriteria={searchCriteria}
          setTitle={setModalTitle}
          setError={setDownloadError}
          setIsFetching={setIsFetching}
          setIsFetched={setIsFetched}/>}
      </Modal.Body>
          <Modal.Footer style={{ justifyContent: 'end'}}>
      {!downloadError && <Button className="btn btn-secondary" onClick={close}>Cancel</Button>}
      {!isAuthenticated && <Button className="sign-in-button" ><SignIn /></Button>}
      {isAuthenticated && !isFetching && !downloadError && <Button onClick={download}>Download</Button>}
      {downloadError && <Button className="btn btn-secondary" onClick={close}>Okay</Button>}      
      </Modal.Footer>
    </Modal>
  );
}

function ModalTitleGeneric() {
  return (
    <label>Download</label>
  );
}

function ModalTitleDownloadLimit() {
  return (
    <label>Download Limit</label>
  );
}

function ErrorMessageGeneric() {
  return (
    <p>An error occurred while attempting to download the data. Please adjust your filters to reduce the data-set and try again.</p>
  );
}

function ErrorMessageTooMuchData() {
  return (
    <p>You tried downloading a water rights dataset containing more than the supported limit for software efficiency reasons.<br/><br/>
    Please adjust your filters to reduce the dataset size to less than 100,000 water rights and try the download again.<br/><br/>
    If not, don't hesitate to get in touch with the WaDE Team for a larger dataset request.</p>
  );
}

export default DownloadModal;
