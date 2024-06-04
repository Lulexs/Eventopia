import {
  TextInput,
  Anchor,
  Paper,
  Title,
  Text,
  Button,
  Select,
  PasswordInput,
  SimpleGrid,
  Image,
  InputLabel,
} from "@mantine/core";
import classes from "./RegisterPage.module.css";
import { PasswordStrength } from "./Utils/PasswordStrength";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useForm } from "@mantine/form";
import { DateInput } from "@mantine/dates";
import axios from "axios";
import { useDispatch } from "react-redux";
import { login } from "../store/features/auth";

export function RegisterPage1() {
  const navigate = useNavigate();
  const [userType, setUserType] = useState<string | null>("Visitor");
  const [selectedAvatar, setSelectedAvatar] = useState<number>(0);
  const dispatch = useDispatch();

  const registerForm = useForm({
    mode: "controlled",
    initialValues: {
      firstName: "",
      lastName: "",
      email: "",
      password: "",
      confirmPassword: "",
      phoneNumber: "",
      birthday: new Date(2024, 1, 1),
      userType: "Visitor",
      avatar:
        "https://raw.githubusercontent.com/mantinedev/mantine/master/.demo/avatars/avatar-1.png",
      address: "",
      city: "",
    },

    validate: {
      email: (value) => (/^\S+@\S+$/.test(value) ? null : "Invalid email"),
      password: (value) => (value.length > 0 ? null : "Empty password field"),
      firstName: (value) =>
        value.length > 0 ? null : "Empty first name field",
      lastName: (value) => (value.length > 0 ? null : "Empty last name field"),
      phoneNumber: (value) =>
        value.length > 0 ? null : "Required field missing",
      address: (value) =>
        userType == "Visitor" || (userType != "Visitor" && value.length > 0)
          ? null
          : "Empty address field",
      userType: (value) =>
        value == "Visitor" || value == "Host" || value == "Space owner"
          ? null
          : "Wrong user type",
      city: (value) =>
        userType == "Visitor" || (userType != "Visitor" && value.length > 0)
          ? null
          : "Empty city field",
      confirmPassword: (value, values) =>
        value !== values.password ? "Passwords did not match" : null,
    },
  });

  return (
    <Paper
      withBorder
      p="70"
      className={classes.pejper}
      my={40}
      style={{ opacity: 0.85 }}
      radius="md"
    >
      <Title ta="center" className={classes.title}>
        Sign up
      </Title>
      <Text c="dimmed" size="sm" ta="center" mt={5}>
        Have an account yet?{" "}
        <Anchor
          size="sm"
          component="button"
          onClick={(event) => {
            event.stopPropagation();
            navigate("/login");
          }}
        >
          Log in
        </Anchor>
      </Text>

      <Paper withBorder shadow="md" p={30} mt={30} radius="md">
        <form
          onSubmit={registerForm.onSubmit((_, event) => {
            event?.stopPropagation();
          })}
        >
          <TextInput
            mb={10}
            label="First name"
            placeholder="John"
            required
            key={registerForm.key("firstName")}
            {...registerForm.getInputProps("firstName")}
          />
          <TextInput
            mb={10}
            label="Last name"
            placeholder="Doe"
            required
            key={registerForm.key("lastName")}
            {...registerForm.getInputProps("lastName")}
          />
          <TextInput
            mb={10}
            label="Email"
            placeholder="example@gmail.com"
            required
            key={registerForm.key("email")}
            {...registerForm.getInputProps("email")}
          />
          <PasswordStrength
            label="Password"
            placeholder="Your password"
            key={registerForm.key("password")}
            useFormProps={{ ...registerForm.getInputProps("password") }}
          />
          <PasswordInput
            required
            placeholder="Selected password"
            label="Repeat password"
            key={registerForm.key("confirmPassword")}
            {...registerForm.getInputProps("confirmPassword")}
            mt={10}
          />
          <TextInput
            mb={10}
            label="Phone number"
            placeholder="012456789"
            required
            key={registerForm.key("phoneNumber")}
            {...registerForm.getInputProps("phoneNumber")}
          />
          <DateInput
            required
            label="Birthday"
            key={registerForm.key("birthday")}
            {...registerForm.getInputProps("birthday")}
          />

          <Select
            required
            mt={10}
            label="User type"
            data={["Visitor", "Host", "Space owner"]}
            key={registerForm.key("userType")}
            {...registerForm.getInputProps("userType")}
            value={userType}
            onChange={(value) => {
              setUserType(value);
            }}
          />
          {userType == "Visitor" && (
            <>
              <InputLabel mt={20}>Avatar</InputLabel>
              <SimpleGrid
                cols={3}
                key={registerForm.key("avatar")}
                {...registerForm.getInputProps("avatar")}
              >
                {Array.from({ length: 9 }).map((_, idx) => (
                  <Image
                    key={idx}
                    src={`https://raw.githubusercontent.com/mantinedev/mantine/master/.demo/avatars/avatar-${
                      idx + 1
                    }.png`}
                    styles={
                      selectedAvatar == idx
                        ? {
                            root: {
                              border: "2px solid blue",
                              padding: "5px",
                            },
                          }
                        : {}
                    }
                    onClick={(event) => {
                      event.stopPropagation();
                      registerForm.setFieldValue(
                        "avatar",
                        `https://raw.githubusercontent.com/mantinedev/mantine/master/.demo/avatars/avatar-${
                          idx + 1
                        }.png`
                      );
                      setSelectedAvatar(idx);
                    }}
                  />
                ))}
              </SimpleGrid>
            </>
          )}
          {(userType == "Host" || userType == "Space owner") && (
            <>
              <TextInput
                mt={10}
                mb={10}
                label="Address"
                placeholder="123 Main Street"
                required
                key={registerForm.key("address")}
                {...registerForm.getInputProps("address")}
              />
              <TextInput
                mb={10}
                label="City"
                placeholder="New York"
                required
                key={registerForm.key("city")}
                {...registerForm.getInputProps("city")}
              />
            </>
          )}
          <Button
            type="submit"
            fullWidth
            mt="xl"
            onClick={async (event) => {
              event.stopPropagation();
              const values = registerForm.getValues();
              if (registerForm.validate().hasErrors) {
                console.log("Here");
                return;
              }
              await axios
                .post(`${import.meta.env.VITE_DB_SERVER}/Account/register`, {
                  ...values,
                  userType: userType,
                })
                .then((resp) => {
                  const obj = JSON.parse(atob(resp.data.token.split(".")[1]));
                  dispatch(
                    login({
                      userId: obj["nameid"],
                      token: resp.data.token,
                      email: obj["email"],
                      userType: obj["role"],

                      firstName: resp.data.firstName,
                      lastName: resp.data.lastName,
                      birthday: resp.data.dateOfBirth,
                      phoneNumber: resp.data.phoneNumber,
                      avatar: resp.data.avatar,
                      address: resp.data.address,
                      city: resp.data.city,
                    })
                  );
                  navigate("/");
                })
                .catch((err) => {
                  console.error(err);
                  alert(err.response.data.detail);
                });
            }}
          >
            Sign up
          </Button>
        </form>
      </Paper>
    </Paper>
  );
}
