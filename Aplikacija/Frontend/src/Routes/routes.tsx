import { PathConstants } from "./pathConstants";
import HomePage from "../HomePage/HomePage";
import { Faq } from "../Faq/Faq";
import Privacy from "../Privacy/Privacy";
import ContactPage from "../Contact/ContactPage";
import { Page404 } from "../PageNotFound/Page404";
import ReservationContainer from "../Reservations/Reservation/ReservationContainer";

const routes = [
  { path: PathConstants.HOME, element: <HomePage /> },
  { path: PathConstants.FAQ, element: <Faq /> },
  { path: PathConstants.PRIVACY, element: <Privacy /> },
  { path: PathConstants.CONTACT, element: <ContactPage /> },
  { path: PathConstants.EVENTINFO, element: <ReservationContainer /> },
  { path: PathConstants.PAGENOTFOUND, element: <Page404 /> },
];

export default routes;
