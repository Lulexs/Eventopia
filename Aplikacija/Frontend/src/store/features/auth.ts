import { PayloadAction, createSlice } from "@reduxjs/toolkit";

export interface AuthState {
  userId: string;
  token: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  userType: "Visitor" | "Space owner" | "Host" | "Unregistered" | "Admin";
  avatar: string;
}

const initialState: AuthState = {
  userId: "",
  token: "",
  username: "TEST USERNAME",
  firstName: "TEST",
  lastName: "TEST",
  email: "",
  userType: "Unregistered",
  avatar: "",
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
      state.firstName = action.payload.firstName;
      state.lastName = action.payload.lastName;
      state.avatar = action.payload.avatar;
      state.token = action.payload.token;
    },
    logout: (state) => {
      Object.assign(state, initialState);
    },
  },
});

export const { login, logout } = authSlice.actions;
export default authSlice.reducer;
