import { Text, Progress, Card } from "@mantine/core";
import classes from "./UserProfile.module.css";

export interface StatsCardProps {
  title: string;
  level: string;
  current: number;
  nextStage: number;
}

export function StatsCard(props: StatsCardProps) {
  return (
    <Card
      className={classes.statisticsCard}
      withBorder
      radius="md"
      padding="xl"
      bg="var(--mantine-color-body)"
    >
      <Text fz="xs" tt="uppercase" fw={700} c="dimmed" ta="center">
        {props.title}
        {" - "}
        {props.level}
      </Text>
      <Text fz="lg" fw={500} ta="center">
        {`${props.current} / ${props.nextStage}`}
      </Text>
      <Progress
        value={(props.current / props.nextStage) * 100}
        mt="md"
        size="lg"
        radius="xl"
      />
    </Card>
  );
}
