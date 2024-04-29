import { Flex } from "@mantine/core";
import { Footer } from "../HomePage/Footer/Footer";
import { HeaderMegaMenu } from "../HomePage/HeaderMegaMenu/HeaderMegaMenu";
import { ContactUs } from "./ContactUs";
import EventBgImage from "../assets/event_listing_bg_op.png";

export default function ContactPage() {
  return (
    <Flex
      direction="column"
      styles={{
        root: {
          backgroundImage: `url(${EventBgImage})`,
        },
      }}
      h="100vh"
    >
      <HeaderMegaMenu />
      <Flex m={20} justify="center" align="center" flex="1">
        <ContactUs />
      </Flex>
      <Footer />
    </Flex>
  );
}
