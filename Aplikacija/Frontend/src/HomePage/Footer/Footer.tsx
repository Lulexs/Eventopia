import { Group, Anchor, Image, Flex } from "@mantine/core";
import Logo from "../../assets/logo.png";
import classes from "./Footer.module.css";

const links = [
  { link: "#", label: "Contact" },
  { link: "#", label: "Privacy" },
  { link: "#", label: "Blog" },
  { link: "#", label: "©Copyright 2024 AverageDebuggingEnjoyers" },
];

export function Footer() {
  const items = links.map((link, idx) => (
    <Anchor<"a">
      c="dimmed"
      key={link.label}
      href={link.link}
      onClick={(event) => event.preventDefault()}
      size="md"
      underline={idx == links.length - 1 ? "never" : "hover"}
    >
      {link.label}
    </Anchor>
  ));

  return (
    <div
      className={classes.footer}
      style={{
        paddingBottom: 0,
        background:
          "linear-gradient(250deg, rgba(4,26,51,1) 0%, rgba(6,35,67,1) 70%)",
      }}
    >
      <Flex p={30} pl={40} pr={40} className={classes.inner}>
        <Image h={30} src={Logo} />
        <Group className={classes.links}>{items}</Group>
      </Flex>
    </div>
  );
}
