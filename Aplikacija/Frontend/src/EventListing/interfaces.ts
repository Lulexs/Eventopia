export interface Event {
  img: string;
  title: string;
  location: string;
  date: string;
}

export interface EventCardProps {
  event: Event;
}

export interface EventListingProps {
  events: Event[];
  isLoading: boolean;
  isError: boolean;
}
