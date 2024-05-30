import {
  Button,
  Fieldset,
  Flex,
  Group,
  InputLabel,
  Title,
  Stack,
  TextInput,
  Textarea,
  Select,
  NumberInput,
  FileInput,
  TagsInput,
  Table,
  Checkbox,
} from "@mantine/core";
import classes from "./NewEvent.module.css";
import EventBgImage from "../../assets/event_listing_bg_op.png";
import { AuthState } from "../../store/features/auth";
import View from "../EventViewPages";
import { DateInput } from "@mantine/dates";
import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import axios from "axios";
import { SpaceDataType } from "../../Reservations/Reservation/interfaces";

export interface NewEventProps {
  user: AuthState;
  showEvent: React.Dispatch<React.SetStateAction<View>>;
}

export default function NewEvent(props: NewEventProps) {
  const [testTags, setTestTags] = useState<string[]>([]);
  const [selectedSpaceId, setSelectedSpaceId] = useState<number | string>(-1);

  const {
    isLoading,
    data: spaces,
    isError,
  } = useQuery<SpaceDataType[]>({
    queryKey: ["events"],
    queryFn: async () => {
      return await axios
        .get(`${import.meta.env.VITE_JSON_SERVER}/spaces`)
        .then((resp) => resp.data);
    },
  });

  return (
    <>
      <Flex
        className={classes.mainContentFlex}
        styles={{
          root: {
            backgroundImage: `url(${EventBgImage})`,
            backgroundRepeat: "repeat",
          },
        }}
      >
        <Group style={{ width: "100%" }}>
          <Button
            onClick={(event) => {
              event.stopPropagation();
              props.showEvent(View.Basic);
            }}
          >
            Go back
          </Button>
          <Title c="#5a5959">Scheduele new event</Title>
        </Group>

        <Flex
          w="100%"
          h="100%"
          mt="20"
          align="center"
          justify="center"
          className={classes.subTitleContainer}
        >
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
            <Stack w="50%">
              <TextInput
                required
                label="Event name"
                placeholder="Rambo"
              ></TextInput>
              <Textarea
                required
                placeholder="Write something about the event..."
                label="Description"
                autosize
                minRows={5}
              />
              <TagsInput
                miw="100%"
                label="Press Enter to submit a tag"
                placeholder="Enter tag"
                value={testTags}
                onChange={setTestTags}
                styles={{
                  input: {
                    overflowY: "scroll",
                    height: "7rem",
                  },
                }}
              />
            </Stack>
            <Stack w="50%">
              <DateInput required placeholder={`May 10, 2024`} label="Date" />
              <TextInput required placeholder="Military time" label="Time" />
              <FileInput
                required
                placeholder="Image to be displayed"
                label="Promo image"
              />
              <TextInput placeholder="Optional video" label="Promo video" />
              <div
                style={{
                  width: "100%",
                  display: "flex",
                  flexDirection: "column",
                  lineHeight: "var(--mantine-line-height)",
                  marginTop: "8px",
                }}
              >
                <InputLabel className="mantine-TextInput-label">
                  Schedule
                </InputLabel>
                <Button>Schedule event</Button>
              </div>
            </Stack>
          </Fieldset>

          <Fieldset
            legend="Query spaces"
            w="50%"
            h="fit-content"
            fz="xl"
            styles={{
              root: {
                display: "flex",
                justifyContent: "space-between",
                gap: "10px",
                flexDirection: "column",
                alignItems: "center",
              },
            }}
            mb={10}
          >
            <Group w="100%" justify="center" mb={10}>
              <Select label="Location" />
              <NumberInput
                label="Capacity"
                inputMode="numeric"
                inputContainer={(children) => (
                  <Group align="flex-start">
                    {children}
                    <Button>Query</Button>
                  </Group>
                )}
              />
            </Group>
            <Checkbox
              checked={selectedSpaceId != -1}
              label="Selected space?"
              disabled
            />
            {isLoading || isError ? (
              <div className={classes.controls}>
                <div className={classes.ldsRing}>
                  <div></div>
                  <div></div>
                  <div></div>
                  <div></div>
                </div>
              </div>
            ) : (
              <Table>
                <Table.Thead>
                  <Table.Tr>
                    <Table.Th>City</Table.Th>
                    <Table.Th>Country</Table.Th>
                    <Table.Th>Address</Table.Th>
                    <Table.Th>Siting capacity</Table.Th>
                    <Table.Th></Table.Th>
                  </Table.Tr>
                </Table.Thead>
                <Table.Tbody>
                  {spaces?.map((space, idx) => (
                    <Table.Tr key={idx}>
                      <Table.Td>{space.city}</Table.Td>
                      <Table.Td>{space.country}</Table.Td>
                      <Table.Td>{space.address}</Table.Td>
                      <Table.Td>{50}</Table.Td>
                      <Table.Td>
                        <Button
                          onClick={(event) => {
                            event.stopPropagation();
                            setSelectedSpaceId(space.id);
                          }}
                        >
                          Select
                        </Button>
                      </Table.Td>
                    </Table.Tr>
                  ))}
                </Table.Tbody>
              </Table>
            )}
          </Fieldset>
        </Flex>
      </Flex>
    </>
  );
}
