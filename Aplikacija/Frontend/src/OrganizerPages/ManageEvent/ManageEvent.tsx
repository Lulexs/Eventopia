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
import { ChangeEventDto, EventDto } from "../interfaces";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import axios from "../../../axiosconfig";
import { useForm } from "@mantine/form";
import { CustomStatsCard } from "../../VisitorProfile/CustomStatsCard";

export interface ManageEventProps {
  user: AuthState;
  showEvent: React.Dispatch<React.SetStateAction<View>>;
  eventId: number;
}

export default function ManageEvent(props: ManageEventProps) {
  const [tags, setTags] = useState<string[]>([]);
  const [location, setLocation] = useState<string>(""); 
  const [address, setAddress] = useState<string>(""); 
  const [capacity, setCapacity] = useState<number>(0);
  const [reservedTables, setReservedTables] = useState<number>(0);
  const [maxTables, setMaxTables] = useState<number>(0);
  const [totalEarnings, setTotalEarnings] = useState<number>(0);

  const queryClient = useQueryClient();

  const {
    data: event,
  } = useQuery<EventDto>({
    queryKey: ["event_preview"],
    queryFn: async () => {
      return await axios
        .get(`${import.meta.env.VITE_DB_SERVER}/Host/getEventDetails/${props.eventId}`)
        .then((resp) => {
          setTags(resp.data.tags);
          setLocation(resp.data.location);
          setAddress(resp.data.address);
          setCapacity(resp.data.capacity);
          setReservedTables(resp.data.reservedTables);
          setMaxTables(resp.data.maxTables);
          setTotalEarnings(resp.data.totalEarnings);
          return resp.data;
        })
        .catch((err) => {
          console.error(err);
          alert(err.resp.data);
          return null;
        });
    },
  });

  const cancelEvent = async (eventId: number) => {
    try {
      if (confirm("Are you sure you want to cancel this event?")) {
        await axios.delete(
          `${
            import.meta.env.VITE_DB_SERVER
          }/Host/cancelEvent/${eventId}`
        );
        queryClient.invalidateQueries({ queryKey: ["incoming_events"] });
        queryClient.invalidateQueries({ queryKey: ["past_events"] });
      }
    } catch (err: any) {
      if (Array.isArray(err.response.data) && err.response.data.length > 0) {
        alert(err.response.data[0].description);
      } else {
        alert(err.response.data);
      }
      console.error(err);
    }
  };

  const changeEventForm = useForm({
    mode: "controlled",
    initialValues: {
      eventName: event?.eventName,
      description: event?.description,
      date: new Date(event?.date ?? Date.now()),
      tags: event?.tags,
      time: event?.time,
      image: null,
      video: event?.video,
    },

    validate: {
      eventName: (value) =>
        value && value.length > 0 ? null : "Empty event name field",
      description: (value) =>
        value && value.length > 0 ? null : "Empty description field",
      time: (value) =>
        /^(?:[01]\d|2[0-3]):[0-5]\d$/.test(value ?? "") ? null : "Wrong time format",
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
          <Title c="#5a5959">Manage event</Title>
          <Button
              bg={"red"}
              onClick={() => {
                cancelEvent(props.eventId);
                props.showEvent(View.Basic);
              }}
          >
            Cancel event
          </Button>
        </Group>

        <Flex
          w="100%"
          h="100%"
          mt="20"
          align="center"
          justify="center"
          gap="10px"
          className={classes.subTitleContainer}
        >
          <form
            style={{
              width: "100%",
              height: "fit-content",
              marginBottom: "10px",
            }}
            onSubmit={changeEventForm.onSubmit((_, event) => {
              event?.stopPropagation();
            })}
          >
          <Fieldset
            legend="Basic information"
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
            <Stack w="50%">
            <TextInput
                required
                label="Event name"
                placeholder="Event name..."
                key={changeEventForm.key("eventName")}
                {...changeEventForm.getInputProps("eventName")}
              ></TextInput>
              <Textarea
                  required
                  placeholder="Write something about the event..."
                  label="Description"
                  autosize
                  minRows={5}
                  key={changeEventForm.key("description")}
                  {...changeEventForm.getInputProps("description")}
                />
                <TagsInput
                  miw="100%"
                  label="Press Enter to submit a tag"
                  placeholder="Enter tag"
                  value={tags}
                  onChange={setTags}
                  styles={{
                    input: {
                      overflowY: "scroll",
                      height: "7rem",
                    },
                  }}
              />
            </Stack>
            <Stack w="50%">
            <DateInput
                  required
                  label="Date"
                  key={changeEventForm.key("date")}
                  {...changeEventForm.getInputProps("date")}
                />
                <TextInput
                    required
                    placeholder="HH:mm"
                    label="Time"
                    key={changeEventForm.key("time")}
                    {...changeEventForm.getInputProps("time")}
                />
                <FileInput
                  required
                  placeholder="Image to be displayed"
                  label="Promo image"
                  key={changeEventForm.key("image")}
                  {...changeEventForm.getInputProps("image")}
                />
                <TextInput
                  placeholder="YouTube embed link"
                  label="Promo video"
                  key={changeEventForm.key("video")}
                  {...changeEventForm.getInputProps("video")}
                />
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
                  Edit
                </InputLabel>
                <Button
                  type="submit"
                  onClick={async (event) => {
                    event.stopPropagation();
                    const values = changeEventForm.getValues();
                    
                    if (tags.length === 0) {
                      alert("Please enter at least one tag!");
                      return;
                    }

                    if (values.image === null) {
                      alert("Please upload an image!");
                      return;
                    }

                    const eventObj : ChangeEventDto = {
                      id: props.eventId,
                      eventName: values.eventName ?? "",
                      description: values.description ?? "",
                      tags: tags,
                      date: values.date ? values.date.toISOString().split("T")[0] : "",
                      time: values.time ?? "",
                      video: values.video ?? "",
                    };

                    const imageData = new FormData();
                    imageData.append('file', values.image);

                    await axios
                    .put(
                      `${
                        import.meta.env.VITE_DB_SERVER
                      }/Host/changeEventDetails`,
                      {
                        ...eventObj
                      }
                    )
                    .then((resp) => {
                      queryClient.invalidateQueries({ queryKey: ["event_preview"] });
                      return resp.data;
                    })
                    .catch((err) => {
                      console.error(err);
                      if (Array.isArray(err.response.data) && err.response.data.length > 0) {
                        alert(err.response.data[0].description);
                      }
                      else {
                        alert(err.response.data);
                      }
                    });

                    await axios
                    .post(`${import.meta.env.VITE_DB_SERVER}/Image/uploadImage/${props.eventId}`, 
                      imageData, {
                        headers: {
                          'Content-Type': 'multipart/form-data',
                        },
                    })
                    .then(() => {
                      alert("Successfully changed event info!");
                      props.showEvent(View.Basic);
                    })
                    .catch((err) => {
                      console.error(err);
                      if (Array.isArray(err.response.data) && err.response.data.length > 0) {
                        alert(err.response.data[0].description);
                      }
                      else {
                        alert(err.response.data);
                      }

                    });

                  }}
                >
                  Edit event details
                </Button>
              </div>
            </Stack>
          </Fieldset>
          </form>

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
              <Stack w="100%" justify="center">
                <TextInput label="Location" value={location} disabled={true} />
                <TextInput label="Address" value={address} disabled={true} />
                <NumberInput
                  disabled={true}
                  label="Capacity"
                  inputMode="numeric"
                  value={capacity}
                />
              </Stack>
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
              <CustomStatsCard 
                title="Reserved tables" 
                dash={false}
                level=""
                current={reservedTables ?? 0}
                nextStage={maxTables ?? 0}
              />
              <StatsCard title="Total earnings" current={totalEarnings ?? 0} />
            </Fieldset>
          </Flex>
        </Flex>
      </Flex>
    </>
  );
}
