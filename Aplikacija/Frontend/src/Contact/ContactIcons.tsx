import { Text, Box, Stack, rem } from "@mantine/core";
import { IconSun, IconPhone, IconMapPin, IconAt } from "@tabler/icons-react";
import classes from "./ContactIcons.module.css";
import { useTranslation } from "react-i18next";

interface ContactIconProps
  extends Omit<React.ComponentPropsWithoutRef<"div">, "title"> {
  icon: typeof IconSun;
  title: React.ReactNode;
  description: React.ReactNode;
}

function ContactIcon({
  icon: Icon,
  title,
  description,
  ...others
}: ContactIconProps) {
  return (
    <div className={classes.wrapper} {...others}>
      <Box mr="md">
        <Icon style={{ width: rem(24), height: rem(24) }} />
      </Box>

      <div>
        <Text size="xs" className={classes.title}>
          {title}
        </Text>
        <Text className={classes.description}>{description}</Text>
      </div>
    </div>
  );
}

export function ContactIconsList() {
  const { t } = useTranslation();

  const Data = [
    { title: t("Email"), description: "support@eventopia.org", icon: IconAt },
    { title: t("Phone"), description: "+381 61 251 12 52", icon: IconPhone },
    { title: t("Address"), description: "Dead center 58", icon: IconMapPin },
    {
      title: t("WorkingHours"),
      description: "8 a.m. – 11 p.m.",
      icon: IconSun,
    },
  ];

  const items = Data.map((item, index) => (
    <ContactIcon key={index} {...item} />
  ));
  return <Stack>{items}</Stack>;
}
