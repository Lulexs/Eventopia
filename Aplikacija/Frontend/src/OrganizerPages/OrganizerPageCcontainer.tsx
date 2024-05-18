import { Flex } from "@mantine/core";
import { useEffect } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { Footer } from "../HomePage/Footer/Footer";
import { HeaderMegaMenu } from "../HomePage/HeaderMegaMenu/HeaderMegaMenu";
import { RootState } from "../store/store";
import OrganizerPage from "./OrganizerPage";
import classes from "./OrganizerPage.module.css";

export default function OrganizerPageContainer() {
  const loggedUser = useSelector((state: RootState) => state.auth);
  const navigate = useNavigate();

  useEffect(() => {
    if (loggedUser.userType == "Unregistered") navigate("/");
  }, []);

  return (
    <Flex className={classes.mainMain}>
      <HeaderMegaMenu />
      <OrganizerPage user={loggedUser} />
      <Footer />
    </Flex>
  );
}
