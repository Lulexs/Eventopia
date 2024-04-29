import { Image, Accordion, Grid, Container, Title, Flex } from "@mantine/core";
import image from "../assets/faq.svg";
import classes from "./Faq.module.css";
import { HeaderMegaMenu } from "../HomePage/HeaderMegaMenu/HeaderMegaMenu";
import { Footer } from "../HomePage/Footer/Footer";
import EventBgImage from "../assets/event_listing_bg_op.png";

const faqs = [
  {
    value: "ticket-reservation",
    question: "How can I reserve ticket for an event?",
    answer:
      "To reserve ticket, you have to be signed in. After that select event that interests you and click reserve ticket. You will be redirected to an interactive ticket reservation system. Pick a seat that most suits you and enjoy!",
  },
  {
    value: "event-schedule",
    question: "What is the schedule for upcoming events?",
    answer:
      "You can find the schedule for upcoming events by visiting our website's event calendar. It's regularly updated with information on dates, times, and locations for all our scheduled events.",
  },
  {
    value: "payment-refunds",
    question:
      "How can I make a payment for my ticket, and what is the refund policy?",
    answer:
      "At the moment we are not supporting online payement. After seat is reserved you will be paying at site.",
  },
  {
    value: "accessibility-accommodations",
    question:
      "Do you provide accommodations for attendees with accessibility needs?",
    answer: "For such questions contact event organizer.",
  },
  {
    value: "group-discounts",
    question:
      "Are there any discounts available for group bookings or special offers for frequent attendees?",
    answer:
      "We offer group discounts for bulk bookings, as well as special promotions for loyal attendees. Keep an eye on our website and social media channels for announcements about these offers.",
  },
  {
    value: "event-cancellations",
    question: "What happens if an event is canceled or postponed?",
    answer:
      "In the rare event of a cancellation or postponement, we will notify all registered attendees via email and provide information on next steps, including options for rescheduling. We endeavor to communicate any changes as promptly and transparently as possible.",
  },
  {
    value: "venue-information",
    question: "Where can I find information about the venue?",
    answer:
      "You can find detailed information about the venue, including address, parking details, and facilities available, on the event page or our website. If you have any specific questions about the venue, feel free to reach out to our support team.",
  },
];

export function Faq() {
  return (
    <Flex
      h="100vh"
      direction="column"
      styles={{
        root: {
          backgroundImage: `url(${EventBgImage})`,
        },
      }}
    >
      <HeaderMegaMenu />
      <div className={classes.wrapper} style={{ flex: 1 }}>
        <Container size="lg">
          <Grid id="faq-grid" gutter={50}>
            <Grid.Col span={{ base: 12, md: 6 }}>
              <Image src={image} alt="Frequently Asked Questions" />
            </Grid.Col>
            <Grid.Col span={{ base: 12, md: 6 }}>
              <Title order={2} ta="left" className={classes.title}>
                Frequently Asked Questions
              </Title>

              <Accordion
                chevronPosition="right"
                defaultValue="reset-password"
                variant="separated"
              >
                {faqs.map((faq, idx) => (
                  <Accordion.Item
                    className={classes.item}
                    value={faq.value}
                    key={idx}
                  >
                    <Accordion.Control>{faq.question}</Accordion.Control>
                    <Accordion.Panel>{faq.answer}</Accordion.Panel>
                  </Accordion.Item>
                ))}
              </Accordion>
            </Grid.Col>
          </Grid>
        </Container>
      </div>
      <Footer />
    </Flex>
  );
}
