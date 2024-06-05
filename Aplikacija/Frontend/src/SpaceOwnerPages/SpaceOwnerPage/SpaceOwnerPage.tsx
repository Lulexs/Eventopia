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
  Text,
  Group,
} from "@mantine/core";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import axios from "../../../axiosconfig.ts";
import { useIsMobile } from "../../util/useIsMobile";
import { StatsCard } from "../StatsCard";
import View from "../SpaceViewPages";
import { DateInput } from "@mantine/dates";
import { Space, SpaceOwnerStatistics, SpaceReservation } from "./interfaces.ts";
import { PasswordStrength } from "../../Auth/Utils/PasswordStrength.tsx";
import { matches, useForm } from "@mantine/form";
import { useDispatch } from "react-redux";
import { login } from "../../store/features/auth";

export interface OrganizerPageProps {
  user: AuthState;
  showSpace: React.Dispatch<React.SetStateAction<View>>;
}

export function formatDate(date: Date) {
  const day = String(date.getDate()).padStart(2, "0");
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const year = date.getFullYear();
  const hours = String(date.getHours()).padStart(2, "0");
  const minutes = String(date.getMinutes()).padStart(2, "0");

  return `${day}.${month}.${year}. ${hours}:${minutes}`;
}

export default function SpaceOwnerPage(props: OrganizerPageProps) {
  const isMobile = useIsMobile();
  const queryClient = useQueryClient();
  const dispatch = useDispatch();

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
    isLoading: areSpacesLoading,
    data: spaces,
    isError: spacesError,
  } = useQuery<Space[]>({
    queryKey: ["owner_spaces"],
    queryFn: async () => {
      return await axios
        .get(`${import.meta.env.VITE_DB_SERVER}/Space/getOwnerSpaces`)
        .then((resp) => {
          console.log(resp.data);
          return resp.data;
        })
        .catch((err) => {
          console.log(err);
          return [];
        });
    },
  });

  const {
    isLoading: areReservationsLoading,
    data: reservations,
    isError: reservationsError,
  } = useQuery<SpaceReservation[]>({
    queryKey: ["owner_reservations"],
    queryFn: async () => {
      return await axios
        .get(`${import.meta.env.VITE_DB_SERVER}/Space/getSpacesReservations`)
        .then((resp) => {
          console.log(resp.data);
          return resp.data;
        })
        .catch((err) => {
          console.log(err);
          return [];
        });
    },
  });

  const {
    isLoading: isStatisticsLoading,
    data: statistics,
    isError: statisticsError,
  } = useQuery<SpaceOwnerStatistics>({
    queryKey: ["owner_statistics"],
    queryFn: async () => {
      return await axios
        .get(`${import.meta.env.VITE_DB_SERVER}/Space/getStatistics`)
        .then((resp) => {
          console.log(resp.data);
          return resp.data;
        })
        .catch((err) => {
          console.log(err);
          return [];
        });
    },
  });

  const respondToReservation = async (
    reservationId: number,
    response: string
  ) => {
    try {
      await axios.put(
        `${
          import.meta.env.VITE_DB_SERVER
        }/Space/respondToSpaceReservation/${reservationId}/${response}`
      );
      queryClient.invalidateQueries({ queryKey: ["owner_reservations"] });
    } catch (err) {
      console.error(err);
    }
  };

  const removeSpace = async (spaceId: number) => {
    try {
      await axios.delete(
        `${import.meta.env.VITE_DB_SERVER}/Space/deleteSpace/${spaceId}`
      );
      queryClient.invalidateQueries({ queryKey: ["owner_spaces"] });
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
                      console.log(values);
                      await axios
                        .put(
                          `${
                            import.meta.env.VITE_DB_SERVER
                          }/Account/updateUser`,
                          {
                            ...values,
                            birthday: values.birthday.toISOString(),
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
                          alert(err.response.data[0].description);
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
            <StatsCard
              title="Rentable spaces"
              current={
                isStatisticsLoading || statisticsError
                  ? 0
                  : statistics?.rentableSpaces || 0
              }
            />
            <StatsCard
              title="Total rents"
              current={
                isStatisticsLoading || statisticsError
                  ? 0
                  : statistics?.totalRents || 0
              }
            />
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
        <Stack className={classes.contentStack} align="center">
          {areSpacesLoading || spacesError ? (
            <div className={classes.controls}>
              <div className={classes.ldsRing}>
                <div></div>
                <div></div>
                <div></div>
                <div></div>
              </div>
            </div>
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
                <Button onClick={() => removeSpace(space.id)}>
                  Remove space
                </Button>
              </Flex>
            ))
          )}
        </Stack>
      </Flex>
      <Flex className={classes.contentContainerFlex}>
        <Title>Reservation statuses</Title>
        <Stack className={classes.contentStack} align="center">
          {areReservationsLoading || reservationsError ? (
            <div className={classes.controls}>
              <div className={classes.ldsRing}>
                <div></div>
                <div></div>
                <div></div>
                <div></div>
              </div>
            </div>
          ) : (
            reservations?.map((reservation, idx) => (
              <Flex
                key={idx}
                p="sm"
                columnGap="md"
                className={classes.reservationAndVisitedDiv}
              >
                <Box className={classes.reservationAndVisitedDivBox}>
                  <Text className={classes.reservationAndVisitedDivText}>
                    {reservation.address}
                    <br />
                    {formatDate(new Date(reservation.startTime))}
                    <br />
                    {formatDate(new Date(reservation.endTime))}
                  </Text>
                </Box>
                <Box className={classes.reservationAndVisitedDivBox}>
                  <Text className={classes.reservationAndVisitedDivText}>
                    {reservation.eventName}
                  </Text>
                </Box>
                {reservation.status == "WaitingConfirmation" && (
                  <Group>
                    <Button
                      bg="green"
                      fullWidth
                      onClick={() =>
                        respondToReservation(reservation.id, "accept")
                      }
                    >
                      Accept
                    </Button>
                    <Button
                      bg="red"
                      fullWidth
                      onClick={() =>
                        respondToReservation(reservation.id, "reject")
                      }
                    >
                      Reject
                    </Button>
                  </Group>
                )}
                {reservation.status == "Confirmed" && (
                  <Group>
                    <Button disabled={true} fullWidth>
                      Upcoming
                    </Button>
                  </Group>
                )}
                {reservation.status == "Finished" && (
                  <Group>
                    <Button disabled={true} fullWidth>
                      Finished
                    </Button>
                  </Group>
                )}
              </Flex>
            ))
          )}
        </Stack>
      </Flex>
    </Flex>
  );
}
