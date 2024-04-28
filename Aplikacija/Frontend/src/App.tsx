import { MantineProvider } from "@mantine/core";
import HomePage from "./HomePage/HomePage";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";

const queryClient = new QueryClient();

function App() {
  return (
    <>
      <QueryClientProvider client={queryClient}>
        <MantineProvider>
           <HomePage />
          {/*<Drawer />*/}
        </MantineProvider>
      </QueryClientProvider>
    </>
  );
}

export default App;
