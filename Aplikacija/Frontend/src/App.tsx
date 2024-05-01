import { MantineProvider } from "@mantine/core";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { RouterProvider, createBrowserRouter } from "react-router-dom";
import routes from "./Routes/routes";
import Drawer from "./Reservations/Drawer/Drawer";
import Reservation from "./Reservations/Reservation/Reservation";

const queryClient = new QueryClient();

function App() {
  const router = createBrowserRouter(routes);

  return (
    <>
      <QueryClientProvider client={queryClient}>
        <MantineProvider>
          {/* <RouterProvider router={router} /> */}
          <Reservation
            title="Rammstain"
            location="Nis, Serbia"
            date="22.04.2024. 21:00"
            img="../../src/assets/ramstajn.png"
          />
          {/* <Drawer /> */}
        </MantineProvider>
      </QueryClientProvider>
    </>
  );
}

export default App;
