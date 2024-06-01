import { MantineProvider } from "@mantine/core";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { RouterProvider, createBrowserRouter } from "react-router-dom";
import routes from "./Routes/routes";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
    },
  },
});

function App() {
  const router = createBrowserRouter(routes);

  return (
    <>
      <QueryClientProvider client={queryClient}>
        <MantineProvider>
          <RouterProvider router={router} />
        </MantineProvider>
      </QueryClientProvider>
    </>
  );
}

export default App;