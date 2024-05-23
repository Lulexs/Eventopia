import {
  Paper,
  TextInput,
  PasswordInput,
  Button,
  Title,
  Text,
  Anchor,
} from "@mantine/core";
import classes from "./LoginPage.module.css";
import { HeaderMegaMenu } from "../HomePage/HeaderMegaMenu/HeaderMegaMenu";
import { Footer } from "../HomePage/Footer/Footer";
import { useState } from "react";
import { ForgotPassword } from "./ForgotPassword";
import { useNavigate } from "react-router-dom";
import { useForm } from "@mantine/form";
import { useDispatch } from "react-redux";
import { login } from "../store/features/auth";

export function LoginPage() {
  const loginForm = useForm({
    mode: "controlled",
    initialValues: { email: "", password: "" },

    validate: {
      email: (value) => (/^\S+@\S+$/.test(value) ? null : "Invalid email"),
      password: (value) => (value.length > 0 ? null : "Empty password field"),
    },
  });

  const dispatch = useDispatch();

  const navigate = useNavigate();
  const [forgotPassword, setForgotPassword] = useState(false);
  return (
    <>
      <HeaderMegaMenu />
      <div className={classes.wrapper}>
        <Paper className={classes.form} radius={0} p={30}>
          {forgotPassword ? (
            <ForgotPassword
              rememberedPassword={() => setForgotPassword(false)}
            />
          ) : (
            <>
              <Title
                order={2}
                className={classes.title}
                ta="center"
                mt="md"
                mb={50}
              >
                Welcome back to Eventopia!
              </Title>

              <form
                onSubmit={loginForm.onSubmit((values, event) => {
                  event?.stopPropagation();
                  dispatch(
                    login({
                      userId: 0,
                      username: "TEST USERNAME",
                      email: values.email,
                      userType: "Admin",
                      firstName: "TEST",
                      lastName: "TEST",
                      avatar:
                        "https://raw.githubusercontent.com/mantinedev/mantine/master/.demo/avatars/avatar-6.png",
                    })
                  );
                  navigate("/");
                })}
              >
                <TextInput
                  label="Email address"
                  placeholder="hello@gmail.com"
                  size="md"
                  key={loginForm.key("email")}
                  {...loginForm.getInputProps("email")}
                />
                <PasswordInput
                  label="Password"
                  placeholder="Your password"
                  mt="md"
                  size="md"
                  key={loginForm.key("password")}
                  {...loginForm.getInputProps("password")}
                />
                <Button
                  type="submit"
                  fullWidth
                  mt="xl"
                  size="md"
                  onClick={() => {}}
                >
                  Login
                </Button>
              </form>

              <Text ta="center" mt="md">
                Don&apos;t have an account?{" "}
                <Anchor<"a">
                  href="#"
                  fw={700}
                  onClick={(event) => {
                    event.preventDefault();
                    navigate("/register");
                  }}
                >
                  Register
                </Anchor>
              </Text>
              <Text ta="center" mt="md">
                <Anchor<"a">
                  href="#"
                  fw={700}
                  onClick={(event) => {
                    event.preventDefault();
                    setForgotPassword(true);
                  }}
                >
                  Forgot password?
                </Anchor>
              </Text>
            </>
          )}
        </Paper>
      </div>
      <Footer />
    </>
  );
}
