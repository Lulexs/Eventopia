import { Button, Flex, Group, Title } from "@mantine/core";
import classes from "./EventReviews.module.css";
import EventBgImage from "../../assets/event_listing_bg_op.png";
import { Comment } from "./Comment";
import View from "../EventViewPages";

export interface EventReviewsProps {
  eventId: number;
  showEvent: React.Dispatch<React.SetStateAction<View>>;
}

export default function EventReviews(props: EventReviewsProps) {
  var reviews = Array.from({ length: 5 });

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
      <Group style={{ width: "100%" }}>
        <Button
          onClick={(event) => {
            event.stopPropagation();
            props.showEvent(View.Basic);
          }}
        >
          Go back
        </Button>
        <Title c="#5a5959">Example title - 25.25.2525.</Title>
      </Group>

      <Flex flex={1} mt={20} wrap="wrap" rowGap="20">
        {reviews.map((_, idx) => (
          <Comment key={idx} />
        ))}
      </Flex>
    </Flex>
  );
}
