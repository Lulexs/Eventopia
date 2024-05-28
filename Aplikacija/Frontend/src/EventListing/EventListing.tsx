import {
  Autocomplete,
  Button,
  Flex,
  Group,
  MultiSelect,
  Title,
} from "@mantine/core";
import { useState } from "react";
import { IconSelect } from "@tabler/icons-react";
import { DateInput } from "@mantine/dates";
import EventBgImage from "../assets/event_listing_bg_op.png";
import EventCard from "./EventCard";
import classes from "./EventListing.module.css";
import axios from "axios";
import { useQuery } from "@tanstack/react-query";
import { Event } from "./interfaces";

const gradOptions = [
  "Nis, Serbia",
  "Belgrade, Serbia",
  "Prague, Checkia",
  "Moscow, Russia",
  "Berlin, Germany",
  "Rome, Italy",
  "Novi Sad, Serbia",
  "Copenhagen, Denmark",
  "Cleavland, USA",
  "LA, USA",
  "San Francisco, USA",
  "Donji Milanovac, Serbia",
];

const organizers = ["Milenium house", "Petar Petrovic"];

const tags = ["Rock", "Hip hop", "Eating", "Old music", "Sport"];

export default function EventListing() {
  const [selectedCity, setSelectedCity] = useState("");
  const [selectedOrganizer, setSelectedOrganizer] = useState("");
  const [selectedTags, setSelectedTags] = useState<string[]>([]);
  const [dateTime, setDateTime] = useState<Date | null>(null);

  const {
    isLoading,
    data: events,
    isError,
  } = useQuery<Event[]>({
    queryKey: ["events"],
    queryFn: async () => {
      return await axios
        .get(`${import.meta.env.VITE_JSON_SERVER}/hotevents`)
        .then((resp) => resp.data);
    },
  });

  return (
    <Flex
      direction="column"
      align="center"
      styles={{
        root: {
          backgroundImage: `url(${EventBgImage})`,
        },
      }}
      pt={40}
      className="main-ev-listing-div"
    >
      <Title
        style={{
          fontFamily: "Greycliff CF, var(--mantine-font-family)",
          fontSize: "3rem",
          color: "white",
          textShadow: `-1px -1px 0 #868e96, 1px -1px 0 #868e96, -1px 1px 0 #868e96, 1px 1px 0 #868e96`,
          textAlign: "center",
        }}
        mb={60}
      >
        Explore, Connect, Experience
      </Title>
      <Group align="flex-end" justify="center" mb={50}>
        <Autocomplete
          data={gradOptions}
          value={selectedCity}
          onChange={setSelectedCity}
          placeholder="Select location..."
          label="Filter by location"
          styles={{
            label: {
              fontFamily: "Greycliff CF, var(--mantine-font-family)",
              fontSize: "1.01rem",
            },
          }}
          rightSection={<IconSelect />}
          rightSectionPointerEvents="none"
          maw={229}
        />
        <Autocomplete
          data={organizers}
          value={selectedOrganizer}
          onChange={setSelectedOrganizer}
          placeholder="Select organizer..."
          label="Filter by organizer"
          styles={{
            label: {
              fontFamily: "Greycliff CF, var(--mantine-font-family)",
              fontSize: "1.01rem",
            },
          }}
          rightSection={<IconSelect />}
          rightSectionPointerEvents="none"
          maw={229}
        />
        <DateInput
          value={dateTime}
          onChange={setDateTime}
          label="Filter by date"
          placeholder="Pick a date..."
          maw={229}
          styles={{
            label: {
              fontFamily: "Greycliff CF, var(--mantine-font-family)",
              fontSize: "1.01rem",
            },
          }}
          rightSection={<IconSelect />}
          rightSectionPointerEvents="none"
        />

        <MultiSelect
          label="Filter by event tags"
          data={tags}
          placeholder="Select tags..."
          value={selectedTags}
          onChange={setSelectedTags}
          checkIconPosition="left"
          withScrollArea={false}
          maw={229}
          rightSection={<IconSelect />}
          styles={{
            label: {
              fontFamily: "Greycliff CF, var(--mantine-font-family)",
              fontSize: "1.01rem",
            },
          }}
        />
        <Button
          style={{
            fontFamily: "Greycliff CF, var(--mantine-font-family)",
            fontSize: "1.01rem",
          }}
          variant="outline"
          color="gray"
          size="md"
        >
          Search
        </Button>
      </Group>
      <Flex
        h="fit-content"
        w="100%"
        p={30}
        pl={40}
        pr={40}
        align="flex-start"
        justify="center"
        gap="30px"
        wrap="wrap"
      >
        {isLoading || isError ? (
          <div className={classes.controls}>
            <div className={classes.ldsRing}>
              <div></div>
              <div></div>
              <div></div>
              <div></div>
            </div>
          </div>
        ) : (
          events?.map((ev: Event, idx: number) => (
            <EventCard key={idx} event={ev} />
          ))
        )}
      </Flex>
    </Flex>
  );
}
