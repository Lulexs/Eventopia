import { Flex } from "@mantine/core";
import { Footer } from "../HomePage/Footer/Footer";
import { HeaderMegaMenu } from "../HomePage/HeaderMegaMenu/HeaderMegaMenu";
import { useState } from "react";
import classes from "./RegisterPage.module.css";
import { RegisterPage1 } from "./RegisterPage1";
import Drawer from "../Reservations/Drawer/Drawer";

export default function RegisterPage() {
  const [drawer, setDrawer] = useState(false);
  return (
    <>
      <HeaderMegaMenu />
      {drawer ? (
        <Drawer
          onSubmit={() => setDrawer(false)}
          onCancel={() => setDrawer(false)}
        />
      ) : (
        <Flex
          h="100%"
          w="100%"
          align="center"
          justify="center"
          flex={1}
          className={classes.wrapper}
        >
          <RegisterPage1 enterDrawer={() => setDrawer(true)} />
        </Flex>
      )}
      <Footer />
    </>
  );
}
