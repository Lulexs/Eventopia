import { ItemInterface, Line } from "../Reservations/Reservation/interfaces";

export interface SpaceBasic {
  id: number;
  city: string;
  country: string;
  address: string;
  capacity: number;
}

export interface NewEventDto {
  eventName: string;
  description: string;
  tags: string[];
  date: string;
  time: string;
  video: string;
  spaceId: number;
  items: ItemInterface[];
  lines: Line[];
  surfaceDimension: { width: number; height: number };
}

export interface HostStatistics {
  hostedEvents: number;
  averageRating: number;
  reservations: number;
  estimatedEarnings: number;
}

export interface Review {
  avatar: string;
  name: string;
  rating: number;
  comment: string;
  time: string;
}
