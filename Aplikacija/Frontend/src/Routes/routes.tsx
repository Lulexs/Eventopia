import { PathConstants } from "./pathConstants";
import HomePage from "../HomePage/HomePage";
import { Faq } from "../Faq/Faq";
import Privacy from "../Privacy/Privacy";
import ContactPage from "../Contact/ContactPage";
import { Page404 } from "../PageNotFound/Page404";
import ReservationContainer from "../Reservations/Reservation/ReservationContainer";
import { LoginPage } from "../Auth/LoginPage";
import RegisterPage from "../Auth/RegisterPage";
import VisitorProfileContainer from "../VisitorProfile/VisitorProfileContainer";
import OrganizerPageContainer from "../OrganizerPages/OrganizerPageCcontainer";

const routes = [
  { path: PathConstants.HOME, element: <HomePage /> },
  { path: PathConstants.FAQ, element: <Faq /> },
  { path: PathConstants.PRIVACY, element: <Privacy /> },
  { path: PathConstants.CONTACT, element: <ContactPage /> },
  { path: PathConstants.EVENTINFO, element: <ReservationContainer /> },
  { path: PathConstants.LOGIN, element: <LoginPage /> },
  { path: PathConstants.REGISTER, element: <RegisterPage /> },
  {
    path: PathConstants.VISITOR_PROFILE_PAGE,
    element: <VisitorProfileContainer />,
  },
  {
    path: PathConstants.ORGANIZER_PAGE,
    element: <OrganizerPageContainer />,
  },
  { path: PathConstants.PAGENOTFOUND, element: <Page404 /> },
];

export default routes;
