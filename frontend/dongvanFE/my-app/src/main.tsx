import React from "react";
import ReactDOM from "react-dom/client";
import { RouterProvider } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { router } from "./router";   

import "./App.css";

const qc = new QueryClient();
const rootEl = document.getElementById("root");
if (!rootEl) throw new Error('Root element "#root" not found');

ReactDOM.createRoot(rootEl).render(
  <React.StrictMode>
    <QueryClientProvider client={qc}>
      <RouterProvider router={router} />
    </QueryClientProvider>
  </React.StrictMode>
);