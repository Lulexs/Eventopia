import { TableInterface } from "./interfaces";
import TableFreeImage from "../../assets/table_free.png";
import TableNotFreeImage from "../../assets/table_not_free.png";
import { Button, CloseButton, Dialog, Group, Text } from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import { useRef } from "react";

export interface TableProps {
  item: TableInterface;
}

export default function Table({ item }: TableProps) {
  const [dialogOpened, { toggle, close }] = useDisclosure(false);
  const dialogTopLeft = useRef([20, 20]);
  return (
    <>
      <img
        key={item.id}
        style={{
          position: "absolute",
          top: item.top,
          left: item.left,
          zIndex: 5,
        }}
        height={`${item.height * item.heightFactor}%`}
        src={
          (item as TableInterface).reserved ? TableNotFreeImage : TableFreeImage
        }
        onClick={(e) => {
          e.stopPropagation();
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

        {item.reserved ? (
          <Group align="center" mb="xl">
            <Text size="sm" fw={300} miw="45px" c="red">
              TABLE IS ALREDY RESERVED
            </Text>
          </Group>
        ) : (
          <>
            <Group align="center" mb="xs">
              <Text size="sm" fw={300} miw="45px">
                Price per seat: {item.price}
              </Text>
            </Group>
            <Group align="center" mb="xl">
              <Text size="sm" fw={300} miw="45px">
                Number of seats: {item.numberOfSeats}
              </Text>
            </Group>
          </>
        )}

        <Button
          w="100%"
          onClick={(e) => {
            e.stopPropagation();
            close();
          }}
        >
          Make a reservation
        </Button>
      </Dialog>
    </>
  );
}
