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
} from "@mantine/core";
import { Footer } from "../HomePage/Footer/Footer";
import { HeaderMegaMenu } from "../HomePage/HeaderMegaMenu/HeaderMegaMenu";
import EventBgImage from "../assets/event_listing_bg_op.png";
import { CSSProperties, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { RootState } from "../store/store";
import { useQuery } from "@tanstack/react-query";
import axios from "axios";
import { Event } from "../EventListing/interfaces";
import classes from "./UserProfile.module.css";
import { useDisclosure } from "@mantine/hooks";
import { StatsCard } from "./StatsCard";

const commonProperties: CSSProperties = {
  border: "1px dashed black",
  width: "33%",
  padding: "10px",
  color: "#5A5959",
  overflow: "hidden",
};

export default function VisitorProfile() {
  const userInfo = useSelector((state: RootState) => state.auth);
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
          <Title mb={10}>User info</Title>
          <Stack
            w="100%"
            h="100%"
            align="center"
            style={{ overflowY: "scroll", overflowX: "hidden" }}
          >
            <Fieldset
              legend="Avatar & Tags"
              w="100%"
              fz="xl"
              mb={10}
              styles={{
                root: {
                  display: "flex",
                  gap: "15px",
                },
              }}
            >
              <Image
                w="30%"
                src={avatarN == null ? userInfo.avatar : avatarN}
                alt="avatar currently unavailable"
                onClick={(e) => {
                  e.stopPropagation();
                  dialogTopLeft.current = [e.clientY, e.clientX];
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
              w="100%"
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
            <Fieldset
              legend="Statistics"
              w="100%"
              fz="sm"
              styles={{
                root: {
                  display: "flex",
                  justifyContent: "space-between",
                  gap: "10px",
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
              <StatsCard
                title="Reviews"
                level="Rookie"
                current={0}
                nextStage={5}
              />
            </Fieldset>
          </Stack>
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
