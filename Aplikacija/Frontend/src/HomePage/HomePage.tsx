import { useQuery } from "@tanstack/react-query";
import { HeaderMegaMenu } from "./HeaderMegaMenu/HeaderMegaMenu";
import { HeroImageRight } from "./HeroIamgeRight/HeroImageRight";
import { HeroJustMissed } from "./HeroJustMissed/HeroJustMissed";
import axios from "axios";
import EventListing from "../EventListing/EventListing";
import { Footer } from "./Footer/Footer";

export default function HomePage() {
  const { isLoading, data, isError } = useQuery({
    queryKey: ["highlights"],
    queryFn: async () => {
      return await axios
        .get(`${import.meta.env.VITE_JSON_SERVER}/highlights`)
        .then((resp) => resp.data);
    },
  });

  return (
    <>
      <HeaderMegaMenu />
      <HeroImageRight />
      <HeroJustMissed
        isLoading={isLoading}
        isError={isError}
        CarouselProps={data}
      />
      <EventListing />
      <Footer />
    </>
  );
}
