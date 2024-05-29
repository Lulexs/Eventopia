import {
  TextInput,
  Anchor,
  Paper,
  Title,
  Text,
  Button,
  Select,
  PasswordInput,
  FileInput,
  SimpleGrid,
  Image,
  InputLabel,
} from "@mantine/core";
import classes from "./RegisterPage.module.css";
import { PasswordStrength } from "./Utils/PasswordStrength";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import MapWithInput from "./Utils/MapInput";
import { LatLng } from "leaflet";
import { useForm } from "@mantine/form";

export interface RegisterPage1Props {
  enterDrawer: Function;
}

export function RegisterPage1(props: RegisterPage1Props) {
  const [position, setPosition] = useState<LatLng>(new LatLng(51.505, -0.09));

  const registerForm = useForm({
    mode: "controlled",
    initialValues: {
      firstName: "",
      lastName: "",
      email: "",
      password: "",
      userType: "Visitor",
      avatar: 0,
      address: "",
      city: "",
      identificationImage: null,
    },

    validate: {
      email: (value) => (/^\S+@\S+$/.test(value) ? null : "Invalid email"),
      password: (value) => (value.length > 0 ? null : "Empty password field"),
      firstName: (value) =>
        value.length > 0 ? null : "Empty first name field",
      lastName: (value) => (value.length > 0 ? null : "Empty last name field"),
      address: (value, { userType }) =>
        userType != "Visitor" && value.length > 0
          ? null
          : "Empty address field",
      city: (value, { userType }) =>
        userType != "Visitor" && value.length > 0 ? null : "Empty city field",
      identificationImage: (value, { userType }) =>
        userType != "Visitor" && value != null ? null : "Id image is required",
    },
  });

  const navigate = useNavigate();
  const [userType, setUserType] = useState<string | null>("Visitor");
  const [selectedAvatar, setSelectedAvatar] = useState<number>(0);

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
          onSubmit={registerForm.onSubmit((values, event) => {
            event?.stopPropagation();
            console.log(values);
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
            placeholder="you@mantine.dev"
            required
            key={registerForm.key("email")}
            {...registerForm.getInputProps("email")}
          />
          <PasswordStrength
            key={registerForm.key("password")}
            {...registerForm.getInputProps("password")}
          />
          <PasswordInput
            required
            placeholder="Selected password"
            label="Repeat password"
            mt={10}
          />
          <Select
            required
            mt={10}
            label="User type"
            defaultValue="Visitor"
            data={["Visitor", "Host", "Space owner"]}
            key={registerForm.key("userType")}
            {...registerForm.getInputProps("userType")}
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
                      registerForm.setFieldValue("avatar", idx);
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
              <FileInput
                required
                label="Identification image"
                description="Image is used for identification purposes and it is not saved"
                placeholder="Personal identification"
                key={registerForm.key("identificationImage")}
                {...registerForm.getInputProps("identificationImage")}
              />
            </>
          )}
          {userType == "Space owner" && (
            <>
              <Button
                mt={10}
                mb={10}
                fullWidth
                onClick={() => props.enterDrawer()}
              >
                Add space image
              </Button>

              <TextInput
                required
                label="City"
                placeholder="New York"
                mb={10}
              ></TextInput>
              <TextInput
                required
                label="Country"
                placeholder="USA"
                mb={10}
              ></TextInput>
              <TextInput
                required
                label="Select location address"
                placeholder="123 Street"
                mb={10}
              ></TextInput>
              <TextInput
                required
                label="Select location address"
                disabled={true}
                value={`${position.lat} ${position.lng}`}
              ></TextInput>
              <MapWithInput position={position} setPosition={setPosition} />
            </>
          )}
          <Button
            type="submit"
            fullWidth
            mt="xl"
            onClick={(event) => {
              event.stopPropagation();
              console.log(registerForm.getValues());
            }}
          >
            Sign up
          </Button>
        </form>
      </Paper>
    </Paper>
  );
}
