import { AuthState, login } from "../../store/features/auth";
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
import axios from "../../../axiosconfig.ts";
import { useState, useEffect } from "react";
import { StatsCard } from "../StatsCard";
import { View } from "../EventViewPages";
import { useIsMobile } from "../../util/useIsMobile";
import { DateInput } from "@mantine/dates";
import { useForm, matches } from "@mantine/form";
import { useDispatch } from "react-redux";
import { PasswordStrength } from "../../Auth/Utils/PasswordStrength";
import { EventBasic } from "../../AdminPages/AdminPage/interfaces.ts";
import { formatOnlyDate } from "../../AdminPages/AdminPage/AdminPage.tsx";
import { formatTimeOnly } from "../../VisitorProfile/UserProfile.tsx";
import { HostStatistics } from "../interfaces.ts";

export interface OrganizerPageProps {
  user: AuthState;
  showEvent: React.Dispatch<React.SetStateAction<View>>;
  setEventId: React.Dispatch<React.SetStateAction<number>>;
  setEventName: React.Dispatch<React.SetStateAction<string>>;
  setEventDate: React.Dispatch<React.SetStateAction<string>>;
}

export default function OrganizerPage(props: OrganizerPageProps) {
  const [imageWidth, setImageWidth] = useState("25%");
  const isMobile = useIsMobile();
  const dispatch = useDispatch();

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

  const updateUserForm = useForm({
    mode: "controlled",
    initialValues: {
      firstName: props.user.firstName,
      lastName: props.user.lastName,
      city: props.user.city,
      address: props.user.address,
      birthday: new Date(props.user.birthday),
      phoneNumber: props.user.phoneNumber,
      newPassword: "",
      currentPassword: "",
    },

    validate: {
      firstName: (value) =>
        value.length > 0 ? null : "Empty first name field",
      lastName: (value) => (value.length > 0 ? null : "Empty last name field"),
      city: (value) =>
        value != null && value.length > 0 ? null : "Empty city field",
      address: (value) =>
        value != null && value.length > 0 ? null : "Empty address field",
      phoneNumber: (value) =>
        value.length > 0 ? null : "Empty phone number field",
      currentPassword: (value) =>
        value.length >= 0 ? null : "Empty current password field",
      newPassword: (value) =>
        value.length == 0 || matches(/(?:[0-9]|[a-z]|[A-Z]|[^\w\s])/)
          ? null
          : "Empty new password field",
    },
  });

  const {
    isLoading: areEventsLoading,
    data: events,
    isError: eventsError,
  } = useQuery<EventBasic[]>({
    queryKey: ["incoming_events"],
    queryFn: async () => {
      return await axios
        .get(`${import.meta.env.VITE_DB_SERVER}/Host/getIncomingEvents`)
        .then((resp) => {
          return resp.data;
        })
        .catch((err) => {
          console.error(err);
          if (
            Array.isArray(err.response.data) &&
            err.response.data.length > 0
          ) {
            alert(err.response.data[0].description);
          } else {
            alert(err.response.data);
          }
          return [];
        });
    },
  });

  const {
    isLoading: arePastEventsLoading,
    data: pastEvents,
    isError: pastEventsError,
  } = useQuery<EventBasic[]>({
    queryKey: ["past_events"],
    queryFn: async () => {
      return await axios
        .get(`${import.meta.env.VITE_DB_SERVER}/Host/getPastEvents`)
        .then((resp) => {
          return resp.data;
        })
        .catch((err) => {
          console.error(err);
          if (
            Array.isArray(err.response.data) &&
            err.response.data.length > 0
          ) {
            alert(err.response.data[0].description);
          } else {
            alert(err.response.data);
          }
          return [];
        });
    },
  });

  const {
    isLoading: isStatisticsLoading,
    data: statistics,
    isError: statisticsError,
  } = useQuery<HostStatistics>({
    queryKey: ["host_statistics"],
    queryFn: async () => {
      return await axios
        .get(`${import.meta.env.VITE_DB_SERVER}/Host/getStatistics`)
        .then((resp) => {
          return resp.data;
        })
        .catch((err) => {
          console.error(err);
          if (
            Array.isArray(err.response.data) &&
            err.response.data.length > 0
          ) {
            alert(err.response.data[0].description);
          } else {
            alert(err.response.data);
          }
          return null;
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
          <form
            onSubmit={updateUserForm.onSubmit((_, event) => {
              event?.stopPropagation();
            })}
          >
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
                <TextInput
                  label="First name"
                  key={updateUserForm.key("firstName")}
                  {...updateUserForm.getInputProps("firstName")}
                />
                <TextInput
                  label="City"
                  key={updateUserForm.key("city")}
                  {...updateUserForm.getInputProps("city")}
                />
                <DateInput
                  label="Birthday"
                  key={updateUserForm.key("birthday")}
                  {...updateUserForm.getInputProps("birthday")}
                />
                <PasswordStrength
                  label="New password"
                  placeholder="New password"
                  key={updateUserForm.key("password")}
                  useFormProps={{
                    ...updateUserForm.getInputProps("newPassword"),
                  }}
                />
              </Stack>
              <Stack w="50%">
                <TextInput label="Email" disabled value={props.user.email} />{" "}
                <TextInput
                  label="Last name"
                  key={updateUserForm.key("lastName")}
                  {...updateUserForm.getInputProps("lastName")}
                />
                <TextInput
                  label="Address"
                  key={updateUserForm.key("address")}
                  {...updateUserForm.getInputProps("address")}
                />
                <TextInput
                  label="Phone number"
                  key={updateUserForm.key("phoneNumber")}
                  {...updateUserForm.getInputProps("phoneNumber")}
                />
                <PasswordInput
                  label="Current password"
                  placeholder="Enter current password"
                  key={updateUserForm.key("currentPassword")}
                  {...updateUserForm.getInputProps("currentPassword")}
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
                    Save changes
                  </InputLabel>
                  <Button
                    type="submit"
                    onClick={async (event) => {
                      event.stopPropagation();
                      const values = updateUserForm.getValues();

                      await axios
                        .put(
                          `${
                            import.meta.env.VITE_DB_SERVER
                          }/Account/updateUser`,
                          {
                            ...values,
                            birthday: values.birthday.toISOString()
                          }
                        )
                        .then((resp) => {
                          alert("Successfully changed user info!");
                          const obj = JSON.parse(
                            atob(resp.data.token.split(".")[1])
                          );
                          dispatch(
                            login({
                              userId: obj["nameid"],
                              token: resp.data.token,
                              email: obj["email"],
                              userType: obj["role"],

                              firstName: resp.data.firstName,
                              lastName: resp.data.lastName,
                              birthday: resp.data.dateOfBirth,
                              phoneNumber: resp.data.phoneNumber,
                              avatar: resp.data.avatar,
                              address: resp.data.address,
                              city: resp.data.city,
                            })
                          );
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
                    Save changes
                  </Button>
                </div>
              </Stack>
            </Fieldset>
          </form>
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
            <StatsCard title="Hosted events" current={statistics && !isStatisticsLoading && !statisticsError ? statistics.hostedEvents : 0 } />
            <StatsCard title="Average rating" current={statistics && !isStatisticsLoading && !statisticsError ? statistics.averageRating : 0} />
            <StatsCard title="Reservations" current={statistics && !isStatisticsLoading && !statisticsError ? statistics.reservations : 0} />
            <StatsCard title="Estimated earnings" current={statistics && !isStatisticsLoading && !statisticsError ? statistics.estimatedEarnings : 0} />
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
                    {formatOnlyDate(new Date(ev.date))} {formatTimeOnly(new Date(ev.date))}
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
        <Stack className={classes.contentStack} align="center">
          {(arePastEventsLoading || pastEventsError) && (
            <div className={classes.controls}>
              <div className={classes.ldsRing}>
                <div></div>
                <div></div>
                <div></div>
                <div></div>
              </div>
            </div>
          )}
          {!arePastEventsLoading &&
            !pastEventsError &&
            pastEvents?.map((ev, idx) => (
              <Flex
                key={idx}
                p="sm"
                columnGap="md"
                className={classes.reservationAndVisitedDiv}
              >
                <Image
                  className={classes.reservationAndVisitedDivImage}
                  src={`data:image/jpeg;base64,${ev.image}`}
                  alt={`Couldn't load ${ev.name} image`}
                  fit="cover"
                  w={imageWidth}
                />
                <Box className={classes.reservationAndVisitedDivBox}>
                  <Text className={classes.reservationAndVisitedDivText}>
                    {ev.name}
                    <br />
                    {formatOnlyDate(new Date(ev.date))} {formatTimeOnly(new Date(ev.date))}
                  </Text>
                </Box>
                <Button
                  w="fit-content"
                  onClick={(event) => {
                    event.stopPropagation();
                    console.log(ev.id);
                    props.setEventId(ev.id);
                    props.setEventName(ev.name);
                    props.setEventDate(formatOnlyDate(new Date(ev.date)));
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
