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
} from "@mantine/core";
import classes from "./NewEvent.module.css";
import EventBgImage from "../../assets/event_listing_bg_op.png";
import { AuthState } from "../../store/features/auth";
import View from "../EventViewPages";
import { DateInput } from "@mantine/dates";

export interface NewEventProps {
  user: AuthState;
  showEvent: React.Dispatch<React.SetStateAction<View>>;
}

export default function NewEvent(props: NewEventProps) {
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
              <TextInput label="Event name"></TextInput>
              <Textarea label="Description" autosize minRows={5} />
            </Stack>
            <Stack w="50%">
              <DateInput label="Date" />
              <TextInput label="Time" />
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
              },
            }}
            mb={10}
          >
            <Group w="100%" justify="center">
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
          </Fieldset>
        </Flex>
      </Flex>
    </>
  );
}
