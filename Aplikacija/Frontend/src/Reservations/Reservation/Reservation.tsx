import { useQuery } from "@tanstack/react-query";
import axios from "axios";
import classes from "./Reservation.module.css";
import { Button, Divider, Flex, Title, Text, Image } from "@mantine/core";
import { SpaceDataType, TableInterface } from "./interfaces";
import PartiBgImage from "../../assets/partibg.png";
import TableFreeImage from "../../assets/table_free.png";
import TableNotFreeImage from "../../assets/table_not_free.png";
import BarImage from "../../assets/bar.png";
import StageImage from "../../assets/stage.png";
import Katanac from "../../assets/lock.png";

export interface ReservationProps {
  title: string;
  location: string;
  date: string;
  img: string;
}

export default function Reservation(props: ReservationProps) {
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
        <Flex className={classes.infoContainer}>
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
          >{`${props.date}`}</Text>
          <Image
            width="100%"
            src={props.img}
            style={{ borderRadius: "20px" }}
          />
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
              textAlign: "center",
              zIndex: 1,
            }}
          >
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
              <Button>Login to get the ticket</Button>
            </Flex>
            {/* {data?.items.map((item) => {
              {
                if (item.type == "table") {
                  return (
                    <img
                      key={item.id}
                      style={{
                        position: "absolute",
                        top: item.top,
                        left: item.left,
                      }}
                      height={`${item.height * item.heightFactor}%`}
                      src={
                        (item as TableInterface).reserved
                          ? TableNotFreeImage
                          : TableFreeImage
                      }
                    />
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
                zIndex: 2,
                overflow: "auto",
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
            </svg> */}
          </div>
        )}
      </Flex>
    </>
  );
}
