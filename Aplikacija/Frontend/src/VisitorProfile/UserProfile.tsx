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
  TagsInput,
  Dialog,
  Group,
  CloseButton,
  SimpleGrid,
  Box,
  Slider,
  Textarea,
} from "@mantine/core";
import EventBgImage from "../assets/event_listing_bg_op.png";
import { useEffect, useRef, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import axios from "axios";
import { Event } from "../EventListing/interfaces";
import classes from "./UserProfile.module.css";
import { useDisclosure } from "@mantine/hooks";
import { StatsCard } from "./StatsCard";
import { AuthState } from "../store/features/auth";
import { DateInput } from "@mantine/dates";

export interface VisitorProfileProps {
  user: AuthState;
}

export default function VisitorProfile(props: VisitorProfileProps) {
  const [testTags, setTestTags] = useState<string[]>([
    "Rock",
    "Heavy metal",
    "Saban Saulic",
    "film",
    "comics",
  ]);
  const [dialogOpened, { toggle, close }] = useDisclosure(false);
  const dialogTopLeft = useRef([20, 20]);
  const [avatarN, setAvatarN] = useState<string | null>(null);

  const [reviewDialogOpened, { toggle: toggleReview, close: closeReivew }] =
    useDisclosure(false);
  const reviewDialogTopLeft = useRef([
    document.body.clientHeight / 2 - 200,
    document.body.clientWidth / 2 - 200,
  ]);

  const [imageWidth, setImageWidth] = useState("25%");
  const [avatarWidth, setAvatarWidth] = useState("30%");

  useEffect(() => {
    function handleResize() {
      if (document.body.clientWidth > 1000) {
        setImageWidth("25%");
        setAvatarWidth("30%");
      } else {
        setImageWidth("100%");
        setAvatarWidth("80%");
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
            legend="Avatar & Tags"
            w="98%"
            fz="xl"
            mb={10}
            styles={{
              root: {
                display: "flex",
                gap: "15px",
                flexWrap: "wrap",
                justifyContent: "center",
              },
            }}
          >
            <Image
              w={avatarWidth}
              src={avatarN == null ? props.user.avatar : avatarN}
              alt="avatar currently unavailable"
              onClick={(e) => {
                e.stopPropagation();
                dialogTopLeft.current = [e.clientY - 20, e.clientX - 20];
                toggle();
              }}
            />
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
                  Pick your avatar
                </Text>
                <CloseButton
                  onClick={(event) => {
                    event.stopPropagation();
                    close();
                  }}
                />
              </Group>
              <SimpleGrid cols={3}>
                {Array.from({ length: 9 }).map((_, idx) => (
                  <Image
                    key={idx}
                    src={`https://raw.githubusercontent.com/mantinedev/mantine/master/.demo/avatars/avatar-${
                      idx + 1
                    }.png`}
                    onClick={(event) => {
                      event.stopPropagation();
                      setAvatarN(
                        `https://raw.githubusercontent.com/mantinedev/mantine/master/.demo/avatars/avatar-${
                          idx + 1
                        }.png`
                      );
                      close();
                    }}
                  />
                ))}
              </SimpleGrid>
            </Dialog>
            <Flex
              direction="column"
              gap="xs"
              align="center"
              justify="center"
              h="100%"
              flex={1}
            >
              <TagsInput
                miw="100%"
                label="Press Enter to submit a tag"
                placeholder="Enter tag"
                value={testTags}
                onChange={setTestTags}
                styles={{
                  input: {
                    overflowY: "scroll",
                    height: "5rem",
                  },
                }}
              />
              <Group w="100%" justify="center">
                <Button>Save changes</Button>
              </Group>
            </Flex>
          </Fieldset>
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
              <DateInput label="Birthday" />
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
              ></PasswordInput>
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
            <StatsCard
              title="Visited events"
              level="Rookie"
              current={15}
              nextStage={30}
            />
            <StatsCard
              title="Money spent"
              level="Marco Polo"
              current={85}
              nextStage={100}
            />
          </Fieldset>
        </Stack>
      </Flex>
      <Flex className={classes.contentContainerFlex}>
        <Title>Active reservations</Title>
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
                  src={new URL(ev.img, import.meta.url).href}
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
                <Button w="fit-content">Cancel</Button>
                <Text w="10%" ta="center">
                  15$
                </Text>
              </Flex>
            ))}
        </Stack>
      </Flex>
      <Flex className={classes.contentContainerFlex}>
        <Title>Visited events</Title>
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
                  src={new URL(ev.img, import.meta.url).href}
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
                  onClick={(e) => {
                    e.stopPropagation();
                    toggleReview();
                  }}
                >
                  Leave reaview
                </Button>
              </Flex>
            ))}
        </Stack>
      </Flex>
      <Dialog
        opened={reviewDialogOpened}
        withCloseButton={false}
        onClose={close}
        size="md"
        radius="md"
        position={{
          top: reviewDialogTopLeft.current[0],
          left: reviewDialogTopLeft.current[1],
        }}
        onClick={(event) => event.stopPropagation()}
      >
        {" "}
        <Group mb="md" align="center">
          <Text size="sm" fw={300} flex={1}>
            Leave a review
          </Text>
          <CloseButton
            onClick={(event) => {
              event.stopPropagation();
              closeReivew();
            }}
          />
        </Group>
        <Group align="center" mb="xl">
          <Text size="sm" fw={300} miw="45px">
            Rating:{" "}
          </Text>
          <Slider
            flex={1}
            color="blue"
            min={0}
            max={10}
            marks={[
              { value: 0, label: "0" },
              { value: 1, label: "1" },
              { value: 2, label: "2" },
              { value: 3, label: "3" },
              { value: 4, label: "4" },
              { value: 5, label: "5" },
              { value: 6, label: "6" },
              { value: 7, label: "7" },
              { value: 8, label: "8" },
              { value: 9, label: "9" },
              { value: 10, label: "10" },
            ]}
            scale={(value) => value}
          />
        </Group>
        <Textarea
          mb={10}
          label="Review:"
          placeholder="Event was enjoyable..."
          maxRows={8}
          minRows={3}
          autosize
        />
        <Button w="100%" onClick={close}>
          Post review
        </Button>
      </Dialog>
    </Flex>
  );
}
