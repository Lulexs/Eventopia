import { AuthState } from "../../store/features/auth";
import classes from "./SpaceOwnerPage.module.css";
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
  Group,
} from "@mantine/core";
import { useQuery, useQueryClient} from "@tanstack/react-query";
import axios from "../../../axiosconfig.ts";
import { Event } from "../../EventListing/interfaces";
import { useState, useEffect } from "react";
import { useIsMobile } from "../../util/useIsMobile";
import { StatsCard } from "../StatsCard";
import View from "../SpaceViewPages";
import { DateInput } from "@mantine/dates";
import { Space } from "./interfaces.ts";

export interface OrganizerPageProps {
  user: AuthState;
  showSpace: React.Dispatch<React.SetStateAction<View>>;
}

export default function SpaceOwnerPage(props: OrganizerPageProps) {
  const isMobile = useIsMobile();
  const queryClient = useQueryClient();

  const {
    isLoading: areSpacesLoading,
    data: spaces,
    isError: spacesError
  } = useQuery<Space[]>({
    queryKey: ["owner_spaces"],
    queryFn: async () => {
      return await axios
        .get(`${import.meta.env.VITE_DB_SERVER}/Space/getOwnerSpaces`)
        .then((resp) => {
          console.log(resp.data);
          return resp.data;
        })
        .catch((err) => { console.log(err); return [] })
    },
  });

  const removeSpace = async (spaceId: number) => {
    try {
      await axios.delete(`${import.meta.env.VITE_DB_SERVER}/Space/deleteSpace/${spaceId}`);
      queryClient.invalidateQueries({queryKey: ['owner_spaces']});
    } catch (err) {
      console.error(err);
    }
  };

  const dummyReservations = [
    {
      Adresa: "123 St.John",
      Date: "25.25.2525.",
      Status: "Pending",
      EventName: "Rammstein",
    },
    {
      Adresa: "123 St.John",
      Date: "25.25.2525.",
      Status: "Accepted",
      EventName: "Rammstein",
    },
    {
      Adresa: "123 St.John",
      Date: "25.25.2525.",
      Status: "Pending",
      EventName: "Rammstein",
    },
    {
      Adresa: "123 St.John",
      Date: "25.25.2525.",
      Status: "Accepted",
      EventName: "Rammstein",
    },
    {
      Adresa: "123 St.John",
      Date: "25.25.2525.",
      Status: "Pending",
      EventName: "Rammstein",
    },
    {
      Adresa: "123 St.John",
      Date: "25.25.2525.",
      Status: "Finished",
      EventName: "Rammstein",
    },
    {
      Adresa: "123 St.John",
      Date: "25.25.2525.",
      Status: "Finished",
      EventName: "Rammstein",
    },
  ];

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
              <DateInput label="Birthday"></DateInput>
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
              <TextInput label="Phone number"></TextInput>
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
            <StatsCard title="Rentable spaces" current={15} />
            <StatsCard title="Total rents" current={2} />
          </Fieldset>
        </Stack>
      </Flex>
      <Flex className={classes.contentContainerFlex}>
        <Title>
          My spaces{" "}
          <Button
            onClick={(e) => {
              e.stopPropagation();
              if (isMobile) {
                alert(
                  "Cannot schedule event from mobile device. We are working on it"
                );
                return;
              }
              props.showSpace(View.NewSpace);
            }}
          >
            New space
          </Button>
        </Title>
        <Stack className={classes.contentStack}>
          {/* {(areEventsLoading || eventsError) && (
            <div className={classes.controls}>
              <div className={classes.ldsRing}>
                <div></div>
                <div></div>
                <div></div>
                <div></div>
              </div>
            </div>
          )} */}
          {/* {!areEventsLoading &&
            !eventsError && */}
          {areSpacesLoading || spacesError ? (
            <div></div>
          ) : (
          spaces?.map((space, idx) => (
            <Flex
              key={idx}
              p="sm"
              columnGap="md"
              className={classes.reservationAndVisitedDiv}
              style={{ justifyContent: "center" }}
            >
              <Box className={classes.reservationAndVisitedDivBox}>
                <Text className={classes.reservationAndVisitedDivText}>
                  {space.address}
                </Text>
              </Box>
              <Button onClick={() => removeSpace(space.id)}>Remove space</Button>
            </Flex>
          )))}
        </Stack>
      </Flex>
      <Flex className={classes.contentContainerFlex}>
        <Title>Reservation statuses</Title>
        <Stack className={classes.contentStack}>
          {/* {(areEventsLoading || eventsError) && (
            <div className={classes.controls}>
              <div className={classes.ldsRing}>
                <div></div>
                <div></div>
                <div></div>
                <div></div>
              </div>
            </div>
          )}
          {/* {!areEventsLoading &&
            !eventsError && */}
          {dummyReservations?.map((reservation, idx) => (
            <Flex
              key={idx}
              p="sm"
              columnGap="md"
              className={classes.reservationAndVisitedDiv}
            >
              <Box className={classes.reservationAndVisitedDivBox}>
                <Text className={classes.reservationAndVisitedDivText}>
                  {reservation.Adresa}
                  <br />
                  {reservation.Date}
                </Text>
              </Box>
              <Box className={classes.reservationAndVisitedDivBox}>
                <Text className={classes.reservationAndVisitedDivText}>
                  {reservation.EventName}
                </Text>
              </Box>
              {reservation.Status == "Pending" && (
                <Group>
                  <Button bg="green" fullWidth>
                    Accept
                  </Button>
                  <Button bg="red" fullWidth>
                    Reject
                  </Button>
                </Group>
              )}
              {reservation.Status == "Accepted" && (
                <Group>
                  <Button disabled={true} fullWidth>
                    Upcoming
                  </Button>
                </Group>
              )}
              {reservation.Status == "Finished" && (
                <Group>
                  <Button disabled={true} fullWidth>
                    Finished
                  </Button>
                </Group>
              )}
            </Flex>
          ))}
        </Stack>
      </Flex>
    </Flex>
  );
}
