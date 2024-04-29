import { Container, Flex, Paper, Text } from "@mantine/core";
import { HeaderMegaMenu } from "../HomePage/HeaderMegaMenu/HeaderMegaMenu";
import { Footer } from "../HomePage/Footer/Footer";
import EventBgImage from "../assets/event_listing_bg_op.png";

export default function Privacy() {
  return (
    <Flex
      direction="column"
      h="100%"
      styles={{
        root: {
          backgroundImage: `url(${EventBgImage})`,
          backgroundRepeat: "repeat",
        },
      }}
    >
      <HeaderMegaMenu />
      <Container size="sm">
        <Paper p="lg">
          <Text ta="center" size="lg" style={{ marginBottom: 20 }}>
            Privacy Policy for Eventopia Event Management System
          </Text>
          <Text ta="justify">
            At Eventopia, we are committed to protecting the privacy and
            security of your personal information. This Privacy Policy explains
            how we collect, use, and disclose your personal data when you use
            our event management system.
          </Text>
          <Text ta="justify" style={{ marginBottom: 10, marginTop: 10 }}>
            <strong>1. Information We Collect:</strong>
          </Text>
          <Text ta="justify">
            <strong>Personal Information:</strong> When you register an account
            with us, we may collect personal information such as your name,
            email address, contact number, and billing information.
          </Text>
          <Text ta="justify">
            <strong>Event Information:</strong> We may collect information about
            the events you attend or show interest in, including event
            preferences and ticket purchases.
          </Text>
          <Text ta="justify">
            <strong>Usage Data:</strong> We collect information about your
            interactions with our platform, including log data, device
            information, and location data.
          </Text>
          <Text ta="justify" style={{ marginBottom: 10, marginTop: 10 }}>
            <strong>2. How We Use Your Information:</strong>
          </Text>
          <Text ta="justify">
            <strong>To Provide Services:</strong> We use your personal
            information to provide and maintain our event management services,
            including processing ticket reservations, sending event updates, and
            facilitating payments.
          </Text>
          <Text ta="justify">
            <strong>Communication:</strong> We may use your contact information
            to send you important updates, newsletters, promotional offers, and
            other communications related to our services.
          </Text>
          <Text ta="justify" style={{ marginBottom: 10, marginTop: 10 }}>
            <strong>3. Data Sharing and Disclosure:</strong>
          </Text>
          <Text ta="justify">
            <strong>Third-Party Service Providers:</strong> We may share your
            personal information with third-party service providers who assist
            us in operating our platform, processing payments, or delivering
            services on our behalf.
          </Text>
          <Text ta="justify">
            <strong>Legal Compliance:</strong> We may disclose your information
            in response to lawful requests by public authorities, including to
            meet national security or law enforcement requirements.
          </Text>
          <Text ta="justify" style={{ marginBottom: 10, marginTop: 10 }}>
            <strong>4. Data Security:</strong>
          </Text>
          <Text ta="justify">
            <strong>
              We implement industry-standard security measures to protect your
              personal information from unauthorized access, alteration,
              disclosure, or destruction.
            </strong>
          </Text>
          <Text ta="justify" style={{ marginBottom: 10, marginTop: 10 }}>
            <strong>5. Your Rights:</strong>
          </Text>
          <Text ta="justify">
            <strong>Access and Control:</strong> You have the right to access,
            correct, or delete your personal information. You may also request
            restrictions on the processing of your data or object to certain
            uses.
          </Text>
          <Text ta="justify" style={{ marginBottom: 10, marginTop: 10 }}>
            <strong>6. Changes to This Policy:</strong>
          </Text>
          <Text ta="justify">
            <strong>
              We reserve the right to update or modify this Privacy Policy at
              any time. Any changes will be effective immediately upon posting
              on our website.
            </strong>
          </Text>
          <Text ta="justify" style={{ marginBottom: 10, marginTop: 10 }}>
            <strong>7. Contact Us:</strong>
          </Text>
          <Text ta="justify">
            <strong>
              If you have any questions or concerns about our Privacy Policy or
              the handling of your personal information, please contact us at
              [Your Contact Information].
            </strong>
          </Text>
          <Text ta="justify">
            This Privacy Policy applies solely to information collected by our
            event management system and does not cover any third-party websites
            or services linked to our platform.
          </Text>{" "}
        </Paper>
      </Container>
      <Footer />
    </Flex>
  );
}
