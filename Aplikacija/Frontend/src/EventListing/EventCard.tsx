import { Card, Image, Text, Button, Group } from "@mantine/core";
import { EventCardProps } from "./interfaces";
import { IconCalendar, IconFlame, IconMapPin } from "@tabler/icons-react";

export default function EventCard(props: EventCardProps) {
  return (
    <Card shadow="sm" padding="lg" radius="lg" withBorder w="20%" miw={300}>
      <Card.Section>
        <Image
          src={new URL(props.event.img, import.meta.url).href}
          height={160}
          alt={`Couldn't load ${props.event.title} image`}
          fit="cover"
        />
      </Card.Section>

      <Group justify="space-between" mt="md" mb="lg">
        <Text fw={500}>{props.event.title}</Text>
      </Group>

      <Group mb="md">
        <IconMapPin />
        <Text fw={500}>{props.event.location}</Text>
      </Group>

      <Group mb="md">
        <IconCalendar />
        <Text fw={500}>{props.event.date}</Text>
      </Group>

      <Button
        variant="outline"
        color="gray"
        size="md"
        fullWidth
        mt="md"
        radius="md"
      >
        Reserve seats now{" "}
        <IconFlame color="var(--mantine-color-red-filled)" stroke={2.5} />
      </Button>
    </Card>
  );
}
