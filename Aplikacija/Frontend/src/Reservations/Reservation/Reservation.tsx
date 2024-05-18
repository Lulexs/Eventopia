import { useQuery } from "@tanstack/react-query";
import axios from "axios";
import classes from "./Reservation.module.css";
import {
  Button,
  Divider,
  Flex,
  Title,
  Text,
  Image,
  Anchor,
  Group,
} from "@mantine/core";
import { SpaceDataType, TableInterface } from "./interfaces";
import PartiBgImage from "../../assets/partibg.png";
import BarImage from "../../assets/bar.png";
import StageImage from "../../assets/stage.png";
import Katanac from "../../assets/lock.png";
import { useNavigate } from "react-router-dom";
import Table from "./Table";
import MapComponent from "./MapComponent";
import { useSelector } from "react-redux";
import { RootState } from "../../store/store";
import { useState } from "react";

export interface ReservationProps {
  id: number;
  title: string;
  location: string;
  date: string;
  img: string;
  time: string;
  organizerID: number;
  organizerName: string;
}

export default function Reservation(props: ReservationProps) {
  const navigate = useNavigate();
  const isUserLoggedIn = useSelector((state: RootState) => state.auth);

  const [showMap, setShowMap] = useState(false);

  const { isLoading, isError, data } = useQuery<SpaceDataType>({
    queryKey: ["reservedSpace"],
    queryFn: async () => {
      return await axios
        .get(`${import.meta.env.VITE_JSON_SERVER}/space`)
        .then((resp) => {
          console.log(resp.data);
          return resp.data;
        });
    },
  });

  return (
    <>
      <Flex
        className={classes.container}
        style={{
          backgroundImage: `url(${PartiBgImage})`,
          backgroundSize: "contain",
        }}
      >
        <Title
          style={{
            fontFamily: "Greycliff CF, var(--mantine-font-family)",
            fontSize: "3rem",
            color: "#453636",
            textShadow: `-1px -1px 0 #DBD8D8, 1px -1px 0 #DBD8D8, -1px 1px 0 #DBD8D8, 1px 1px 0 #DBD8D8`,
            textAlign: "center",
          }}
          mb={10}
        >
          {props.title}
          <Divider color="#453636" />
        </Title>
        <Flex w="100%" h="100vh" justify="center" gap="70px">
          <Flex className={classes.infoContainer}>
            <Text
              style={{
                fontFamily: "Greycliff CF, var(--mantine-font-family)",
                fontSize: "1.5rem",
                color: "#453636",
              }}
            >{`@${props.location}`}</Text>
            <Text
              style={{
                fontFamily: "Greycliff CF, var(--mantine-font-family)",
                fontSize: "1.4rem",
                color: "#453636",
              }}
            >{`${props.date} ${props.time}`}</Text>
            <Image
              width="100%"
              src={props.img}
              style={{ borderRadius: "20px" }}
            />
            <Anchor
              onClick={(event) => {
                event.stopPropagation();
                navigate("/organizerinfo");
              }}
              mb="xl"
            >
              <Text
                style={{
                  fontFamily: "Greycliff CF, var(--mantine-font-family)",
                  fontSize: "1.4rem",
                  color: "#453636",
                }}
              >
                {`By ${props.organizerName}`}
              </Text>
            </Anchor>
            <Group>
              <Text
                style={{
                  fontFamily: "Greycliff CF, var(--mantine-font-family)",
                  fontSize: "1.2rem",
                  color: "#453636",
                }}
                onClick={(event) => {
                  event.stopPropagation();
                  setShowMap(false);
                }}
              >
                Details
                <span style={{ fontSize: "1.6rem" }}>
                  {showMap ? "↓" : "↑"}
                </span>
              </Text>
              <Text
                style={{
                  fontFamily: "Greycliff CF, var(--mantine-font-family)",
                  fontSize: "1.2rem",
                  color: "#453636",
                }}
                onClick={(event) => {
                  event.stopPropagation();
                  setShowMap(true);
                }}
              >
                Map
                <span style={{ fontSize: "1.6rem" }}>
                  {!showMap ? "↓" : "↑"}
                </span>
              </Text>
            </Group>

            <Flex w="100%" h="40vh" mah="400px" mih="200px">
              {showMap ? (
                <MapComponent />
              ) : (
                <Text
                  style={{
                    fontFamily: "Greycliff CF, var(--mantine-font-family)",
                    fontSize: "1.1rem",
                    color: "#453636",
                  }}
                >
                  Ovaj dogadjaj je za sve ljubitelje dogadjaja. Ovaj dogadjaj je
                  za sve ljubitelje dogadjaja Ovaj dogadjaj je za sve ljubitelje
                  dogadjaja Ovaj dogadjaj je za sve ljubitelje dogadjaja
                </Text>
              )}
            </Flex>
          </Flex>

          {isLoading || isError ? (
            <div className={classes.ldsRing}>
              <div></div>
              <div></div>
              <div></div>
              <div></div>
            </div>
          ) : (
            <div
              className={classes.mainContentContainer}
              style={{
                zIndex: 1,
                backgroundColor: "grey",
                overflow: "auto",
              }}
            >
              {isUserLoggedIn.userType != "Visitor" && (
                <Flex
                  direction="column"
                  align="center"
                  justify="center"
                  gap="20px"
                  bg="grey"
                  h="100%"
                  w="100%"
                >
                  <img src={Katanac} style={{ height: "20%" }}></img>
                  <Button
                    onClick={(event) => {
                      event.stopPropagation();
                      navigate("/login");
                    }}
                  >
                    Login as visitor to get the ticket
                  </Button>
                </Flex>
              )}
              {isUserLoggedIn.userType == "Visitor" && (
                <>
                  {data?.items.map((item) => {
                    {
                      if (item.type == "table") {
                        return (
                          <Table key={item.id} item={item as TableInterface} />
                        );
                      } else {
                        return (
                          <img
                            key={item.id}
                            style={{
                              position: "absolute",
                              top: item.top,
                              left: item.left,
                            }}
                            height={`${item.height * item.heightFactor}%`}
                            src={item.type == "bar" ? BarImage : StageImage}
                          />
                        );
                      }
                    }
                  })}
                  <svg
                    style={{
                      width: data?.surfaceDimension.width,
                      height: data?.surfaceDimension.height,
                      position: "absolute",
                      top: 0,
                      left: 0,
                    }}
                  >
                    {data?.lines.map((line, index) => (
                      <line
                        key={index}
                        x1={line.x1}
                        y1={line.y1}
                        x2={line.x2}
                        y2={line.y2}
                        style={{
                          stroke: "black",
                          strokeWidth: 2,
                        }}
                      />
                    ))}
                  </svg>
                </>
              )}
            </div>
          )}
        </Flex>
      </Flex>
    </>
  );
}
