import { Fieldset, Flex, Stack, TextInput } from "@mantine/core";
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
import axios from "axios";
import MapWithInput from "../Auth/Utils/MapInput";
import { LatLng } from "leaflet";

export default function SpaceOwnerPageContainer() {
  const [position, setPosition] = useState<LatLng>(new LatLng(51.505, -0.09));

  const loggedUser = useSelector((state: RootState) => state.auth);
  const navigate = useNavigate();

  const [view, setView] = useState<View>(View.Basic);

  useEffect(() => {
    if (loggedUser.userType != "Space owner") navigate("/");
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
        <Flex direction="column" justify="center" align="center">
          <Drawer
            onSubmit={(spaceObject: any) => {
              axios
                .post(`${import.meta.env.VITE_JSON_SERVER}/spaces`, spaceObject)
                .then((_) => setView(View.Basic))
                .catch((_) =>
                  alert("There was an internal error, please try again latter")
                );
            }}
            onCancel={() => setView(View.Basic)}
          />
          <Fieldset
            legend="Basic information"
            w="50%"
            h="fit-content"
            fz="xl"
            styles={{
              root: {
                display: "flex",
                justifyContent: "space-between",
                gap: "10px",
              },
            }}
            mb={10}
          >
            <Stack w="50%" justify="center">
              <TextInput label="City"></TextInput>
              <TextInput label="Country"></TextInput>
              <TextInput label="Address"></TextInput>
            </Stack>
            <Stack w="50%" justify="center">
              <TextInput
                required
                label="Select location address"
                disabled={true}
                value={`${position.lat} ${position.lng}`}
              ></TextInput>
              <MapWithInput position={position} setPosition={setPosition} />
            </Stack>
          </Fieldset>
        </Flex>
      )}
      <Footer />
    </Flex>
  );
}
