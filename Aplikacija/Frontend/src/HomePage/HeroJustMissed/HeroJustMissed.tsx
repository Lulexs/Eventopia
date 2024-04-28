import { Title, Text, Button, Container } from "@mantine/core";
import { Dots } from "./Dots";
import classes from "./HeroJustMissed.module.css";
import { HighlightsCarousel } from "./HighlightsCarousel";
import { HeroJustMissedProps } from "../interfaces";

export function HeroJustMissed(props: HeroJustMissedProps) {
  return (
    <Container className={`${classes.wrapper} trending-container`} size={1400}>
      <Dots className={classes.dots} style={{ left: 0, top: 0 }} />
      <Dots className={classes.dots} style={{ left: 60, top: 0 }} />
      <Dots className={classes.dots} style={{ left: 0, top: 140 }} />
      <Dots className={classes.dots} style={{ right: 0, top: 60 }} />

      <div className={classes.inner}>
        <Title className={classes.title} mb={60}>
          <Text
            component="span"
            inherit
            variant="gradient"
            gradient={{ from: "pink", to: "yellow" }}
          >
            Check out some of the trending highlights
          </Text>
          <Text
            inherit
            variant="gradient"
            gradient={{ from: "pink", to: "yellow" }}
          >
            from past week
          </Text>
        </Title>

        {props.isLoading || props.isError ? (
          <div className={classes.controls}>
            <div className={classes.ldsRing}>
              <div></div>
              <div></div>
              <div></div>
              <div></div>
            </div>
          </div>
        ) : (
          <HighlightsCarousel highlightsUrls={props.CarouselProps} />
        )}

        <div className={classes.controls}>
          <Button
            className={classes.control}
            size="lg"
            variant="default"
            color="gray"
          >
            Start exploring
          </Button>
          <Button
            variant="gradient"
            gradient={{ from: "pink", to: "yellow" }}
            className={classes.control}
            size="lg"
          >
            Start exploring
          </Button>
        </div>
      </div>
    </Container>
  );
}
