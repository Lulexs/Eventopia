import { Container, Title, Text, Button } from "@mantine/core";
import classes from "./HeroImageRight.module.css";
import { useNavigate } from "react-router-dom";
import { useSelector } from "react-redux";
import { RootState } from "../../store/store";

export function HeroImageRight() {
  const navigate = useNavigate();
  const loggedUser = useSelector((state: RootState) => state.auth);

  return (
    <div className={classes.root}>
      <Container size="lg">
        <div className={classes.inner}>
          <div className={classes.content}>
            <Title className={classes.title}>
              <Text
                component="span"
                inherit
                variant="gradient"
                gradient={{ from: "pink", to: "yellow" }}
              >
                Explore, Connect, Experience:
              </Text>{" "}
              Your Gateway to Event Discovery
            </Title>

            <Text
              className={classes.description}
              mt={30}
              style={{ color: "white" }}
            >
              Welcome to the ultimate hub for event enthusiasts! Step into a
              world where every moment holds the promise of excitement and
              discovery. Our dynamic event management platform not only empowers
              organizers but also invites you to embark on a journey of
              exploration like never before.
            </Text>

            <Button
              variant="gradient"
              gradient={{ from: "pink", to: "yellow" }}
              size="xl"
              className={classes.control}
              mt={40}
              onClick={(e) => {
                e.stopPropagation();
                if (loggedUser.userType == "Unregistered") navigate("/login");
                else
                  document
                    .querySelector(".main-ev-listing-div")
                    ?.scrollIntoView({ behavior: "smooth", block: "start" });
              }}
            >
              Start exploring
            </Button>
          </div>
        </div>
      </Container>
    </div>
  );
}
