import { AuthState } from "../../store/features/auth";
import classes from "./OrganizerPage.module.css";
import EventBgImage from "../../assets/event_listing_bg_op.png";
import {
  Box,
  Button,
  Fieldset,
  Flex,
  InputLabel,
  PasswordInput,
  Stack,
  TextInput,
  Title,
  Image,
  Text,
} from "@mantine/core";
import { useQuery } from "@tanstack/react-query";
import axios from "axios";
import { Event } from "../../EventListing/interfaces";
import { useState, useEffect } from "react";
import { StatsCard } from "../StatsCard";
import { View } from "../EventViewPages";
import { useIsMobile } from "../../util/useIsMobile";

export interface OrganizerPageProps {
  user: AuthState;
  showEvent: React.Dispatch<React.SetStateAction<View>>;
  setEventId: React.Dispatch<React.SetStateAction<number>>;
}

export default function OrganizerPage(props: OrganizerPageProps) {
  const [imageWidth, setImageWidth] = useState("25%");
  const isMobile = useIsMobile();

  useEffect(() => {
    function handleResize() {
      if (document.body.clientWidth > 1000) {
        setImageWidth("25%");
      } else {
        setImageWidth("100%");
      }
    }

    window.addEventListener("resize", handleResize);

    return () => {
      window.removeEventListener("resize", handleResize);
    };
  }, []);

  const {
    isLoading: areEventsLoading,
    data: events,
    isError: eventsError,
  } = useQuery<Event[]>({
    queryKey: ["visited_events"],
    queryFn: async () => {
      return await axios
        .get(`${import.meta.env.VITE_JSON_SERVER}/hotevents`)
        .then((resp) => {
          return resp.data;
        });
    },
  });

  return (
    <Flex
      className={classes.mainContentFlex}
      styles={{
        root: {
          backgroundImage: `url(${EventBgImage})`,
          backgroundRepeat: "repeat",
        },
      }}
    >
      <Flex className={classes.contentContainerFlex}>
        <Title mb={10}>User info</Title>
        <Stack className={classes.contentStack}>
          <Fieldset
            legend="Personal information"
            w="98%"
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
                label="User ID"
                disabled
                value={props.user.userId}
              ></TextInput>
              <TextInput label="First name"></TextInput>
              <TextInput label="Last name"></TextInput>
              <TextInput label="Address"></TextInput>
            </Stack>
            <Stack w="50%">
              <TextInput
                label="Email"
                disabled
                value={props.user.email}
              ></TextInput>{" "}
              <PasswordInput
                label="Password"
                placeholder="Enter new password"
              />
              <TextInput label="City"></TextInput>
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
                  Save changes
                </InputLabel>
                <Button>Save changes</Button>
              </div>
            </Stack>
          </Fieldset>
          <Fieldset
            legend="Statistics"
            w="98%"
            fz="sm"
            styles={{
              root: {
                display: "flex",
                justifyContent: "space-between",
                flexWrap: "wrap",
              },
            }}
          >
            <StatsCard title="Hosted events" current={15} />
            <StatsCard title="Average rating" current={4.53} />
            <StatsCard title="Reservations" current={100} />
            <StatsCard title="Estimated earnings" current={1500} />
          </Fieldset>
        </Stack>
      </Flex>
      <Flex className={classes.contentContainerFlex}>
        <Title>
          Incoming events{" "}
          <Button
            onClick={(e) => {
              e.stopPropagation();
              if (isMobile) {
                alert(
                  "Cannot schedule event from mobile device. We are working on it"
                );
                return;
              }
              props.showEvent(View.NewEvent);
            }}
          >
            New event
          </Button>
        </Title>
        <Stack className={classes.contentStack}>
          {(areEventsLoading || eventsError) && (
            <div className={classes.controls}>
              <div className={classes.ldsRing}>
                <div></div>
                <div></div>
                <div></div>
                <div></div>
              </div>
            </div>
          )}
          {!areEventsLoading &&
            !eventsError &&
            events?.map((ev, idx) => (
              <Flex
                key={idx}
                p="sm"
                columnGap="md"
                className={classes.reservationAndVisitedDiv}
              >
                <Image
                  src={new URL("../" + ev.img, import.meta.url).href}
                  alt={`Couldn't load ${ev.title} image`}
                  fit="cover"
                  w={imageWidth}
                  className={classes.reservationAndVisitedDivImage}
                />
                <Box className={classes.reservationAndVisitedDivBox}>
                  <Text className={classes.reservationAndVisitedDivText}>
                    {ev.title}
                    <br />
                    {ev.date}
                  </Text>
                </Box>
                <Button
                  w="fit-content"
                  onClick={(event) => {
                    event.stopPropagation();
                    if (isMobile) {
                      alert(
                        "Cannot schedule event from mobile device. We are working on it"
                      );
                      return;
                    }
                    props.setEventId(ev.id);
                    props.showEvent(View.ManageEvent);
                  }}
                >
                  Manage
                </Button>
              </Flex>
            ))}
        </Stack>
      </Flex>
      <Flex className={classes.contentContainerFlex}>
        <Title>Past events</Title>
        <Stack className={classes.contentStack}>
          {(areEventsLoading || eventsError) && (
            <div className={classes.controls}>
              <div className={classes.ldsRing}>
                <div></div>
                <div></div>
                <div></div>
                <div></div>
              </div>
            </div>
          )}
          {!areEventsLoading &&
            !eventsError &&
            events?.map((ev, idx) => (
              <Flex
                key={idx}
                p="sm"
                columnGap="md"
                className={classes.reservationAndVisitedDiv}
              >
                <Image
                  className={classes.reservationAndVisitedDivImage}
                  src={new URL("../" + ev.img, import.meta.url).href}
                  alt={`Couldn't load ${ev.title} image`}
                  fit="cover"
                  w={imageWidth}
                />
                <Box className={classes.reservationAndVisitedDivBox}>
                  <Text className={classes.reservationAndVisitedDivText}>
                    {ev.title}
                    <br />
                    {ev.date}
                  </Text>
                </Box>
                <Button
                  w="fit-content"
                  onClick={(event) => {
                    event.stopPropagation();
                    props.setEventId(ev.id);
                    props.showEvent(View.PastEventDetails);
                  }}
                >
                  Reviews
                </Button>
              </Flex>
            ))}
        </Stack>
      </Flex>
    </Flex>
  );
}
