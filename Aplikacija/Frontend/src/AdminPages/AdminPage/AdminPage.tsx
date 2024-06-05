import {
  Flex,
  Title,
  Stack,
  Button,
  Box,
  Image,
  Text,
  Avatar,
  CloseButton,
  Dialog,
  Group,
  Textarea,
} from "@mantine/core";
import { AuthState } from "../../store/features/auth";
import classes from "./AdminPage.module.css";
import EventBgImage from "../../assets/event_listing_bg_op.png";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import axios from "axios";
import { useState, useEffect, useRef } from "react";
import { Comment, EventBasic, UserWithBans } from "./interfaces";
import { useDisclosure } from "@mantine/hooks";
import { DateInput } from "@mantine/dates";

export function formatOnlyDate(date: Date | null) {
  const day = String(date?.getDate()).padStart(2, '0');
  const month = String(date?.getMonth()! + 1).padStart(2, '0');
  const year = date?.getFullYear();

  return `${day}.${month}.${year}.`;
}

export interface AdminPageProps {
  user: AuthState;
}

export default function AdminPage() {
  const [imageWidth, setImageWidth] = useState("25%");
  const [dialogOpened, { toggle, close }] = useDisclosure(false);
  const [userId, setUserId] = useState<string>("");
  const [banUntil, setBanUntil] = useState<Date | null>(null);
  const [banReason, setBanReason] = useState<string>("");
  const dialogTopLeft = useRef([20, 20]);
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
    isLoading: areUsersLoading,
    data: users,
    isError: usersError,
  } = useQuery<UserWithBans[]>({
    queryKey: ["user_list"],
    queryFn: async () => {
      return await axios
        .get(`${import.meta.env.VITE_DB_SERVER}/Administrator/getUsersWithBans`)
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
        `${
          import.meta.env.VITE_DB_SERVER
        }/Administrator/deleteComment/${commentId}`
      );
      queryClient.invalidateQueries({ queryKey: ["all_comments"] });
    } catch (err) {
      console.error(err);
    }
  };

  const banUser = async () => {
    try {
      await axios.post(`${import.meta.env.VITE_DB_SERVER}/Administrator/banUser`, {
        userId: userId,
        timeTo: banUntil?.toISOString(),
        reason: banReason,
      });
      setUserId("");
      setBanReason("");
      setBanUntil(null);
      queryClient.invalidateQueries({ queryKey: ["user_list"] });
      alert("User is successfully banned!");
    } catch (err : any) {
      console.error(err);
      alert(err.response.data);
    }
  }

  const unbanUser = async (banId : number) => {
    try {
      await axios.delete(`${import.meta.env.VITE_DB_SERVER}/Administrator/unbanUser/${banId}`);
      queryClient.invalidateQueries({ queryKey: ["user_list"] });
      alert("User is successfully unbanned!");
    } catch (err : any) {
      console.error(err);
      alert(err.response.data);
    }
  }

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
        <Stack className={classes.contentStack} align="center">
        {areUsersLoading || usersError ? (
            <div className={classes.controls}>
              <div className={classes.ldsRing}>
                <div></div>
                <div></div>
                <div></div>
                <div></div>
              </div>
            </div>
          ) : (
          users?.map((user, idx) => (
            <Flex
              key={idx}
              p="sm"
              columnGap="md"
              className={classes.reservationAndVisitedDiv}
              style={{ justifyContent: "center" }}
            >
              <Avatar src={user.role == "Visitor" ? user.avatar : null} w="70px" h="70px" />
              <Box className={classes.usersDivBox}>
                <Text className={classes.reservationAndVisitedDivText}>
                  {user.firstName} {user.lastName}
                  <br />
                  {user.role}
                  <br />
                  {user.reason != null ? (`${formatOnlyDate(new Date(user.timeFrom))} - ${(formatOnlyDate(new Date(user.timeTo)))}`) : ""}
                </Text>
              </Box>
              <Button
                w="30%"
                bg={user.reason == null ?  "red" : "green"}
                onClick={() => {
                  setUserId(user.userId);
                  if (user.reason == null) {
                    toggle();
                  }
                  else {
                    close();
                    unbanUser(user.banId);
                  }
                }}
              >
                {user.reason == null ? "Ban" : "Unban"}
              </Button>
            </Flex>
          )))}
        </Stack>
      </Flex>
      <Flex className={classes.contentContainerFlex}>
        <Title>Active events</Title>
        <Stack className={classes.contentStack} align="center">
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
                  src={`data:image/jpeg;base64,${ev.image}`}
                  alt={`Couldn't load ${ev.name} image`}
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
                <Button w="fit-content" onClick={() => deleteEvent(ev.id)}>
                  Remove
                </Button>
              </Flex>
            ))}
        </Stack>
      </Flex>
      <Flex className={classes.contentContainerFlex}>
        <Title>Recent comments</Title>
        <Stack className={classes.contentStack} align="center">
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
                <Button
                  w="fit-content"
                  bg="red"
                  onClick={() => deleteComment(comm.id)}
                >
                  Remove
                </Button>
              </Flex>
            ))}
        </Stack>
      </Flex>
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
        {" "}
        <Group mb="md" align="center">
          <Text size="sm" fw={300} flex={1}>
            Ban user
          </Text>
          <CloseButton
            onClick={(event) => {
              event.stopPropagation();
              close();
            }}
          />
        </Group>
        <Group align="center" mb={10}>
          <Text size="sm" fw={300} miw="45px">
            Until:{" "}
          </Text>
          <DateInput
            placeholder={`${formatOnlyDate(new Date(Date.now()))}`}
            flex={1}
            value={banUntil}
            onChange={(date) => setBanUntil(date) }
          />
        </Group>
        <Textarea
          mb={10}
          label="Reason:"
          placeholder="User was rude to others..."
          maxRows={8}
          minRows={3}
          autosize
          value={banReason}
          onChange={(event) => setBanReason(event.currentTarget.value)}
        />
        <Button
          w="100%"
          onClick={() => {
            if (!banReason) {
              alert("Please enter a reason for the ban!");
              return;
            }
      
            if (!banUntil || (banUntil && banUntil < new Date(Date.now()))) {
              alert("Please enter a valid date for the ban!");
              return;
            }
            banUser();
            close();
          }}
        >
          Ban user
        </Button>
      </Dialog>
    </Flex>
  );
}
