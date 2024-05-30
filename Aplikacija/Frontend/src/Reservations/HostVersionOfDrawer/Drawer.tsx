import { Flex, Text } from "@mantine/core";
import { useRef } from "react";
import { DndProvider } from "react-dnd";
import { HTML5Backend } from "react-dnd-html5-backend";
import Surface from "./Surface";
import styles from "./Drawer.module.css";
import { SpaceDataType } from "../Reservation/interfaces";

export interface DrawerProps {
  plan: SpaceDataType;
}

export default function Drawer(props: SpaceDataType) {
  const exportPlanFunctionRef = useRef<Function | null>(null);

  function setNewExportPlanFunction(exportFunction: Function) {
    exportPlanFunctionRef.current = exportFunction;
  }

  return (
    <DndProvider backend={HTML5Backend}>
      <Flex
        align="center"
        justify="center"
        direction="column"
        w="100%"
        h="100vh"
        className={styles.gradiental}
      >
        <h1 style={{ color: "hsla(0, 0%, 57%, 0.7)", marginBottom: "0" }}>
          Draw floor plan
        </h1>
        <Text m="0" style={{ color: "hsla(0, 0%, 57%, 0.7)" }}>
          Please keep browser open in full screen mode
        </Text>
        <Surface
          spacePlan={props}
          changeExportFunctionRef={setNewExportPlanFunction}
        />
      </Flex>
    </DndProvider>
  );
}
