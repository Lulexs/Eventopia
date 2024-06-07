import {
  Text,
  Title,
  SimpleGrid,
  TextInput,
  Textarea,
  Button,
  Group,
  ActionIcon,
} from "@mantine/core";
import {
  IconBrandTwitter,
  IconBrandYoutube,
  IconBrandInstagram,
} from "@tabler/icons-react";
import { ContactIconsList } from "./ContactIcons";
import classes from "./ContactUs.module.css";
import { useTranslation } from "react-i18next";

const social = [IconBrandTwitter, IconBrandYoutube, IconBrandInstagram];

export function ContactUs() {
  const { t } = useTranslation();

  const icons = social.map((Icon, index) => (
    <ActionIcon
      key={index}
      size={28}
      className={classes.social}
      variant="transparent"
    >
      <Icon size="1.4rem" stroke={1.5} />
    </ActionIcon>
  ));

  return (
    <div className={classes.wrapper}>
      <SimpleGrid cols={{ base: 1, sm: 2 }} spacing={50}>
        <div>
          <Title className={classes.title}>{t("ContactUs")}</Title>
          <Text className={classes.description} mt="sm" mb={30} c="white">
            {t("LeaveEmail")}
          </Text>

          <ContactIconsList />

          <Group mt="xl">{icons}</Group>
        </div>
        <div className={classes.form}>
          <TextInput
            label={t("Email")}
            placeholder={t("EmailP")}
            required
            classNames={{ input: classes.input, label: classes.inputLabel }}
          />
          <TextInput
            label={t("Name")}
            placeholder={t("NameP")}
            mt="md"
            classNames={{ input: classes.input, label: classes.inputLabel }}
          />
          <Textarea
            required
            label={t("YourMessage")}
            placeholder={t("YourMessageP")}
            minRows={4}
            mt="md"
            classNames={{ input: classes.input, label: classes.inputLabel }}
          />

          <Group justify="flex-end" mt="md">
            <Button className={classes.control}>{t("SendMessage")}</Button>
          </Group>
        </div>
      </SimpleGrid>
    </div>
  );
}
