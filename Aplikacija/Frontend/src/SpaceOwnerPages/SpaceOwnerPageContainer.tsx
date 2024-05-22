import { Flex } from "@mantine/core";
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { Footer } from "../HomePage/Footer/Footer";
import { HeaderMegaMenu } from "../HomePage/HeaderMegaMenu/HeaderMegaMenu";
import { RootState } from "../store/store";
import SpaceOwnerPage from "./SpaceOwnerPage/SpaceOwnerPage";
import classes from "./SpaceOwnerPage/SpaceOwnerPage.module.css";
import View from "./SpaceViewPages";
import Drawer from "../Reservations/Drawer/Drawer";

export default function SpaceOwnerPageContainer() {
  const loggedUser = useSelector((state: RootState) => state.auth);
  const navigate = useNavigate();

  const [view, setView] = useState<View>(View.Basic);

  useEffect(() => {
    if (loggedUser.userType == "Unregistered") navigate("/");
  }, []);

  return (
    <Flex
      className={classes.mainMain}
      style={{ height: view == View.Basic ? "100vh" : "unset" }}
    >
      <HeaderMegaMenu />
      {view == View.Basic && (
        <SpaceOwnerPage showSpace={setView} user={loggedUser} />
      )}
      {view == View.NewSpace && (
        <Drawer
          onSubmit={() => {
            console.log("Here");
          }}
          onCancel={() => setView(View.Basic)}
        />
      )}
      <Footer />
    </Flex>
  );
}
