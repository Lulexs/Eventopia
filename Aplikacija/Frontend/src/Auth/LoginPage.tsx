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

export function LoginPage() {
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

              <TextInput
                label="Email address"
                placeholder="hello@gmail.com"
                size="md"
              />
              <PasswordInput
                label="Password"
                placeholder="Your password"
                mt="md"
                size="md"
              />
              <Button fullWidth mt="xl" size="md">
                Login
              </Button>

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
