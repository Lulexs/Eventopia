import {
  Flex,
  Stack,
  Title,
  Text,
  Image,
  Button,
  Fieldset,
  TextInput,
  InputLabel,
  PasswordInput,
} from "@mantine/core";
import { Footer } from "../HomePage/Footer/Footer";
import { HeaderMegaMenu } from "../HomePage/HeaderMegaMenu/HeaderMegaMenu";
import EventBgImage from "../assets/event_listing_bg_op.png";
import { CSSProperties } from "react";
import { useSelector } from "react-redux";
import { RootState } from "../store/store";
import { useQuery } from "@tanstack/react-query";
import axios from "axios";
import { Event } from "../EventListing/interfaces";
import classes from "./UserProfile.module.css";

const commonProperties: CSSProperties = {
  border: "1px dashed black",
  width: "33%",
  padding: "10px",
  color: "#5A5959",
  overflow: "hidden",
};

export default function VisitorProfile() {
  const userInfo = useSelector((state: RootState) => state.auth);

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
          console.log(resp.data);
          return resp.data;
        });
    },
  });

  return (
    <Flex
      direction="column"
      h="100vh"
      styles={{
        root: {
          backgroundImage: `url(${EventBgImage})`,
          backgroundRepeat: "repeat",
        },
      }}
    >
      <HeaderMegaMenu />
      <Flex
        m={20}
        flex="1"
        justify="space-between"
        style={{ overflow: "hidden" }}
      >
        <Flex
          style={{ ...commonProperties }}
          justify="flex-start"
          direction="column"
          align="center"
        >
          <Title mb={20}>User info</Title>
          <Fieldset
            legend="Personal information"
            w="100%"
            fz="xl"
            styles={{
              root: {
                display: "flex",
                justifyContent: "space-between",
                gap: "10px",
              },
            }}
          >
            <Stack w="50%">
              <TextInput
                label="User ID"
                disabled
                value={userInfo.userId}
              ></TextInput>
              <TextInput label="First name"></TextInput>
              <TextInput label="Last name"></TextInput>
            </Stack>
            <Stack w="50%">
              <TextInput
                label="Email"
                disabled
                value={userInfo.email}
              ></TextInput>{" "}
              <PasswordInput
                label="Password"
                placeholder="Enter new password"
              ></PasswordInput>
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
        </Flex>
        <Flex
          style={{ ...commonProperties }}
          justify="flex-start"
          align="center"
          direction="column"
        >
          <Title>Active reservations</Title>
          <Stack
            w="100%"
            h="100%"
            align="center"
            style={{ overflowY: "scroll" }}
          >
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
                  justify="space-between"
                  align="center"
                  w="90%"
                >
                  <Image
                    src={new URL(ev.img, import.meta.url).href}
                    height={90}
                    alt={`Couldn't load ${ev.title} image`}
                    fit="cover"
                    mr={10}
                  />
                  <Text>
                    {ev.title}
                    <br />
                    {ev.date}
                  </Text>
                  <Button>Cancel reservation</Button>
                  <Text>15$</Text>
                </Flex>
              ))}
          </Stack>
        </Flex>
        <Flex
          style={{ ...commonProperties }}
          justify="flex-start"
          align="center"
          direction="column"
        >
          <Title>Visited events</Title>
          <Stack
            w="100%"
            h="100%"
            align="center"
            style={{ overflowY: "scroll" }}
          >
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
                  justify="space-between"
                  align="center"
                  w="90%"
                >
                  <Image
                    src={new URL(ev.img, import.meta.url).href}
                    height={90}
                    alt={`Couldn't load ${ev.title} image`}
                    fit="cover"
                    mr={10}
                  />
                  <Text>
                    {ev.title}
                    <br />
                    {ev.date}
                  </Text>
                  <Button>Leave comment or rating</Button>
                </Flex>
              ))}
          </Stack>
        </Flex>
      </Flex>
      <Footer />
    </Flex>
  );
}
