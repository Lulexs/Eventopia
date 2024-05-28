import {
  HoverCard,
  Group,
  Button,
  UnstyledButton,
  Text,
  SimpleGrid,
  ThemeIcon,
  Divider,
  Center,
  Box,
  Burger,
  Drawer,
  Collapse,
  ScrollArea,
  rem,
  useMantineTheme,
  Image,
  Stack,
} from "@mantine/core";
import Logo from "../../assets/logo.png";
import { useDisclosure } from "@mantine/hooks";
import {
  IconNotification,
  IconChartPie3,
  IconChevronDown,
  IconTicket,
  IconSearch,
  IconCalendar,
  IconMapPinShare,
} from "@tabler/icons-react";
import classes from "./HeaderMegaMenu.module.css";
import { Link, useNavigate } from "react-router-dom";
import { PathConstants } from "../../Routes/pathConstants";
import { useDispatch, useSelector } from "react-redux";
import { logout } from "../../store/features/auth";
import { RootState } from "../../store/store";

const data = [
  {
    icon: IconSearch,
    title: "Explore latest events",
    description: "Explore, filter and find events for your next journey",
    onClick: (navigate: Function) => {
      navigate("/");
      document
        .querySelector(".main-ev-listing-div")
        ?.scrollIntoView({ behavior: "smooth", block: "start" });
    },
  },
  {
    icon: IconTicket,
    title: "Reserve seat",
    description: "Get your seat reservation for your favourite event now",
  },
  {
    icon: IconCalendar,
    title: "Host an event",
    description: "Host next memorable event and let everyone see!",
  },
  {
    icon: IconChartPie3,
    title: "Statistics",
    description: "Check list of visited events or check event statistics",
  },
  {
    icon: IconMapPinShare,
    title: "Rent space for next exciting event",
    description: "Advertise your space and let organizers host their events!",
  },
  {
    icon: IconNotification,
    title: "Notifications",
    description:
      "Subscribe to get updates about events that might interest you!",
  },
];

export function HeaderMegaMenu() {
  const [drawerOpened, { toggle: toggleDrawer, close: closeDrawer }] =
    useDisclosure(false);
  const [linksOpened, { toggle: toggleLinks }] = useDisclosure(false);
  const theme = useMantineTheme();
  const navigate = useNavigate();
  const loggedUser = useSelector((state: RootState) => state.auth);
  const dispatch = useDispatch();

  const links = data.map((item) => (
    <UnstyledButton className={classes.subLink} key={item.title}>
      <Group wrap="nowrap" align="flex-start">
        <ThemeIcon size={34} variant="default" radius="md">
          <item.icon
            style={{ width: rem(22), height: rem(22) }}
            color={theme.colors.blue[6]}
          />
        </ThemeIcon>
        <div>
          <Text size="sm" fw={500}>
            {item.title}
          </Text>
          <Text size="xs" c="dimmed">
            {item.description}
          </Text>
        </div>
      </Group>
    </UnstyledButton>
  ));

  return (
    <Box
      pb={0}
      bg="linear-gradient(
        250deg, rgba(4,26,51,1) 0%, rgba(6,35,67,1) 70%
      )"
      m={0}
    >
      <header className={classes.header}>
        <Group justify="space-between" h="100%">
          <Image h={35} src={Logo} />

          <Group h="100%" gap={0} visibleFrom="sm">
            <Link to={PathConstants.HOME} className={classes.link}>
              Home
            </Link>
            <HoverCard
              width={600}
              position="bottom"
              radius="md"
              shadow="md"
              withinPortal
            >
              <HoverCard.Target>
                <a href="#" className={classes.link}>
                  <Center inline>
                    <Box component="span" mr={5}>
                      Features
                    </Box>
                    <IconChevronDown
                      style={{ width: rem(16), height: rem(16) }}
                      color={theme.colors.blue[6]}
                    />
                  </Center>
                </a>
              </HoverCard.Target>

              <HoverCard.Dropdown style={{ overflow: "hidden" }}>
                <Group justify="space-between" px="md">
                  <Text fw={500}>Features</Text>
                </Group>

                <Divider my="sm" />

                <SimpleGrid cols={2} spacing={0}>
                  {links}
                </SimpleGrid>

                <div className={classes.dropdownFooter}>
                  <Group justify="space-between">
                    <div>
                      <Text fw={500} fz="sm">
                        loring
                      </Text>
                      <Text size="xs" c="dimmed">
                        Explore, Conenct, Experience
                      </Text>
                    </div>
                    <Button
                      variant="default"
                      onClick={(e) => {
                        e.stopPropagation();
                        if (loggedUser.userType == "Unregistered")
                          navigate("/login");
                        else
                          document
                            .querySelector(".main-ev-listing-div")
                            ?.scrollIntoView({
                              behavior: "smooth",
                              block: "start",
                            });
                      }}
                    >
                      Start exploring
                    </Button>
                  </Group>
                </div>
              </HoverCard.Dropdown>
            </HoverCard>
            <a
              className={classes.link}
              onClick={() => {
                navigate("/");
                document
                  .querySelector(".trending-container")
                  ?.scrollIntoView({ behavior: "smooth", block: "end" });
              }}
            >
              Trending
            </a>
            <a
              className={classes.link}
              onClick={() => {
                navigate("/");
                document
                  .querySelector(".main-ev-listing-div")
                  ?.scrollIntoView({ behavior: "smooth", block: "start" });
              }}
            >
              Explore events
            </a>
          </Group>

          <Group visibleFrom="sm">
            {loggedUser.userType == "Unregistered" && (
              <>
                <Button
                  variant="default"
                  onClick={(event) => {
                    event.stopPropagation();
                    navigate("/login");
                  }}
                >
                  Log in
                </Button>
                <Button
                  onClick={(event) => {
                    event.stopPropagation();
                    navigate("/register");
                  }}
                >
                  Sign up
                </Button>
              </>
            )}
            {loggedUser.userType != "Unregistered" && (
              <>
                <Stack
                  gap={0}
                  className={classes.link}
                  onClick={(event) => {
                    event.stopPropagation();
                    if (loggedUser.userType == "Host")
                      navigate("/organizerpage");
                    else if (loggedUser.userType == "Visitor")
                      navigate("/visitorprofile");
                    else if (loggedUser.userType == "Admin")
                      navigate(`${PathConstants.ADMIN_PAGE}`);
                    else navigate("/spaceownerpage");
                  }}
                >
                  <Text mb={0}>{loggedUser.email}</Text>
                  <Text mt={0}>{loggedUser.firstName}</Text>
                </Stack>
                <Button
                  onClick={(event) => {
                    event.stopPropagation();
                    dispatch(logout());
                    navigate("/");
                  }}
                >
                  Log out
                </Button>
              </>
            )}
          </Group>

          <Burger
            opened={drawerOpened}
            onClick={toggleDrawer}
            hiddenFrom="sm"
          />
        </Group>
      </header>

      <Drawer
        opened={drawerOpened}
        onClose={closeDrawer}
        size="100%"
        padding="md"
        title="Navigation"
        hiddenFrom="sm"
        zIndex={1000000}
      >
        <ScrollArea h={`calc(100vh - ${rem(80)})`} mx="-md">
          <Divider my="sm" />

          <a href="#" className={classes.link} style={{ color: "black" }}>
            Home
          </a>
          <UnstyledButton
            className={classes.link}
            onClick={toggleLinks}
            pl="md"
            pr="md"
          >
            <Center inline>
              <Box component="span" mr={5}>
                Features
              </Box>
              <IconChevronDown
                style={{ width: rem(16), height: rem(16) }}
                color={theme.colors.blue[6]}
              />
            </Center>
          </UnstyledButton>
          <Collapse in={linksOpened}>{links}</Collapse>
          <a href="#" className={classes.link} style={{ color: "black" }}>
            Trending
          </a>
          <a href="#" className={classes.link} style={{ color: "black" }}>
            Explore events
          </a>

          <Divider my="sm" />

          <Group justify="center" grow pb="xl" px="md">
            {loggedUser.userType == "Unregistered" && (
              <>
                <Button
                  variant="default"
                  onClick={(event) => {
                    event.stopPropagation();
                    navigate("/login");
                  }}
                >
                  Log in
                </Button>
                <Button
                  onClick={(event) => {
                    event.stopPropagation();
                    navigate("/register");
                  }}
                >
                  Sign up
                </Button>
              </>
            )}
            {loggedUser.userType != "Unregistered" && (
              <>
                <Stack
                  gap={0}
                  onClick={(event) => {
                    event.stopPropagation();
                    if (loggedUser.userType == "Host")
                      navigate("/organizerpage");
                    else if (loggedUser.userType == "Visitor")
                      navigate("/visitorprofile");
                    else if (loggedUser.userType == "Admin")
                      navigate(`${PathConstants.ADMIN_PAGE}`);
                    else navigate("/spaceownerpage");
                  }}
                >
                  <Text mb={0}>{loggedUser.email}</Text>
                  <Text mt={0}>{loggedUser.firstName}</Text>
                </Stack>
                <Button
                  onClick={(event) => {
                    event.stopPropagation();
                    dispatch(logout());
                    navigate("/");
                  }}
                >
                  Log out
                </Button>
              </>
            )}
          </Group>
        </ScrollArea>
      </Drawer>
    </Box>
  );
}
