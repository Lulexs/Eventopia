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
import { AuthState } from "../../store/features/auth";
import classes from "./AdminPage.module.css";
import EventBgImage from "../../assets/event_listing_bg_op.png";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import axios from "axios";
import { useState, useEffect } from "react";
import { Comment, EventBasic } from "./interfaces";

export function formatOnlyDate(date: Date) {
  const day = String(date.getDate()).padStart(2, '0');
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const year = date.getFullYear();

  return `${day}.${month}.${year}.`;
}

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

export default function AdminPage(props: AdminPageProps) {
  const [imageWidth, setImageWidth] = useState("25%");
  const queryClient = useQueryClient();

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
  } = useQuery<EventBasic[]>({
    queryKey: ["all_events"],
    queryFn: async () => {
      return await axios
        .get(`${import.meta.env.VITE_DB_SERVER}/Administrator/getAllEvents`)
        .then((resp) => {
          return resp.data;
        })
        .catch((err) => {
          console.log(err);
          return [];
        });
    },
  });

  const {
    isLoading: areCommentsLoading,
    data: comments,
    isError: commentsError,
  } = useQuery<Comment[]>({
    queryKey: ["all_comments"],
    queryFn: async () => {
      return await axios
        .get(`${import.meta.env.VITE_DB_SERVER}/Administrator/getAllComments`)
        .then((resp) => {
          return resp.data;
        })
        .catch((err) => {
          console.log(err);
          return [];
        });
    },
  });

  const deleteEvent = async (eventId: number) => {
    try {
      await axios.delete(
        `${import.meta.env.VITE_DB_SERVER}/Administrator/deleteEvent/${eventId}`
      );
      queryClient.invalidateQueries({ queryKey: ["all_events"] });
    } catch (err) {
      console.error(err);
    }
  };

  const deleteComment = async (commentId: number) => {
    try {
      await axios.delete(
        `${import.meta.env.VITE_DB_SERVER}/Administrator/deleteComment/${commentId}`
      );
      queryClient.invalidateQueries({ queryKey: ["all_comments"] });
    } catch (err) {
      console.error(err);
    }
  };

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
                  //src={new URL("../" + ev.img, import.meta.url).href}
                  src={`data:image/jpeg;base64,${ev.image}`}
                  alt={"Couldn't load image"}
                  fit="cover"
                  w={imageWidth}
                  className={classes.reservationAndVisitedDivImage}
                />
                <Box className={classes.reservationAndVisitedDivBox}>
                  <Text className={classes.reservationAndVisitedDivText}>
                    {ev.name}
                    <br />
                    {formatOnlyDate(new Date(ev.date))}
                  </Text>
                </Box>
                <Button w="fit-content" onClick={() => deleteEvent(ev.id)}>Remove</Button>
              </Flex>
            ))}
        </Stack>
      </Flex>
      <Flex className={classes.contentContainerFlex}>
        <Title>Recent comments</Title>
        <Stack className={classes.contentStack}>
          {(areCommentsLoading || commentsError) && (
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
            comments?.map((comm, idx) => (
              <Flex
                key={idx}
                p="sm"
                columnGap="md"
                className={classes.reservationAndVisitedDiv}
              >
                <Box flex={1}>
                  <Text>{comm.comment}</Text>
                </Box>
                <Button w="fit-content" bg="red" onClick={() => deleteComment(comm.id)}>
                  Remove
                </Button>
              </Flex>
            ))}
        </Stack>
      </Flex>
    </Flex>
  );
}
