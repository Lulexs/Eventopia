import { PayloadAction, createSlice } from "@reduxjs/toolkit";

export interface AuthState {
  userId: number;
  username: string;
  email: string;
  userType: "Visitor" | "Space owner" | "Host" | "Unregistered";
}

const initialState: AuthState = {
  userId: 0,
  username: "",
  email: "",
  userType: "Unregistered",
};

export const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {
    login: (state, action: PayloadAction<AuthState>) => {
      state.email = action.payload.email;
      state.username = action.payload.username;
      state.userId = action.payload.userId;
      state.userType = action.payload.userType;
    },
    logout: (state) => {
      Object.assign(state, initialState);
    },
  },
});

export const { login, logout } = authSlice.actions;
export default authSlice.reducer;
