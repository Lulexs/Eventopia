import { Text } from "@mantine/core";
import { useRef } from "react";
import { DndProvider } from "react-dnd";
import { HTML5Backend } from "react-dnd-html5-backend";
import Surface from "./Surface";
import styles from "./Drawer.module.css";
import { SpaceDataType } from "../Reservation/interfaces";

export interface DrawerProps {
  plan: SpaceDataType;
}

export default function Drawer(props: DrawerProps) {
  const exportPlanFunctionRef = useRef<Function | null>(null);

  function setNewExportPlanFunction(exportFunction: Function) {
    exportPlanFunctionRef.current = exportFunction;
  }

  return (
    <DndProvider backend={HTML5Backend}>
      <div
        className={styles.mainContentContainer}
        style={{
          zIndex: 1,
          overflow: "auto",
        }}
      >
        <h1 style={{ color: "hsla(0, 0%, 57%, 0.7)", marginBottom: "0" }}>
          Edit floor plan
        </h1>
        <Text m="0" style={{ color: "hsla(0, 0%, 57%, 0.7)" }}>
          Please keep browser open in full screen mode
        </Text>
        <Surface
          spacePlan={props.plan}
          changeExportFunctionRef={setNewExportPlanFunction}
        />
      </div>
    </DndProvider>
  );
}
