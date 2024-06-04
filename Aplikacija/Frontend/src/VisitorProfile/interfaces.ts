import { AuthState } from "../store/features/auth";

export interface VisitorProfileProps {
  user: AuthState;
}

export interface VisitorStatistics {
  visitedEventsCount: number;
  moneySpent: number;
  rankName: string;
  points: number;
  nextRankPoints: number;
}

export interface ActiveReservations {
  reservationId: number;
  title: string;
  date: Date;
  image: string;
  price: number;
}

export interface VisitedEvents {
  title: string;
  date: Date;
  image: string;
}
