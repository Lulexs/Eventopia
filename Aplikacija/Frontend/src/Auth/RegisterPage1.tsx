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
} from "@mantine/core";
import classes from "./RegisterPage.module.css";
import { PasswordStrength } from "./Utils/PasswordStrength";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import MapWithInput from "./Utils/MapInput";
import { LatLng } from "leaflet";

export interface RegisterPage1Props {
  enterDrawer: Function;
}

export function RegisterPage1(props: RegisterPage1Props) {
  const [position, setPosition] = useState<LatLng>(new LatLng(51.505, -0.09));

  const navigate = useNavigate();
  const [userType, setUserType] = useState<string | null>("Visitor");
  return (
    <Paper
      withBorder
      p="70"
      w="30%"
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
        <TextInput mb={10} label="First name" placeholder="John" required />
        <TextInput mb={10} label="Last name" placeholder="Doe" required />
        <TextInput
          mb={10}
          label="Email"
          placeholder="you@mantine.dev"
          required
        />
        <PasswordStrength />
        <PasswordInput
          placeholder="Selected password"
          label="Repeat password"
          mt={10}
        />
        <Select
          mt={10}
          label="User type"
          defaultValue="Visitor"
          value={userType}
          onChange={setUserType}
          data={["Visitor", "Host", "Space owner"]}
        />
        {(userType == "Host" || userType == "Space owner") && (
          <>
            <TextInput
              mt={10}
              mb={10}
              label="Address"
              placeholder="123 Main Street"
              required
            />
            <TextInput mb={10} label="City" placeholder="New York" required />
            <FileInput
              label="Identification image"
              description="Image is used for identification purposes and it is not saved"
              placeholder="Personal identification"
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
              label="Select location address"
              disabled={true}
              value={`${position.lat} ${position.lng}`}
            ></TextInput>
            <MapWithInput position={position} setPosition={setPosition} />
          </>
        )}
        <Button fullWidth mt="xl">
          Sign up
        </Button>
      </Paper>
    </Paper>
  );
}
