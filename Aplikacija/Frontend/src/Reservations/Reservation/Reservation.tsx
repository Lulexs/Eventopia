import { useQuery } from "@tanstack/react-query";
import axios from "axios";
import { CSSProperties } from "react";
import classes from "./Reservation.module.css";
import { Flex } from "@mantine/core";
import { SpaceDataType } from "./interfaces";
import TableImage from "../../assets/table.png";
import BarImage from "../../assets/bar.png";
import StageImage from "../../assets/stage.png";

const widthPercent = 45;
const heightPercent = 80;

const styles: CSSProperties = {
  width: `${widthPercent}%`,
  height: `${heightPercent}%`,
  borderRadius: "20px",
  overflow: "hidden",
  border: "none",
  backgroundColor: "gray",
  position: "relative",
};

export default function Reservation() {
  const { isLoading, isError, data } = useQuery<SpaceDataType>({
    queryKey: ["reservedSpace"],
    queryFn: async () => {
      return await axios
        .get(`${import.meta.env.VITE_JSON_SERVER}/space1`)
        .then((resp) => {
          console.log(resp.data);
          return resp.data;
        });
    },
  });

  return (
    <Flex align="center" justify="center" direction="row" w="100%" h="100vh">
      <Flex w="55%"></Flex>
      {isLoading || isError ? (
        <div className={classes.ldsRing}>
          <div></div>
          <div></div>
          <div></div>
          <div></div>
        </div>
      ) : (
        <div
          className="main-surface-container"
          style={{ ...styles, textAlign: "center", zIndex: 1 }}
        >
          {data?.items.map((item) => {
            {
              if (item.type == "table") {
                return (
                  <img
                    key={item.id}
                    style={{
                      position: "absolute",
                      top:
                        (item.left * document.body.offsetWidth * widthPercent) /
                        100 /
                        data.surfaceDimension.width,
                      left:
                        (item.top *
                          document.body.clientHeight *
                          heightPercent) /
                        100 /
                        data.surfaceDimension.height,
                    }}
                    height={`${item.height * item.heightFactor}%`}
                    src={TableImage}
                  />
                );
              } else {
                return (
                  <img
                    key={item.id}
                    style={{
                      position: "absolute",
                      top:
                        (item.left * document.body.offsetWidth * widthPercent) /
                        100 /
                        data.surfaceDimension.width,
                      left:
                        (item.top *
                          document.body.clientHeight *
                          heightPercent) /
                        100 /
                        data.surfaceDimension.height,
                    }}
                    height={`${item.height * item.heightFactor}%`}
                    src={item.type == "bar" ? BarImage : StageImage}
                  />
                );
              }
            }
          })}
          <svg style={{ width: "100%", height: "100%", zIndex: 2 }}>
            {data?.lines.map((line, index) => (
              <line
                key={index}
                x1={line.x1}
                y1={line.y2}
                x2={line.x2}
                y2={line.y2}
                style={{
                  stroke: "white",
                  strokeWidth: 2,
                }}
              />
            ))}
          </svg>
        </div>
      )}
    </Flex>
  );
}
