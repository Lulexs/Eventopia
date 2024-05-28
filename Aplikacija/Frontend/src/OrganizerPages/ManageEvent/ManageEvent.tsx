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
} from "@mantine/core";
import classes from "./ManageEvent.module.css";
import EventBgImage from "../../assets/event_listing_bg_op.png";
import { AuthState } from "../../store/features/auth";
import View from "../EventViewPages";
import { DateInput } from "@mantine/dates";
import { StatsCard } from "../StatsCard";
import { useState } from "react";

export interface ManageEventProps {
  user: AuthState;
  showEvent: React.Dispatch<React.SetStateAction<View>>;
  eventId: number;
}

export default function ManageEvent(props: ManageEventProps) {
  const [testTags, setTestTags] = useState<string[]>([
    "Rock",
    "Heavy metal",
    "Saban Saulic",
    "film",
    "comics",
  ]);
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
          <Title c="#5a5959">Manage event</Title>
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
              <TextInput label="Event name"></TextInput>
              <Textarea label="Description" autosize minRows={5} />
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
              <DateInput label="Date" />
              <TextInput label="Time" />
              <FileInput label="Promo image" />
              <TextInput label="Promo video" />
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

          <Flex w="50%" direction="column">
            <Fieldset
              legend="Space information"
              w="100%"
              h="fit-content"
              fz="xl"
              styles={{
                root: {
                  display: "flex",
                  justifyContent: "space-between",
                  gap: "10px",
                  flexDirection: "column",
                },
              }}
              mb={10}
            >
              <Group w="100%" justify="center">
                <Select label="Location" disabled={true} />
                <NumberInput
                  disabled={true}
                  label="Capacity"
                  inputMode="numeric"
                />
              </Group>
              <Group w="100%" justify="center">
                <Button>Change space layout</Button>
              </Group>
            </Fieldset>

            <Fieldset
              legend="Statistics"
              w="100%"
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
              <StatsCard title="Reserved tables" current={15} />
              <StatsCard title="Total earning" current={225} />
            </Fieldset>
          </Flex>
        </Flex>
      </Flex>
    </>
  );
}
