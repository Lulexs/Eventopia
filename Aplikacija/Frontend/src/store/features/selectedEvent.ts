import { createSlice } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit";

export interface SelectedEventState {
  id: number;
  title: string;
  date: string;
  location: string;
  img: string;
}

const initialState: SelectedEventState = {
  id: 0,
  title: "",
  date: "",
  location: "",
  img: "",
};

export const selectedEventSlice = createSlice({
  name: "selectedEvent",
  initialState,
  reducers: {
    setEvent: (state, action: PayloadAction<SelectedEventState>) => {
      state.id = action.payload.id;
      state.title = action.payload.title;
      state.date = action.payload.date;
      state.location = action.payload.location;
      state.img = action.payload.img;
    },
  },
});

export const { setEvent } = selectedEventSlice.actions;

export default selectedEventSlice.reducer;
