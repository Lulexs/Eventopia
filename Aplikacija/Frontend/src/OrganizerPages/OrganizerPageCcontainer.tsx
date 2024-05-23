import { Flex } from "@mantine/core";
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { Footer } from "../HomePage/Footer/Footer";
import { HeaderMegaMenu } from "../HomePage/HeaderMegaMenu/HeaderMegaMenu";
import { RootState } from "../store/store";
import OrganizerPage from "./OrganizerPage/OrganizerPage";
import classes from "./OrganizerPage/OrganizerPage.module.css";
import NewEvent from "./NewEvent/NewEvent";
import View from "./EventViewPages";
import ManageEvent from "./ManageEvent/ManageEvent";
import EventReviews from "./EventReviews/EventReviews";

export default function OrganizerPageContainer() {
  const loggedUser = useSelector((state: RootState) => state.auth);
  const navigate = useNavigate();

  const [view, setView] = useState<View>(View.Basic);
  const [eventId, setEventId] = useState<number>(-1);

  useEffect(() => {
    if (loggedUser.userType == "Unregistered") navigate("/");
  }, []);

  return (
    <Flex className={classes.mainMain}>
      <HeaderMegaMenu />
      {view == View.Basic && (
        <OrganizerPage
          setEventId={setEventId}
          showEvent={setView}
          user={loggedUser}
        />
      )}
      {view == View.NewEvent && (
        <NewEvent showEvent={setView} user={loggedUser} />
      )}
      {view == View.ManageEvent && (
        <ManageEvent eventId={eventId} showEvent={setView} user={loggedUser} />
      )}
      {view == View.PastEventDetails && (
        <EventReviews eventId={eventId} showEvent={setView} />
      )}
      <Footer />
    </Flex>
  );
}
