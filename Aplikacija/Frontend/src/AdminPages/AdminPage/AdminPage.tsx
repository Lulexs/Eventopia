import {
  Flex,
  Title,
  Stack,
  Button,
  Box,
  Image,
  Text,
  Avatar,
} from "@mantine/core";
import { Event } from "../../EventListing/interfaces";
import { AuthState } from "../../store/features/auth";
import classes from "./AdminPage.module.css";
import EventBgImage from "../../assets/event_listing_bg_op.png";
import { useQuery } from "@tanstack/react-query";
import axios from "axios";
import { useState, useEffect } from "react";

export interface AdminPageProps {
  user: AuthState;
}

const dummyUsers = [
  {
    Ime: "Zika",
    Prezime: "Zika",
    Username: "Zile",
    avatar:
      "https://raw.githubusercontent.com/mantinedev/mantine/master/.demo/avatars/avatar-6.png",
    status: "Free",
  },
  {
    Ime: "Zika",
    Prezime: "Zika",
    Username: "Zile",
    avatar:
      "https://raw.githubusercontent.com/mantinedev/mantine/master/.demo/avatars/avatar-6.png",
    status: "Banned",
  },
  {
    Ime: "Zika",
    Prezime: "Zika",
    Username: "Zile",
    avatar:
      "https://raw.githubusercontent.com/mantinedev/mantine/master/.demo/avatars/avatar-6.png",
    status: "Banned",
  },
  {
    Ime: "Zika",
    Prezime: "Zika",
    Username: "Zile",
    avatar:
      "https://raw.githubusercontent.com/mantinedev/mantine/master/.demo/avatars/avatar-6.png",
    status: "Free",
  },
  {
    Ime: "Zika",
    Prezime: "Zika",
    Username: "Zile",
    avatar:
      "https://raw.githubusercontent.com/mantinedev/mantine/master/.demo/avatars/avatar-6.png",
    status: "Free",
  },
  {
    Ime: "Zika",
    Prezime: "Zika",
    Username: "Zile",
    avatar:
      "https://raw.githubusercontent.com/mantinedev/mantine/master/.demo/avatars/avatar-6.png",
    status: "Free",
  },
];

const dummyComments = [
  {
    username: "Zika",
    content:
      "This Pokémon likes to lick its palms that are sweetened by being soaked" +
      "in honey. Teddiursa concocts its own honey by blending fruits and pollen" +
      "collected by Beedrill. Blastoise has water spouts that protrude from its" +
      "shell. The water spouts are very accurate",
  },
  {
    username: "Zika",
    content:
      "This Pokémon likes to lick its palms that are sweetened by being soaked" +
      "in honey. Teddiursa concocts its own honey by blending fruits and pollen" +
      "collected by Beedrill. Blastoise has water spouts that protrude from its" +
      "shell. The water spouts are very accurate",
  },
  {
    username: "Zika",
    content:
      "This Pokémon likes to lick its palms that are sweetened by being soaked" +
      "in honey. Teddiursa concocts its own honey by blending fruits and pollen" +
      "collected by Beedrill. Blastoise has water spouts that protrude from its" +
      "shell. The water spouts are very accurate",
  },
  {
    username: "Zika",
    content:
      "This Pokémon likes to lick its palms that are sweetened by being soaked" +
      "in honey. Teddiursa concocts its own honey by blending fruits and pollen" +
      "collected by Beedrill. Blastoise has water spouts that protrude from its" +
      "shell. The water spouts are very accurate",
  },
];

export default function AdminPage(props: AdminPageProps) {
  const [imageWidth, setImageWidth] = useState("25%");

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
        <Title mb={10}>All users</Title>
        <Stack className={classes.contentStack}>
          {dummyUsers?.map((user, idx) => (
            <Flex
              key={idx}
              p="sm"
              columnGap="md"
              className={classes.reservationAndVisitedDiv}
              style={{ justifyContent: "center" }}
            >
              <Avatar src={user.avatar} w="70px" h="70px" />
              <Box className={classes.reservationAndVisitedDivBox}>
                <Text className={classes.reservationAndVisitedDivText}>
                  {user.Ime} {user.Prezime}
                  <br />
                  {user.Username}
                </Text>
              </Box>
              <Button w="30%" bg={user.status == "Free" ? "red" : "green"}>
                {user.status == "Free" ? "Ban" : "Unban"}
              </Button>
            </Flex>
          ))}
        </Stack>
      </Flex>
      <Flex className={classes.contentContainerFlex}>
        <Title>Active events</Title>
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
                <Button w="fit-content">Remove</Button>
              </Flex>
            ))}
        </Stack>
      </Flex>
      <Flex className={classes.contentContainerFlex}>
        <Title>Recent comments</Title>
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
            dummyComments?.map((comm, idx) => (
              <Flex
                key={idx}
                p="sm"
                columnGap="md"
                className={classes.reservationAndVisitedDiv}
              >
                <Box flex={1}>
                  <Text>{comm.content}</Text>
                </Box>
                <Button w="fit-content" bg="red">
                  Remove
                </Button>
              </Flex>
            ))}
        </Stack>
      </Flex>
    </Flex>
  );
}
