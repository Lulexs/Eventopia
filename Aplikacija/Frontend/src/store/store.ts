import { configureStore } from "@reduxjs/toolkit";
import selectedEventReducer from "./features/selectedEvent";

export const store = configureStore({
  reducer: {
    selectedEvent: selectedEventReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
