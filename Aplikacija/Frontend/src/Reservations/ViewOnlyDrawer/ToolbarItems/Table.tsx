import { CSSProperties, FC, useRef, useState } from "react";
import TableImage from "../../../assets/table.png";
import { useDisclosure } from "@mantine/hooks";
import {
  Button,
  CloseButton,
  Dialog,
  Group,
  Text,
  TextInput,
} from "@mantine/core";

interface TableInterface {
  id: string;
  top: number;
  left: number;
  height: number;
  numberOfSeats: number;
}

const TableStyle: CSSProperties = {
  position: "absolute",
  cursor: "move",
  margin: 0,
};

export const Table: FC<TableInterface> = ({ id, top, left, height }) => {
  const [dialogOpened, { toggle, close }] = useDisclosure(false);
  const dialogTopLeft = useRef([20, 20]);
  const numberOfSeats = useRef(4);
  const [dialogInputFieldVal, setDialogInputFieldVal] = useState("");
  const [dialogInputFieldVal1, setDialogInputFieldVal1] = useState("");
  return (
    <>
      <img
        src={TableImage}
        alt="TABLE"
        id={id}
        style={{
          ...TableStyle,
          top,
          left,
          maxHeight: "20%",
          height: `${height * 0.2}%`,
        }}
        data-testid="table"
        onClick={(e) => {
          e.stopPropagation();
        }}
        onContextMenu={(e) => {
          e.preventDefault();
          dialogTopLeft.current = [e.clientY, e.clientX];
          toggle();
        }}
      />
      <Dialog
        opened={dialogOpened}
        withCloseButton={false}
        onClose={close}
        size="md"
        radius="md"
        position={{
          top: dialogTopLeft.current[0],
          left: dialogTopLeft.current[1],
        }}
        onClick={(event) => event.stopPropagation()}
      >
        <Group mb="md" align="center">
          <Text size="sm" fw={300} flex={1}>
            Information about this table
          </Text>
          <CloseButton
            onClick={(event) => {
              event.stopPropagation();
              close();
            }}
          />
        </Group>

        <Group align="center" mb="md">
          <Text size="sm" fw={300} miw="45px">
            seats:{" "}
          </Text>
          <TextInput
            disabled
            placeholder="Number of seats..."
            style={{ flex: 1 }}
            value={dialogInputFieldVal}
            onChange={(event) =>
              setDialogInputFieldVal(
                event.currentTarget.value
                  .split("")
                  .filter((c) => c >= "0" && c <= "9")
                  .join("")
              )
            }
          />
        </Group>
        <Group align="center" mb="md">
          <Text size="sm" fw={300} miw="45px">
            Price per seat:{" "}
          </Text>
          <TextInput
            disabled
            placeholder="Price per seats..."
            style={{ flex: 1 }}
            value={dialogInputFieldVal1}
            onChange={(event) =>
              setDialogInputFieldVal1(
                event.currentTarget.value
                  .split("")
                  .filter((c) => c >= "0" && c <= "9")
                  .join("")
              )
            }
          />
        </Group>
        <Button
          w="100%"
          onClick={(e) => {
            e.stopPropagation();
            numberOfSeats.current = Number.parseInt(dialogInputFieldVal);
            close();
          }}
        >
          Save table Information
        </Button>
      </Dialog>
    </>
  );
};
