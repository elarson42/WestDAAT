import { IPublicClientApplication } from "@azure/msal-browser";
import { AuthenticatedTemplate, UnauthenticatedTemplate, useMsal } from "@azure/msal-react";
import { loginRequest } from "../authConfig";

import Container from 'react-bootstrap/Container';
import Button from 'react-bootstrap/Button';
import Navbar from 'react-bootstrap/Navbar';
import Nav from 'react-bootstrap/Nav';
import Offcanvas from 'react-bootstrap/Offcanvas';

import { HomePageTab } from '../pages/HomePage';
import '../styles/navbar.scss';
import { useState } from 'react';

interface SiteNavbarProps {
  currentTab: HomePageTab;
  onTabClick: (tab: HomePageTab) => void;
  showContactModal(show: boolean): void;
}

function handleLogin(msalClientApplication: IPublicClientApplication) {
  msalClientApplication.loginPopup(loginRequest).catch(e => {
    console.error(e);
  });
}

function handleLogout(msalClientApplication: IPublicClientApplication) {
  msalClientApplication.logoutPopup().catch(e => {
    console.error(e);
  });
}

function SiteNavbar(props: SiteNavbarProps) {
  const msalClientApplication = useMsal().instance;
  const [showHamburgerMenu, setShowHamburgerMenu] = useState(false);

  const handleClose = () => setShowHamburgerMenu(false);
  const handleShow = () => setShowHamburgerMenu(true);

  return (
    <div>
      <Navbar variant="dark" expand={false}>
        <Container fluid>
          <div className="d-flex">
            <Button variant="link" onClick={handleShow}>
              <span className="navbar-toggler-icon"></span>
            </Button>

            <Nav className="mx-2">
              <Nav.Link target="_blank" href="https://westernstateswater.org/">
                <img alt="Wade Logo" src="/logo32x32.png" />
                Western States Water Council
              </Nav.Link>
            </Nav>
          </div>

          <Nav className="mx-2">
            <Nav.Link href="/">Western States Water Data Access and Analysis Tool (WestDAAT)</Nav.Link>
          </Nav>

          <Nav className="mx-2">
          <UnauthenticatedTemplate>
            <Nav.Link onClick={() => handleLogin(msalClientApplication)}>              
              Log In
            </Nav.Link>
          </UnauthenticatedTemplate>
          <AuthenticatedTemplate>
            <Nav.Link onClick={() => handleLogout(msalClientApplication)}>              
              Log Out
            </Nav.Link>
          </AuthenticatedTemplate>
          </Nav>
        </Container>
      </Navbar>

      <Navbar bg="light" className="p-0 second-nav">
        <Container fluid className="p-0">
          <Nav>
            {(Object.values(HomePageTab)).map(tab =>
              <Nav.Link onClick={() => props.onTabClick(tab)} className={`py-3 px-4 ${props.currentTab === tab ? 'active-tab' : ''}`} key={tab}>
                {tab}
              </Nav.Link>
            )}
          </Nav>

          <div className="mx-2">
            <Button className="ms-1">View Table Results</Button>
            <Button className="ms-1">Download Data</Button>
          </div>
        </Container>
      </Navbar>

      <Offcanvas show={showHamburgerMenu} onHide={handleClose} className="ham-menu">
        <Offcanvas.Header closeButton>
        </Offcanvas.Header>
        <Offcanvas.Body>
          <Nav defaultActiveKey="/" className="flex-column gap(10px)">
            <Nav.Link target="_blank" href="https://westernstateswater.org/wade/about ">About</Nav.Link>
            <Nav.Link target="_blank" href="https://westernstateswater.org/wade/water-rights-data">Water Rights Data</Nav.Link>
            <Nav.Link target="_blank" href="https://westernstateswater.org/wade/Aggregate-Water-Data">Aggregate Water Use Data</Nav.Link>
            <Nav.Link onClick={() => props.showContactModal(true)}>Contact Us</Nav.Link>
            <Nav.Link target="_blank" href="https://westernstateswater.org/wade/terms-of-service">Terms and Conditions</Nav.Link>
          </Nav>
        </Offcanvas.Body>
      </Offcanvas>
    </div>
  );
}

export default SiteNavbar;
