import { Group, Anchor, Image, Flex, Select } from "@mantine/core";
import Logo from "../../assets/logo.png";
import classes from "./Footer.module.css";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";

export function Footer() {
  const navigate = useNavigate();
  const { t, i18n } = useTranslation();

  const links = [
    { link: "/contact", label: "FooterContact" },
    { link: "/privacy", label: "FooterPrivacy" },
    { link: "/faq", label: "FooterFaq" },
    { link: "/", label: "FooterCopyright" },
  ];

  const changeLanguage = (lng: string) => {
    i18n.changeLanguage(lng);
    localStorage.setItem("i18nextLng", lng);
  };

  const items = links.map((link, idx) => (
    <Anchor<"a">
      c="dimmed"
      key={link.label}
      onClick={(event) => {
        event.preventDefault();
        navigate(link.link);
      }}
      size="md"
      underline={idx == links.length - 1 ? "never" : "hover"}
    >
      {t(link.label)}
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
      <Flex p={30} pl={40} pr={40} className={classes.inner} align="center">
        <Image h={35} src={Logo} />
        <Group className={classes.links}>
          <Select
            size="md"
            w="max-content"
            value={i18n.language}
            onChange={(_value) => changeLanguage(_value!)}
            data={["en", "sr"]}
          />
          {items}
        </Group>
      </Flex>
    </div>
  );
}
